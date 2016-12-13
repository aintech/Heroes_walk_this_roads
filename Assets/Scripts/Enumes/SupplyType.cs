using UnityEngine;
using System.Collections;

public enum SupplyType {
	HEALTH_POTION, ARMOR_POTION, REGENERATION_POTION, SPEED_POTION,
	BLINDING_POWDER, PARALIZING_DUST
}

public static class SupplyDescription {

	public static string name (this SupplyType type) {
		switch (type) {
			case SupplyType.HEALTH_POTION: return "Зелье здоровья";
			case SupplyType.ARMOR_POTION: return "Зелье защиты";
			case SupplyType.REGENERATION_POTION: return "Зелье регенерации";
			case SupplyType.SPEED_POTION: return "Зелье скорости";
			case SupplyType.BLINDING_POWDER: return "Слепящий порошок";
			case SupplyType.PARALIZING_DUST: return "Парализующая пыль";
			default: Debug.Log("Unknown supply type: " + type); return "";
		}
	}

	public static string description (this SupplyType type) {
		return "";
	}

	public static float volume (this SupplyType type) {
		return 0;
	}

	public static int cost (this SupplyType type) {
		return 10;
	}

	//Значение - сколько лечит, добавляет брони и т.д.
	public static int value (this SupplyType type) {
		switch (type) {
			case SupplyType.HEALTH_POTION: return 200;
			case SupplyType.ARMOR_POTION: return 10;
			case SupplyType.REGENERATION_POTION: return 50;
			case SupplyType.SPEED_POTION: return 1;
			default: return 0;
		}
	}

	public static StatusEffectType toStatusEffectType (this SupplyType type) {
		switch (type) {
			case SupplyType.BLINDING_POWDER: return StatusEffectType.BLINDED;
			case SupplyType.PARALIZING_DUST: return StatusEffectType.PARALIZED;
			case SupplyType.ARMOR_POTION: return StatusEffectType.ARMORED;
			case SupplyType.REGENERATION_POTION: return StatusEffectType.REGENERATION;
			case SupplyType.SPEED_POTION: return StatusEffectType.SPEED;
			case SupplyType.HEALTH_POTION: return StatusEffectType.HEAL;
			default: Debug.Log ("Unmapped supply type: " + type); return StatusEffectType.NONE;
		}
	}

	public static FightEffectType toFightEffectType (this SupplyType type) {
		switch (type) {
			case SupplyType.BLINDING_POWDER: return FightEffectType.BLIND;
			case SupplyType.PARALIZING_DUST: return FightEffectType.PARALIZED;
			case SupplyType.ARMOR_POTION: return FightEffectType.ARMORED;
			case SupplyType.REGENERATION_POTION: return FightEffectType.REGENERATION;
			case SupplyType.SPEED_POTION: return FightEffectType.SPEED;
			case SupplyType.HEALTH_POTION: return FightEffectType.HEAL;
			default: Debug.Log("Unknown supply type: " + type); return FightEffectType.NONE;
		}
	}
}