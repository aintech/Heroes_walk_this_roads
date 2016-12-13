using UnityEngine;
using System.Collections;

public enum CharacterType {
	ALIKA, GUILD_SECRETARY
}

public static class CharacterDescriptor {
	public static string name (this CharacterType type) {
		switch (type) {
			case CharacterType.ALIKA: return "Алика";
			case CharacterType.GUILD_SECRETARY: return "Секретарь";
			default: Debug.Log("Unknown charakter type"); return "";
		}
	}

	public static CharacterType nameToType (string name) {
		switch (name) {
			case "alika": return CharacterType.ALIKA;
			case "guild_secretary": return CharacterType.GUILD_SECRETARY;
			default: Debug.Log("Unknown name: " + name); return CharacterType.ALIKA;
		}
	}
}