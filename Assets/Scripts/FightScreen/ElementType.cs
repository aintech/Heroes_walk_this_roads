using UnityEngine;
using System.Collections;

public enum ElementType {
	FIRE, WATER, EARTH, AIR, LIGHT, DARK
}
public static class ElementDescriptor {
	public static int getElementsCount () {
		return 6;
	}
}