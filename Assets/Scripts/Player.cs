using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

public class Player {
	
	private const int initHealth = 100, healthPerEndurance = 10;

    public static int strength = 10;

    public static int endurance = 10;

    public static int agility = 15;

    public static int initiative { get { return agility; } private set{;} }

	public static int health { get; private set; }

	public static int maxHealth { get; private set; }

	public static int armorClass { get; private set; }

	public static WeaponData weapon { get; private set; }
	public static ArmorData armor { get; private set; }
	public static HelmetData helmet { get; private set; }
	public static ShieldData shield { get; private set; }
	public static GloveData glove { get; private set; }
    public static AmuletData amulet { get; private set; }
    public static RingData ring_1 { get; private set; }
    public static RingData ring_2 { get; private set; }

    public static int damage { get { return strength + (weapon == null ? 0 : weapon.damage); } private set{ ; } }

    private static float dmg;
	public static int randomDamage { 
		get {
            dmg = damage;
            return (int)UnityEngine.Random.Range(dmg - (Mathf.RoundToInt(dmg * .2f)), dmg + (Mathf.RoundToInt(dmg * .2f)));
		} private set {;} }

	public static FightInterface fightInterface;

    public static StatusScreen statusScreen;

	public static FightScreen fightScreen;

    private static float currentExperience = 0, nextLevelExperience = 100;

    public static float experience = currentExperience / nextLevelExperience;

	public static void init () {
        health = maxHealth = initHealth + endurance * healthPerEndurance;
	}

	public static void equipWeapon (WeaponData weapon) {
		Player.weapon = weapon;
	}

	public static void equipArmor (ArmorModifier armorMod) {
        if (armorMod == null) { Debug.Log("Wrong usage, can`t unEquip unknown armorModifier"); }
        if (armorMod is ArmorData) { Player.armor = (ArmorData)armorMod; }
        else if (armorMod is ShieldData) { Player.shield = (ShieldData)armorMod; }
        else if (armorMod is HelmetData) { Player.helmet = (HelmetData)armorMod; }
        else if (armorMod is GloveData) { Player.glove = (GloveData)armorMod; }
        calculateArmorClass();
//        statusScreen.updatePlayerImage();
	}

	public static void unEquipArmor (ArmorModifier armorMod) {
		if (armorMod is ArmorData) { Player.armor = null; }
		else if (armorMod is ShieldData) { Player.shield = null; }
		else if (armorMod is HelmetData) { Player.helmet = null; }
		else if (armorMod is GloveData) { Player.glove = null; }
        calculateArmorClass();
//        statusScreen.updatePlayerImage();
	}

    public static void equipAmulet (AmuletData amulet) {
        Player.amulet = amulet;
    }

    public static void equipRing (RingData ring, int index) {
        switch (index) {
            case 1: Player.ring_1 = ring; break;
            case 2: Player.ring_2 = ring; break;
            default: Debug.Log("Unknown index for ring: " + index); break;
        }
    }

	private static void calculateArmorClass () {
        armorClass = (armor == null? 0: armor.armorClass()) + (helmet == null? 0: helmet.armorClass()) + (shield == null? 0: shield.armorClass()) + (glove == null? 0: glove.armorClass());
	}

	public static int hitPlayer (int damageAmount)  {
		int armorAmount = armorClass;

		if (fightScreen.getStatusEffectByType(StatusEffectType.ARMORED, true).inProgress) {
			armorAmount += fightScreen.getStatusEffectByType(StatusEffectType.ARMORED, true).value;
		}

		if (damageAmount <= armorAmount) {
			return 0;
		} else {
			health -= (damageAmount - armorAmount);
			if (fightInterface.gameObject.activeInHierarchy) { fightInterface.updatePlayerBar(); }
			return damageAmount - armorAmount;
		}
	}

	public static int heal (int amount) {
		if (amount + health > maxHealth) {
			int heal = maxHealth - health;
			setHealthToMax();
			if (fightInterface.gameObject.activeInHierarchy) { fightInterface.updatePlayerBar(); }
			return heal;
		} else {
			health += amount;
			if (fightInterface.gameObject.activeInHierarchy) { fightInterface.updatePlayerBar(); }
			return amount;
		}
	}

	public static void setHealthToMax () {
		health = maxHealth;
        if (statusScreen.gameObject.activeInHierarchy) {
            statusScreen.updateAttributes();
        }
	}
}