using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

public static class Imager {

	private static bool initialized;

	private static Array characterTypes = Enum.GetValues(typeof(CharacterType)),
						 enemyTypes = Enum.GetValues(typeof(EnemyType)),
						 areaTypes = Enum.GetValues(typeof(AreaType));

	private static Dictionary<CharacterType, Texture> portraits = new Dictionary<CharacterType, Texture>();

	private static Dictionary<EnemyType, Sprite[]> enemies = new Dictionary<EnemyType, Sprite[]>();

	private static Dictionary<AreaType, Sprite> areas = new Dictionary<AreaType, Sprite>();

	private static Dictionary<string, Texture> storyBackgrounds = new Dictionary<string, Texture>();

	private static char delimiter = '=';

	private static string[] typeName = new string[2];

	public static void init () {
		if (initialized) { return; }

//		foreach (Sprite sprite in Resources.LoadAll<Sprite>("Sprites")) { addSpriteToList(sprite); }

		foreach (Sprite sprite in Resources.LoadAll<Sprite>("Sprites/Enemy")) { addSpriteToList(sprite); }

//		foreach (Sprite sprite in Resources.LoadAll<Sprite>("Sprites/Area")) { addSpriteToList(sprite); }

		foreach (Texture texture in Resources.LoadAll<Texture>("Textures/Portraits")) { addTextureToList(texture); }

//		foreach (Texture texture in Resources.LoadAll<Texture>("Textures/Story backgrounds")) { addTextureToList(texture); }

		initialized = true;
	}

	public static Texture getPortrait (CharacterType type) { return portraits[type]; }
	public static Texture getStoryBackground (string background) { return storyBackgrounds[background.ToUpper()]; }

	public static Sprite getAreaBackground (AreaType type) { return areas[type]; }
	public static Sprite getEnemy (EnemyType type, float healthLevel) { return enemies[type][(!Vars.EROTIC)? 0: healthLevel <= .3f? 2: healthLevel <= .7f? 1: 0]; }

	private static void addSpriteToList (Sprite sprite) {
		typeName = sprite.name.ToUpper().Split(delimiter);
		switch (typeName[0]) {
			case "AREA":
				foreach (AreaType type in areaTypes) {
					if (type.ToString().Equals(typeName[1])) {
						areas[type] = sprite;
						return;
					}
				}
				break;
			case "ENEMY":
				int index;
				foreach (EnemyType type in enemyTypes) {
					index = typeName.Length == 2? 0: typeName[2].Equals("NUDE")? 2: 1;
					if (type.ToString().Equals(typeName[1])) {
						if (!enemies.ContainsKey(type)) { enemies.Add(type, new Sprite[3]); }
						enemies[type][index] = sprite;
						return;
					}
				}
				Debug.Log("Unmapped enemy: " + typeName[1] + " " + typeName[2]);
				break;
			default: Debug.Log("Unmapped sprite: " + typeName[0] + " - " + typeName[1]); break;
		}
	}

	private static void addTextureToList (Texture texture) {
		typeName = texture.name.ToUpper().Split(delimiter);
		switch (typeName[0]) {
			case "STORY":
				storyBackgrounds.Add(typeName[1], texture);
				break;
			case "PORTRAIT" :
				foreach (CharacterType type in characterTypes) {
					if (type.ToString().Equals(typeName[1])) { 
						portraits.Add(type, texture);
						return;
					}
				}
				Debug.Log("Unmapped portrait: " + typeName[1]);
				break;
			default: Debug.Log("Unmapped texture: " + typeName[0] + " - " + typeName[1]); break;
		}
	}
}