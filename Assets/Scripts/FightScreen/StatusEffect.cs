using UnityEngine;
using System.Collections;

public class StatusEffect : MonoBehaviour {

	public StatusEffectType statusType;

	public bool asPlayer;

	public int value { get; private set; }

	public int duration { get; private set; }

	private StrokeText turnsText;

	private EnemyHolder enemy;

	public bool inProgress { get; private set; }

	public bool isFired { get; private set; }

	private SpriteRenderer render;

	private Color32 disabledColor = new Color32(255, 255, 255, 120), enabledColor = new Color32(255, 255, 255, 255);

	public StatusEffect init () {
		inProgress = false;

        render = GetComponent<SpriteRenderer>();

        turnsText = transform.Find("Turns").GetComponent<StrokeText>().init(render.sortingLayerName, render.sortingOrder + 1);

		gameObject.SetActive(false);

		return this;
	}

	public void initEnemy (EnemyHolder enemy) {
		this.enemy = enemy;
	}

	public void addStatus (int value, int duration) {
		this.value = value;
		this.duration = duration;

		render.color = disabledColor;
		turnsText.setText (duration.ToString ());
		turnsText.gameObject.SetActive (true);
		gameObject.SetActive(true);
		isFired = true;
	}

	public void updateStatus () {
		if (isFired && !inProgress) {
			inProgress = true;
			render.color = enabledColor;
		}
		if (!inProgress) {
			return;
		}
		duration--;
		if (duration >= 0) {
			applyEffect ();
		}
		if (duration == 1) { turnsText.gameObject.SetActive (false); }
		else if (duration == 0 && !statusType.isStatusActiveOnNextTurn()) { endEffect (); }
		else if (duration < 0 && statusType.isStatusActiveOnNextTurn()) { endEffect(); }

		turnsText.setText (duration.ToString ());
	}

	private void applyEffect () {
		switch (statusType) {
			case StatusEffectType.REGENERATION:
				if (asPlayer) {
					Player.heal (value);
				} else {
                    enemy.enemy.heal (value);
				}
				break;
		}
	}

	private void hideEffect () {
		gameObject.SetActive (false);
	}

	public void endEffect () {
		inProgress = false;
		isFired = false;
		gameObject.SetActive (false);
	}
}