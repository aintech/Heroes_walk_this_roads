using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

public static class Vars {

    public static bool EROTIC = false;

    public static int gold;

	public static int freeSortingOrder = 0;

	public static Story.Chapter chapter = Story.Chapter.NONE;

	public static int itemTypeCharsInLine = 20;

	public static Dictionary<HeroType, Hero> heroes = new Dictionary<HeroType, Hero> ();

    public static int describeableId = 0;
}