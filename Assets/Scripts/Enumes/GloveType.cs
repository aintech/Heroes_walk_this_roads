using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public enum GloveType {
	LEATHER, IRON, STEEL
}

public static class GloveDescriptor {

	private static Dictionary<GloveType, string> descript;

	public static string name (this GloveType type) {
		switch (type) {
			case GloveType.LEATHER: return "Кожаные перчатки";
			case GloveType.IRON: return "Железные перчатки";
			case GloveType.STEEL: return "Стальные перчатки";
			default: Debug.Log("Неизвестный тип перчаток"); return "";
		}
	}

	public static string description (this GloveType type) {
		if (descript == null) {
			descript = new Dictionary<GloveType, string>();
			descript.Add(GloveType.LEATHER, Utils.breakLines("Перчатки из кожи неплохо защищают руки от царапин и заноз.", Vars.itemTypeCharsInLine));
			descript.Add(GloveType.IRON, Utils.breakLines("Прикрытые железными пластинками перчатки помогут сохранить кисть целой, если по ней придется удар.", Vars.itemTypeCharsInLine));
			descript.Add(GloveType.STEEL, Utils.breakLines("Стальные пластины прикрывающие перчатки, помогут справиться с такой неприятностью, как отрубленные кисти.", Vars.itemTypeCharsInLine));
		}
		return descript[type];
	}

	public static int armorClass (this GloveType type) {
		switch (type) {
			case GloveType.LEATHER: return 5;
			case GloveType.IRON: return 10;
			case GloveType.STEEL: return 20;
			default: Debug.Log("Неизвестный тип перчаток"); return 0;
		}
	}
	
	public static int cost (this GloveType type) {
		switch (type) {
			case GloveType.LEATHER: return 100;
			case GloveType.IRON: return 200;
			case GloveType.STEEL: return 300;
			default: Debug.Log("Неизвестный тип перчаток"); return 0;
		}
	}
	
    public static float volume (this GloveType type) {
        return 0;
    }

	public static bool isArtefact (this GloveType type) {
		return false;
	}
}