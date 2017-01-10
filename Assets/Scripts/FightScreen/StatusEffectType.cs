using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public enum StatusEffectType {
	BLINDED, PARALIZED, REGENERATION, ARMORED, SPEED
}

public static class StatusEffectDescriptor {

    private static Dictionary<StatusEffectType, string> statusColors;

	public static string name (this StatusEffectType type) {
		switch (type) {
			case StatusEffectType.BLINDED: return "Ослепление";
			case StatusEffectType.PARALIZED: return "Паралич";
			case StatusEffectType.REGENERATION: return "Регенерация";
			case StatusEffectType.ARMORED: return "Защита";
			case StatusEffectType.SPEED: return "Ускорение";
//			case StatusEffectType.HEAL: return "Лечение";
			default: Debug.Log("Unknown status effect type: " + type); return "";
		}
	}

//	public static bool withoutStatusHolder (this StatusEffectType type) {
//		return type == StatusEffectType.HEAL || type == StatusEffectType.NONE;
//	}

	public static bool isStatusActiveOnNextTurn (this StatusEffectType type) {
		return type == StatusEffectType.BLINDED || type == StatusEffectType.PARALIZED;
	}

    public static string color (this StatusEffectType type) {
        if (statusColors == null) {
            statusColors = new Dictionary<StatusEffectType, string>();
            statusColors.Add(StatusEffectType.ARMORED, "<color=lightblue>");
            statusColors.Add(StatusEffectType.SPEED, "<color=lightblue>");
            statusColors.Add(StatusEffectType.REGENERATION, "<color=#00FF00FF>");
            statusColors.Add(StatusEffectType.BLINDED, "<color=red>");
            statusColors.Add(StatusEffectType.PARALIZED, "<color=red>");
        }
        return statusColors[type];
    }
}