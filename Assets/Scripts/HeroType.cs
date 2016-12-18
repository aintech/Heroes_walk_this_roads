using UnityEngine;
using System.Collections;

public enum HeroType {
	ALIKA, VICTORIA, LIARA, KATE//, ROKSANA, MARIKA
}

public static class HeroDescriptor {
	public static string name (this HeroType type) {
		switch (type) {
			case HeroType.ALIKA: return "Алика";
			case HeroType.VICTORIA: return "Викотрия";
			case HeroType.LIARA: return "Лиара";
			case HeroType.KATE: return "Кейт";
			default: Debug.Log("Unknown charakter type"); return "";
		}
	}

	public static HeroType nameToType (string name) {
		switch (name) {
			case "alika": return HeroType.ALIKA;
			case "victoria": return HeroType.VICTORIA;
			case "liara": return HeroType.LIARA;
			case "kate": return HeroType.KATE;
			default: Debug.Log("Unknown name: " + name); return HeroType.ALIKA;
		}
	}

	public static int strenght (this HeroType type) {
		return 10;
	}

	public static int endurance (this HeroType type) {
		return 10;
	}

	public static int agility (this HeroType type) {
		return 10;
	}
}