using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public enum StatusEffectType {
    HERO_HEAVY_GUARD, HERO_INVULNERABILITY_SPHERE, HERO_SACRIFICE, HERO_DODGE,
	BLINDED, PARALIZED, REGENERATION, ARMORED, SPEED, BURNING
}

public static class StatusEffectDescriptor {

    private static Dictionary<StatusEffectType, string> statusColors;

	public static string name (this StatusEffectType type) {
		switch (type) {
            case StatusEffectType.HERO_HEAVY_GUARD: return "Глухая оборона";
            case StatusEffectType.HERO_INVULNERABILITY_SPHERE: return "Сфера неуязвимости";
            case StatusEffectType.HERO_SACRIFICE: return "Жертва";
            case StatusEffectType.HERO_DODGE: return "Увёртливость";
			case StatusEffectType.BLINDED: return "Ослепление";
			case StatusEffectType.PARALIZED: return "Паралич";
			case StatusEffectType.REGENERATION: return "Регенерация";
			case StatusEffectType.ARMORED: return "Защита";
			case StatusEffectType.SPEED: return "Ускорение";
            case StatusEffectType.BURNING: return "Горит";
			default: Debug.Log("Unknown status effect type: " + type); return "";
		}
	}

//	public static bool withoutStatusHolder (this StatusEffectType type) {
//		return type == StatusEffectType.HEAL || type == StatusEffectType.NONE;
//	}

//	public static bool isStatusActiveOnNextTurn (this StatusEffectType type) {
//		return type == StatusEffectType.BLINDED || type == StatusEffectType.PARALIZED;
//	}

    public static string color (this StatusEffectType type) {
        if (statusColors == null) {
            statusColors = new Dictionary<StatusEffectType, string>();
            statusColors.Add(StatusEffectType.HERO_HEAVY_GUARD, "<color=lightblue>");
            statusColors.Add(StatusEffectType.HERO_INVULNERABILITY_SPHERE, "<color=lightblue>");
            statusColors.Add(StatusEffectType.HERO_SACRIFICE, "<color=lightblue>");
            statusColors.Add(StatusEffectType.HERO_DODGE, "<color=lightblue>");
            statusColors.Add(StatusEffectType.ARMORED, "<color=lightblue>");
            statusColors.Add(StatusEffectType.SPEED, "<color=lightblue>");
            statusColors.Add(StatusEffectType.REGENERATION, "<color=#00FF00FF>");
            statusColors.Add(StatusEffectType.BLINDED, "<color=red>");
            statusColors.Add(StatusEffectType.PARALIZED, "<color=red>");
            statusColors.Add(StatusEffectType.BURNING, "<color=red>");
        }
        return statusColors[type];
    }
}