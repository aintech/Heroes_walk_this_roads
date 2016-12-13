using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class ElementsHolder : MonoBehaviour {

	public Transform elementPrefab;

	private FightScreen fightScreen;

	public const int ROWS = 7, COLUMNS = 8;
	
	private const float CELL_STEP = 1.05f;//расстояние между центрами ячеек
	
	private const float HALF_CELL_STEP = CELL_STEP / 2;

	public const int START_SORT_ORDER = 3;

	private const int DRAGGED_SORT_ORDER = ROWS + START_SORT_ORDER + 2;
	
	private const int AFTER_DRAG_ORDER = DRAGGED_SORT_ORDER - 1;
	
	private const float START_Y = (ROWS / 2) * CELL_STEP;// + HALF_CELL_STEP;

	private const float MAX_X = (COLUMNS / 2) * CELL_STEP - HALF_CELL_STEP;

	private const float MAX_Y = (ROWS / 2) * CELL_STEP;

	private MoveRestrict moveRestrict = MoveRestrict.VERTICAL;
	
	private static Element[,] elements = new Element[ROWS, COLUMNS];
	
	private List<Element> totalMatchHor = new List<Element>(),
						  totalMatchVer = new List<Element>(),
						  allMatch = new List<Element>(),
						  matchLine = new List<Element>();

	private Element draggedElement, changeElement;

	private Vector2 initPos, holdOffset, dir, newPos;

	private FightProcessor fightProcessor;

	public ElementsHolderAnimator holderAnimator { get; private set; }

	public ElementsHolder init (FightScreen fightScreen) {
		fightScreen = transform.parent.GetComponent<FightScreen>();
		fightProcessor = fightScreen.getFightProcessor();
		Element element = null;
		for (int i = 0; i < ROWS; i++) {
			for (int j = 0; j < COLUMNS; j++) {
				element = Instantiate<Transform>(elementPrefab).GetComponent<Element>().init();
				element.transform.SetParent(transform);
				elements[i,j] = element;
				element.setRowAndColumn(i, j);
				element.getRender().sortingOrder = i + START_SORT_ORDER;
			}
		}
		holderAnimator = GetComponent<ElementsHolderAnimator> ().init (fightScreen, elements);
		return this;
	}

	public void initializeElements () {
		Element element = null;
		ElementType[] nearTypes = {ElementType.FIRE, ElementType.FIRE};
		bool checkTypes = false;
		for (int i = 0; i < ROWS; i++) {
			for (int j = 0; j < COLUMNS; j++) {
				element = elements[i,j];
				
				ElementType elementType = getRandomType(null);
				
				//горизонтальные совпадения
				if (j >= 2 && elements[i,j-1].elementType == elementType && elements[i,j-2].elementType == elementType) {
					nearTypes[0] = elementType;
					if (i > 0) {
						nearTypes[1] = elements[i-1,j].elementType;
					}
					checkTypes = true;
				}
				//вертикальные совпадения
				if (i >= 2 && elements[i-1,j].elementType == elementType && elements[i-2,j].elementType == elementType) {
					if (j > 0) {
						nearTypes[0] = elements[i,j-1].elementType;
					}
					nearTypes[1] = elementType;
					checkTypes = true;
				}
				
				if (checkTypes) {
					elementType = getRandomType(nearTypes);
				}
				checkTypes = false;
				
				element.initElement(elementType);

//				element.transform.localPosition = new Vector2(-MAX_X + j * CELL_STEP, START_Y);
				element.cellCenter = getCellPosition(i, j);
				element.target = element.cellCenter;
				element.transform.localPosition = element.target;
				element.gameObject.SetActive(false);
			}
		}
//		holderAnimator.playElementsApperance ();
	}

//	public void startElementsDrop () {
//		foreach (Element element in elements) {
//			element.gameObject.SetActive(true);
//			element.setGoToTarget();
//		}
//	}
	
	public void checkPlayerInput () {
		if (Input.GetMouseButtonDown(0) && draggedElement == null) {
			if (Utils.hit != null && Utils.hit.name.StartsWith("Element")) {
				draggedElement = Utils.hit.GetComponent<Element>();
				draggedElement.getRender().sortingOrder = DRAGGED_SORT_ORDER;
				holdOffset = Utils.mousePos - new Vector2(draggedElement.transform.position.x, draggedElement.transform.position.y);
				initPos = Utils.mousePos;
			}
		}
		if (draggedElement != null) {
			if (Input.GetMouseButtonUp(0)) {
				checkElementDrop();
				draggedElement = null;
			} else {
				moveElement();
			}
		}
	}

	public bool isAllElementsOnCells () {
		foreach (Element element in elements) {
			if (element.isGoToTarget()) {
				return false;
			}
		}
		return true;
	}

	public void rearrangeElements () {
		foreach (Element element in allMatch) {
			elements[element.getRow(), element.getColumn()] = null;
		}
		int step = 1, count = 0, compareRow = 0;
		for (int row = ROWS-1; row >= 0; row--) {
			for (int col = 0; col < COLUMNS; col++) {
				Element element = elements[row, col];
				while (element == null) {
					compareRow = row - step;
					if (compareRow < 0) {
						element = allMatch[count];
						element.setRowAndColumn(row, col);
						elements[row, col] = element;
						Vector2 cellPos = getCellPosition(row, col);
						element.cellCenter = cellPos;
						count++;
						break;
					}
					Element changeElement = elements[compareRow, col];
					if (changeElement == null) {
						if (compareRow == 0) {
							element = allMatch[count];
							element.setRowAndColumn(row, col);
							elements[row, col] = element;
							Vector2 cellPos = getCellPosition(row, col);
							element.cellCenter = cellPos;
							count++;
							break;
						}
					} else {
						elements[changeElement.getRow(), changeElement.getColumn()] = null;
						element = changeElement;
						element.setRowAndColumn(row, col);
						elements[row, col] = element;
						Vector2 cellPos = getCellPosition(row, col);
						element.cellCenter = cellPos;
						break;
					}
					step++;
				}
				step = 1;
			}
		}
	}
	
	private Vector2 getCellPosition (int row, int col) {
		return new Vector2(-MAX_X + col * CELL_STEP, MAX_Y - row * CELL_STEP);
	}
	
	public void moveElement () {
		dir = Utils.mousePos - initPos;
		newPos = new Vector2(Utils.mousePos.x - holdOffset.x - transform.position.x, 
		                     Utils.mousePos.y - holdOffset.y - transform.position.y);
		
		if (Vector2.Distance(dir, Vector3.zero) < .3f) {
			if (Mathf.Abs(dir.x) > Mathf.Abs(dir.y)) {
				moveRestrict = MoveRestrict.HORIZONTAL;
			} else {
				moveRestrict = MoveRestrict.VERTICAL;
			}
		} else {
			if (moveRestrict == MoveRestrict.HORIZONTAL) {
				newPos.y = draggedElement.cellCenter.y;
				if (dir.x > CELL_STEP) {
					newPos.x = draggedElement.cellCenter.x + CELL_STEP;
				} else if (dir.x < -CELL_STEP) {
					newPos.x = draggedElement.cellCenter.x - CELL_STEP;
				}
			} else if (moveRestrict == MoveRestrict.VERTICAL) {
				newPos.x = draggedElement.cellCenter.x;
				if (dir.y > CELL_STEP) {
					newPos.y = draggedElement.cellCenter.y + CELL_STEP;
				} else if (dir.y < -CELL_STEP) {
					newPos.y = draggedElement.cellCenter.y - CELL_STEP;
				}
			}
		}
		
		if ((draggedElement.getColumn() == 0 && dir.x < 0) || (draggedElement.getColumn() == (COLUMNS - 1) && dir.x > 0)) {
			newPos.x = draggedElement.cellCenter.x;
		}
		if ((draggedElement.getRow() == 0 && dir.y > 0) || (draggedElement.getRow() == (ROWS - 1) && dir.y < 0)) {
			newPos.y = draggedElement.cellCenter.y;
		}
		
		draggedElement.transform.localPosition = newPos;
		
		Vector3 offset = draggedElement.transform.localPosition - draggedElement.cellCenter;
		
		if (changeElement == null) {
			if (offset.x > HALF_CELL_STEP + .1f) {
				changeElement = elements[draggedElement.getRow(), draggedElement.getColumn() + 1];
			} else if (offset.x < -HALF_CELL_STEP - .1f) {
				changeElement = elements[draggedElement.getRow(), draggedElement.getColumn() - 1];
			} else if (offset.y > HALF_CELL_STEP + .1f) {
				changeElement = elements[draggedElement.getRow() - 1, draggedElement.getColumn()];
			} else if (offset.y < -HALF_CELL_STEP - .1f) {
				changeElement = elements[draggedElement.getRow() + 1, draggedElement.getColumn()];
			}
			if (changeElement != null) {
				changeElement.target = draggedElement.cellCenter;
				changeElement.setGoToTarget();
			}
		} else {
			bool moveBack = false;
			if (moveRestrict == MoveRestrict.HORIZONTAL && (offset.x < HALF_CELL_STEP - .1f) && (offset.x > -HALF_CELL_STEP + .1f)) {
				moveBack = true;
			}
			if (moveRestrict == MoveRestrict.VERTICAL && (offset.y < HALF_CELL_STEP - .1f) && (offset.y > -HALF_CELL_STEP + .1f)) {
				moveBack = true;
			}
			if (moveBack) {
				changeElement.target = changeElement.cellCenter;
				changeElement.setGoToTarget();
				changeElement = null;
			}
		}
	}
	
	public void checkElementDrop () {
		if (changeElement != null) {
			Vector3 cellCenter = changeElement.cellCenter;
			changeElement.cellCenter = draggedElement.cellCenter;
			draggedElement.cellCenter = cellCenter;
			
			int row = changeElement.getRow();
			int col = changeElement.getColumn();
			changeElement.setRowAndColumn(draggedElement.getRow(), draggedElement.getColumn());
			draggedElement.setRowAndColumn(row, col);
			
			elements[changeElement.getRow(), changeElement.getColumn()] = changeElement;
			elements[draggedElement.getRow(), draggedElement.getColumn()] = draggedElement;
			
			FightProcessor.PLAYER_MOVE_DONE = true;
		}
		
		draggedElement.target = draggedElement.cellCenter;
		draggedElement.setGoToTarget();
		draggedElement.getRender().sortingOrder = AFTER_DRAG_ORDER;
		changeElement = null;
		
		checkElementsMatch();
	}

	public void refreshSortingOrder () {
		for (int row = 0; row < ROWS; row++) {
			for (int col = 0; col < COLUMNS; col++) {
				elements[row, col].getRender().sortingOrder = row + START_SORT_ORDER;
			}
		}
	}

	public void repositionMatchingElements () {
		foreach (Element element in allMatch) {
			element.transform.localPosition = new Vector2(-MAX_X + element.getColumn() * CELL_STEP, START_Y);
			element.initRandomElement();
			element.refreshElement();
		}
		allMatch.Clear();
	}

	public void setElementsGoToCenter () {
		foreach (Element element in elements) {
			element.target = element.cellCenter;
			element.setGoToTarget();
		}
	}
	
	public bool checkElementsMatch () {
		Element element = null, next = null;
		int step = 1;
		
		//Проверяем ряды на совпадение
		int compareCol = 0;
		for (int row = 0; row < ROWS; row++) {
			for (int col = 0; col < COLUMNS-2; col++) {
				element = elements[row, col];
				if (totalMatchHor.Contains(element)) {
					continue;
				}
				
				while (true) {
					compareCol = col + step;
					if (compareCol >= COLUMNS) {
						break;
					}
					next = elements[row, compareCol];
					if (next.elementType == element.elementType) {
						matchLine.Add(next);
						if (compareCol == (COLUMNS - 1) && (matchLine.Count >= 2)) {
							fightProcessor.addToTurnResult(element.elementType, matchLine.Count + 1, getMiddlePoint(matchLine));
							totalMatchHor.Add(element);
							totalMatchHor.AddRange(matchLine);
							matchLine.Clear();
						}
					} else {
						if (matchLine.Count >= 2) {
							fightProcessor.addToTurnResult(element.elementType, matchLine.Count + 1, getMiddlePoint(matchLine));
							totalMatchHor.Add(element);
							totalMatchHor.AddRange(matchLine);
							matchLine.Clear();
							break;
						} else {
							matchLine.Clear();
							break;
						}
					}
					step++;
				}
				step = 1;
				matchLine.Clear();
			}
			matchLine.Clear();
		}
		
		//Проверяем колонны на совпадение
		int compareRow = 0;
		for (int row = 0; row < ROWS-2; row++) {
			for (int col = 0; col < COLUMNS; col++) {
				element = elements[row, col];
				if (totalMatchVer.Contains(element)) {
					continue;
				}
				
				while (true) {
					compareRow = row + step;
					if (compareRow >= ROWS) {
						break;
					}
					next = elements[compareRow, col];
					if (next.elementType == element.elementType) {
						matchLine.Add(next);
						if (compareRow == (ROWS - 1) && (matchLine.Count >= 2)) {
							fightProcessor.addToTurnResult(element.elementType, matchLine.Count + 1, getMiddlePoint(matchLine));
							totalMatchVer.Add(element);
							totalMatchVer.AddRange(matchLine);
							matchLine.Clear();
						}
					} else {
						if (matchLine.Count >= 2) {
							fightProcessor.addToTurnResult(element.elementType, matchLine.Count + 1, getMiddlePoint(matchLine));
							totalMatchVer.Add(element);
							totalMatchVer.AddRange(matchLine);
							matchLine.Clear();
							break;
						} else {
							matchLine.Clear();
							break;
						}
					}
					step++;
				}
				step = 1;
				matchLine.Clear();
			}
			matchLine.Clear();
		}
		
		allMatch.AddRange(totalMatchHor);
		foreach (Element fi in totalMatchVer) {
			if (!allMatch.Contains(fi)) {
				allMatch.Add(fi);
			}
		}
		
		foreach (Element fi in allMatch) {
			fi.initFading (false);
		}
		
		matchLine.Clear();
		totalMatchHor.Clear();
		totalMatchVer.Clear();
		
		rearrangeElements();
		
		FightProcessor.ELEMENTS_ANIM_DONE = (allMatch.Count == 0);
		
		fightProcessor.calculateHeroTurnResults();
		
		return allMatch.Count > 0;
	}

	private Vector2 getMiddlePoint (List<Element> elements) {
		float x = 0, y = 0;
		foreach (Element element in elements) {
			x += element.transform.position.x;
			y += element.transform.position.y;
		}
		x = x / elements.Count;
		y = y / elements.Count;
		return new Vector2(x, y);
	}

	private ElementType getRandomType (ElementType[] exclusion) {
		ElementType elementType = ElementType.FIRE;
		int rand = Random.Range(0, ElementDescriptor.getElementsCount());
		if (exclusion == null) {
			switch (rand) {
				case 0: elementType = ElementType.FIRE; break;
				case 1: elementType = ElementType.WATER; break;
				case 2: elementType = ElementType.EARTH; break;
				case 3: elementType = ElementType.AIR; break;
				case 4: elementType = ElementType.LIGHT; break;
				case 5: elementType = ElementType.DARK; break;
				default: Debug.Log("Unknown element type");break;
			}
		} else {
			while (elementType == exclusion[0] || elementType == exclusion[1]) {
				rand = Random.Range(0, ElementDescriptor.getElementsCount());
				switch (rand) {
					case 0: elementType = ElementType.FIRE; break;
					case 1: elementType = ElementType.WATER; break;
					case 2: elementType = ElementType.EARTH; break;
					case 3: elementType = ElementType.AIR; break;
					case 4: elementType = ElementType.LIGHT; break;
					case 5: elementType = ElementType.DARK; break;
					default: Debug.Log("Unknown element type");break;
				}
			}
		}
		return elementType;
	}

	public void setActive (bool active) {
		foreach (Element element in elements) {
			element.setActive(active);
		}
	}

	private enum MoveRestrict {
		HORIZONTAL, VERTICAL
	}
}