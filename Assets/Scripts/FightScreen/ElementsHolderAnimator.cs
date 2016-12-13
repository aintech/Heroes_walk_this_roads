using UnityEngine;
using System.Collections;

public class ElementsHolderAnimator : MonoBehaviour {

	private FightScreen fightScreen;

	private Element[,] elements;

	private const float nextPeriod = .1f;

	private float nextTime;

	private int index;

	private bool apperance;

	public ElementsHolderAnimator init (FightScreen fightScreen, Element[,] elements) {
		this.fightScreen = fightScreen;
		this.elements = elements;
		enabled = false;
		return this;
	}

	public void playElementsDisapperance () {
		foreach (Element element in elements) {
			element.prepareFading (false);
		}
		FightProcessor.ELEMENTS_ANIM_DONE = false;
		index = 0;
		initNextLine ();
		apperance = false;
		enabled = true;
	}

	public void playElementsApperance () {
		foreach (Element element in elements) {
			element.prepareFading (true);
		}
		FightProcessor.ELEMENTS_ANIM_DONE = false;
		index = 0;
		apperance = true;
		enabled = true;
	}

	void Update () {
		if (nextTime < Time.time) {
			initNextLine ();
		}
	}

	private void calcNextTime () {
		nextTime = Time.time + nextPeriod;
	}

	private void initNextLine () {
		//Пробегаемся по рядам
		if (index < ElementsHolder.COLUMNS) {
			for (int i = 0; i <= index; i++) {
				if (i >= ElementsHolder.ROWS) {
					break;
				}
				if (!elements [i, index].fading) {
					elements [i, index].initFading (apperance);
				}
			}
		}
		//Пробегаемся по колонкам
		if (index < ElementsHolder.ROWS) {
			for (int i = 0; i <= index; i++) {
				if (i >= ElementsHolder.COLUMNS) {
					break;
				}
				if (!elements [index, i].fading) {
					elements [index, i].initFading (apperance);
				}
			}
		}
		calcNextTime ();
		index++;
		if (index > ElementsHolder.COLUMNS && index > ElementsHolder.ROWS) {
			if (!apperance) {
				fightScreen.showFightEndDisplay ();
			}
			apperance = enabled = false;
			FightProcessor.ELEMENTS_ANIM_DONE = true;
		}
	}
}