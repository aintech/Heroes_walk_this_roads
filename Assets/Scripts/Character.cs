﻿using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public abstract class Character {

	private const int initHealth = 100, healthPerEndurance = 10;

	public int health { get; private set; }

	public int maxHealth { get; private set; }

	public int strength { get; private set; }

	public int endurance { get; private set; }

	public int agility { get; private set; }

	public int initiative { get { return agility; } private set{;} }

	public int armorClass { get; protected set; }

	public Dictionary<StatusEffectType, StatusEffect> statusEffects = new Dictionary<StatusEffectType, StatusEffect>();

	public void innerInit (int strength, int endurance, int agility) {
		this.strength = strength;
		this.endurance = endurance;
		this.agility = agility;

		health = maxHealth = initHealth + endurance * healthPerEndurance;
	}

	public abstract int damage ();

	public int randomDamage () {
		float dmg = damage();
		return (int)UnityEngine.Random.Range(dmg - (Mathf.RoundToInt(dmg * .2f)), dmg + (Mathf.RoundToInt(dmg * .2f)));
	}

	public int hit (int damageAmount)  {
		int armorAmount = armorClass;

		if (statusEffects[StatusEffectType.ARMORED].inProgress) {
			armorAmount += statusEffects[StatusEffectType.ARMORED].value;
		}

		if (damageAmount <= armorAmount) {
			return 0;
		} else {
			health -= (damageAmount - armorAmount);
			//			if (fightInterface.gameObject.activeInHierarchy) { fightInterface.updatePlayerBar(); }
			return damageAmount - armorAmount;
		}
	}

	public int heal (int amount) {
		if (amount + health > maxHealth) {
			int heal = maxHealth - health;
			setHealthToMax();
			//			if (fightInterface.gameObject.activeInHierarchy) { fightInterface.updatePlayerBar(); }
			return heal;
		} else {
			health += amount;
			//			if (fightInterface.gameObject.activeInHierarchy) { fightInterface.updatePlayerBar(); }
			return amount;
		}
	}

	public void setHealthToMax () {
		health = maxHealth;
	}
}