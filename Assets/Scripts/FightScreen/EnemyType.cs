using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public enum EnemyType {
	ROGUE
}

public static class EnemyDescriptor {
    
	public static string name (this EnemyType type) {
		switch (type) {
			case EnemyType.ROGUE: return "Разбойник";
			default: Debug.Log("Unknown enemy type: " + type); return "";
		}
	}

	public static int health (this EnemyType type) {
		return 100;
	}

	public static int damage (this EnemyType type) {
		return 20;
	}

	public static int initiative (this EnemyType type) {
		return 10;
	}

	public static int armor (this EnemyType type) {
		switch (type) {
			case EnemyType.ROGUE: return ArmorType.LEATHER.armorClass();
			default: Debug.Log("Unknown enemy type: " + type); return 0;
		}
	}

	public static int drop (this EnemyType type) {
		switch (type) {
			case EnemyType.ROGUE: return 20;
			default: Debug.Log("Unknown enemy type: " + type); return 0;
		}
	}
}