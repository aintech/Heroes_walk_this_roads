using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class ElementEffectPlayer : MonoBehaviour {

	public Transform elementEffectPrefab;

	private List<ElementEffect> elementEffects = new List<ElementEffect>();

	private FightScreen fightScreen;

	private Enemy enemy;

	public void init (FightScreen fightScreen, Enemy enemy) {
		this.fightScreen = fightScreen;
		this.enemy = enemy;

		while (elementEffects.Count <= 3) {
			ElementEffect effect = Instantiate<Transform>(elementEffectPrefab).GetComponent<ElementEffect>();
			effect.init(fightScreen, enemy);
			effect.gameObject.SetActive(false);
			elementEffects.Add(effect);
		}
	}

	public void addEffect (ElementType type, int value, Vector2 pos, int iconsCount) {
		ElementEffect effect = null;
		foreach (ElementEffect other in elementEffects) {
			if (!other.isEffectActive()) {
				effect = other;
				break;
			}
		}

		if (effect == null) {
			effect = Instantiate<Transform>(elementEffectPrefab).GetComponent<ElementEffect>();
			effect.init(fightScreen, enemy);
			elementEffects.Add(effect);
		}

		effect.activateEffect(type, value, pos, iconsCount);

		FightProcessor.FIGHT_ANIM_ENEMY_DONE = false;
	}

	public bool isPlayingEffect () {
		foreach (ElementEffect effect in elementEffects) {
			if (effect.isEffectActive()) {
				return true;
			}
		}
		return false;
	}
}