using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

public class Hero : Character {

	public HeroType type { get; private set; }

	public WeaponData weapon { get; private set; }
	public ArmorData armor { get; private set; }
	public HelmetData helmet { get; private set; }
	public ShieldData shield { get; private set; }
	public GloveData glove { get; private set; }
	public AmuletData amulet { get; private set; }
	public RingData ring_1 { get; private set; }
	public RingData ring_2 { get; private set; }

	private float currentExperience = 0, nextLevelExperience = 100;

	public float experience { get { return currentExperience / nextLevelExperience; } private set {;} }

	public Hero init (HeroType type) {
		this.type = type;
		innerInit(type.strenght(), type.endurance(), type.agility());
		return this;
	}

	public override int damage () { return strength + (weapon == null ? 0 : weapon.damage); }

	public void equipWeapon (WeaponData weapon) {
		this.weapon = weapon;
	}

	public void equipArmor (ArmorModifier armorMod) {
		if (armorMod == null) { Debug.Log("Wrong usage, can`t unEquip unknown armorModifier"); }
		if (armorMod is ArmorData) { this.armor = (ArmorData)armorMod; }
		else if (armorMod is ShieldData) { this.shield = (ShieldData)armorMod; }
		else if (armorMod is HelmetData) { this.helmet = (HelmetData)armorMod; }
		else if (armorMod is GloveData) { this.glove = (GloveData)armorMod; }
		calculateArmorClass();
//		statusScreen.updatePlayerImage();
	}

	public void unEquipArmor (ArmorModifier armorMod) {
		if (armorMod is ArmorData) { this.armor = null; }
		else if (armorMod is ShieldData) { this.shield = null; }
		else if (armorMod is HelmetData) { this.helmet = null; }
		else if (armorMod is GloveData) { this.glove = null; }
		calculateArmorClass();
//		statusScreen.updatePlayerImage();
	}

	public void equipAmulet (AmuletData amulet) {
		this.amulet = amulet;
	}

	public void equipRing (RingData ring, int index) {
		switch (index) {
		case 1: this.ring_1 = ring; break;
		case 2: this.ring_2 = ring; break;
		default: Debug.Log("Unknown index for ring: " + index); break;
		}
	}

	private void calculateArmorClass () {
		armorClass = (armor == null? 0: armor.armorClass()) + (helmet == null? 0: helmet.armorClass()) + (shield == null? 0: shield.armorClass()) + (glove == null? 0: glove.armorClass());
	}

    public override bool isHero () { return true; }
    public override string name () { return type.name(); }
}