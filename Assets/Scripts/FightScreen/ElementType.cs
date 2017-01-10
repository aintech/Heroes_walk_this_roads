using UnityEngine;
using System;
using System.Collections;

public enum ElementType {
	FIRE, WATER, EARTH, AIR//, LIGHT, DARK
}
public static class ElementDescriptor {

    private static int elemCount = 0;
    public static int elementsCount {
        get {
            if (elemCount == 0) { elemCount = Enum.GetValues(typeof(ElementType)).Length; }
            return elemCount;
        } 
        private set {;}
    }

    public static string name (this ElementType type) {
        switch (type) {
            case ElementType.AIR: return "Воздух";
            case ElementType.EARTH: return "Земля";
            case ElementType.FIRE: return "Огонь";
            case ElementType.WATER: return "Вода";
            default: Debug.Log("Unknown element type: " + type); return "";
        }
    }
}