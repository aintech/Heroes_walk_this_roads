using UnityEngine;
using System.Collections;

public class Element : MonoBehaviour {

	public Sprite[] elementSprites;

	public Sprite fireElement, waterElement, earthElement, airElement, lightElement, darkElement;

	private SpriteRenderer render;

	private Transform trans;

	public ElementType elementType { get; private set; }

	[HideInInspector]
	public Vector3 cellCenter, target;

	private int row, column;

	private const float MOVE_SPEED = .2f;

	private const float CENTER_DIST = MOVE_SPEED * 2;

	private float x, y;

	public bool goToTarget { get; private set; }
	public bool fading { get; private set; }
	public bool fadeIn { get; private set; }
	public bool fadeOut { get; private set; }

	private float fadingSpeed = .05f;

	private Color color = new Color(1, 1, 1, 1);

	private Vector3 scale = Vector3.one, rotatePoint = new Vector3(0, 0, 1);

	private Collider2D col;

    private ElementsHolder holder;

    public Element init (ElementsHolder holder) {
        this.holder = holder;
		render = GetComponent<SpriteRenderer>();
		col = GetComponent<Collider2D>();
		trans = transform;
		enabled = false;
		return this;
	}

	public void initRandomElement () {
		int rand = Random.Range(0, ElementDescriptor.getElementsCount());
		switch (rand) {
			case 0: initElement(ElementType.FIRE); break;
			case 1: initElement(ElementType.WATER); break;
			case 2: initElement(ElementType.EARTH); break;
			case 3: initElement(ElementType.AIR); break;
//			case 4: initElement(ElementType.LIGHT); break;
//			case 5: initElement(ElementType.DARK); break;
			default: Debug.Log("Unknown element type");break;
		}
	}

	public void prepareFading (bool fadeIn) {
		color.a = fadeIn ? 0 : 1;
		scale.x = fadeIn ? 0 : 1;
		scale.y = fadeIn ? 0 : 1;
		render.color = color;
		gameObject.SetActive (true);
	}

	public void initElement (ElementType elementType) {
		this.elementType = elementType;
		setSprite();
	}

	public SpriteRenderer getRender () {
		return render;
	}

	public void setGoToTarget () {
		goToTarget = true;
		x = trans.localPosition.x;
		y = trans.localPosition.y;
		setAsEnabled ();
	}

	public void initFading (bool fadeIn) {
		this.fadeIn = fadeIn;
		this.fadeOut = !fadeIn;
		fading = true;
		setAsEnabled ();
	}

	void Update () {
		if (goToTarget) { moveToTarget(); }
		if (fading) {
			if ((fadeOut && render.color.a < .001f) || (fadeIn && render.color.a > .99f)) {
				fading = false;
				setAsEnabled ();
				trans.rotation = new Quaternion ();
//                holder.elementsFaded();
			} else {
				color.a += fadeIn? fadingSpeed: -fadingSpeed;
				render.color = color;
				scale.x += fadeIn ? fadingSpeed : -fadingSpeed;
				scale.y += fadeIn ? fadingSpeed : -fadingSpeed;
				trans.localScale = scale;
//				trans.Rotate(rotatePoint, fadeIn? 10: -10);
			}
		}
	}

	private void setAsEnabled () {
		enabled = fading || goToTarget;
	}

	public void refreshElement () {
		color.a = 1;
		trans.localRotation = new Quaternion();
		trans.localScale = scale = Vector3.one;
		render.color = color;
	}

	private void moveToTarget () {
		if (Vector2.Distance(trans.localPosition, target) <= CENTER_DIST) {
			trans.localPosition = target;
			render.sortingOrder = ElementsHolder.START_SORT_ORDER;
			goToTarget = false;
			setAsEnabled ();
		} else {
			if ((target.x - trans.localPosition.x) < -MOVE_SPEED) {
				x = trans.localPosition.x - MOVE_SPEED;
			} else if ((target.x - trans.localPosition.x) > MOVE_SPEED) {
				x = trans.localPosition.x + MOVE_SPEED;
			}
			if ((target.y - trans.localPosition.y) < -MOVE_SPEED) {
				y = trans.localPosition.y - MOVE_SPEED;
			} else if ((target.y - trans.localPosition.y) > MOVE_SPEED) {
				y = trans.localPosition.y + MOVE_SPEED;
			}
			trans.localPosition = new Vector2(x, y);
		}
	}

	private void setSprite () {
		switch (elementType) {
			case ElementType.FIRE: render.sprite = fireElement; break;
			case ElementType.WATER: render.sprite = waterElement; break;
			case ElementType.EARTH: render.sprite = earthElement; break;
			case ElementType.AIR: render.sprite = airElement; break;
			case ElementType.LIGHT: render.sprite = lightElement; break;
			case ElementType.DARK: render.sprite = darkElement; break;
			default: Debug.Log("Unknown element type"); break;
		}
	}

	public void setRowAndColumn (int row, int column) {
		this.row = row;
		this.column = column;
	}

	public int getRow () {
		return row;
	}

	public int getColumn () {
		return column;
	}

	public bool isGoToTarget () {
		return goToTarget;
	}

	public void setActive (bool active) {
		col.enabled = active;
	}
}