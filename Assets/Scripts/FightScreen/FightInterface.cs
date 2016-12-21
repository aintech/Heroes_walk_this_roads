﻿using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class FightInterface : MonoBehaviour {

//	private Transform enemyHealthBar, playerHealthBar;

//	private EnemyHolder enemy;

//	private Vector3 enemyBarScale, playerBarScale;

//	private Vector3 armorScaleOne = new Vector3(.13f, .13f, 1), armorScaleDouble = new Vector3(.1f, .1f, 1);

//	private float enemyMax;

//	private StrokeText enemyArmorValue;

	private FightScreen fightScreen;

	private List<HeroPortrait> portraits = new List<HeroPortrait>();

	public FightInterface init (FightScreen fightScreen) {
		this.fightScreen = fightScreen;
//		enemyHealthBar = transform.Find("Enemy Health Bar").Find("Bar");
//		playerHealthBar = transform.Find("Player Health Bar").Find("Bar");
//		enemyArmorValue = transform.Find("Enemy Armor Value").GetComponent<StrokeText>().init("default", 5);
//		enemyBarScale = enemyHealthBar.localScale;
//		playerBarScale = playerHealthBar.localScale;
//		Player.fightInterface = this;

//		Transform statusEffectHolder = transform.Find("Player Statuses");
//		for (int i = 0; i < statusEffectHolder.childCount; i++) {
//			fightScreen.playerStatusEffects.Add(statusEffectHolder.GetChild(i).GetComponent<StatusEffect>().init());
//		}
//		statusEffectHolder = transform.Find("Enemy Statuses");
//		for (int i = 0; i < statusEffectHolder.childCount; i++) {
//			fightScreen.enemyStatusEffects.Add(statusEffectHolder.GetChild(i).GetComponent<StatusEffect>().init());
//		}

		Transform portraitsHolder = transform.Find("Portraits");
		for (int i = 0; i < portraitsHolder.childCount; i++) {
			portraits.Add(portraitsHolder.GetChild(i).GetComponent<HeroPortrait>().init());
		}

		gameObject.SetActive(true);

		return this;
	}

//	public void setEnemy (EnemyHolder enemy) {
//		this.enemy = enemy;
//        enemyMax = enemy.enemy.health;
//		updateEnemyBar();
//		updateEnemyArmor();
//		updatePlayerBar();
//	}
//
//	public void updateEnemyBar () {
//        enemyBarScale.y = Mathf.Max(1, enemy.enemy.health) / enemyMax;
//		enemyHealthBar.localScale = enemyBarScale;
//	}
//
//	public void updatePlayerBar () {
//		playerBarScale.y = (float)Mathf.Max(1, Player.health) / (float)Player.maxHealth;
//		playerHealthBar.localScale = playerBarScale;
//	}
//
//	public void updateEnemyArmor () {
//        enemyArmorValue.setText(enemy.enemy.armorClass.ToString());
//        enemyArmorValue.transform.localScale = enemy.enemy.armorClass < 10? armorScaleOne: armorScaleDouble;
//	}

    public void updateHeroRepresentatives () {
        foreach (HeroPortrait port in portraits) {
            port.updateRepresentative();
        }
    }
}