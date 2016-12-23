using UnityEngine;
using System.Collections;

public class ImagesProvider : MonoBehaviour {

	public Sprite[] weaponSprites, shieldSprites, helmetSprites, armorSprites, gloveSprites, amuletSprites, ringSprites, materialSprites, supplySprites,
					enemyMarkerSprites, locationMarkerSprites,
					heroSprites, heroPortraitSprites, heroQueueSprites,
                    enemySprites, enemyQueueSprtes,
                    heroActionSprites;

	public static Sprite[] weapons, shields, helmets, armors, gloves, amulets, rings, materials, supplies,
						   enemyMarkers, locationMarkers,
						   heroes, heroPortraits, heroQueue,
                           enemies, enemyQueue,
                           heroActions;

	public void init () {
        weapons = weaponSprites;
		shields = shieldSprites;
		helmets = helmetSprites;
		armors = armorSprites;
		gloves = gloveSprites;
        amulets = amuletSprites;
        rings = ringSprites;
		materials = materialSprites;
		supplies = supplySprites;
		heroes = heroSprites;
		heroPortraits = heroPortraitSprites;
        heroQueue = heroQueueSprites;
        enemies = enemySprites;
        enemyQueue = enemyQueueSprtes;
        heroActions = heroActionSprites;

		enemyMarkers = enemyMarkerSprites;
        locationMarkers = locationMarkerSprites;
	}

    public static Sprite getHeroAction (HeroActionType type) {
        switch (type) {
            case HeroActionType.ATTACK: return heroActions[0];
            case HeroActionType.GUARD: return heroActions[1];
            default: Debug.Log("Unknown action type: " + type); return null;
        }
    }

	public static Sprite getHero (HeroType type) {
		switch (type) {
    		case HeroType.ALIKA: return heroes[0];
    		case HeroType.VICTORIA: return heroes[1];
    		case HeroType.LIARA: return heroes [2];
    		case HeroType.KATE: return heroes [3];
    		default: Debug.Log ("Unknown hero type: " + type); return null;
		}
	}

	public static Sprite getHeroPortrait (HeroType type) {
		switch (type) {
			case HeroType.ALIKA: return heroPortraits[0];
			case HeroType.VICTORIA: return heroPortraits[1];
			case HeroType.LIARA: return heroPortraits [2];
			case HeroType.KATE: return heroPortraits [3];
			default: Debug.Log ("Unknown hero type: " + type); return null;
		}
	}

    public static Sprite getHeroQueue (HeroType type) {
        switch (type) {
            case HeroType.ALIKA: return heroQueue[0];
            case HeroType.VICTORIA: return heroQueue[1];
            case HeroType.LIARA: return heroQueue [2];
            case HeroType.KATE: return heroQueue [3];
            default: Debug.Log ("Unknown hero type: " + type); return null;
        }
    }

    public static Sprite getEnemy (EnemyType type) {
        switch (type) {
            case EnemyType.ROGUE: return enemies[0];
            default: Debug.Log ("Unknown enemy type: " + type); return null;
        }
    }

    public static Sprite getEnemyQueue (EnemyType type) {
        switch (type) {
            case EnemyType.ROGUE: return enemyQueue[0];
            default: Debug.Log ("Unknown enemy type: " + type); return null;
        }
    }

	public static Sprite getEnemyMarker (EnemyType type) {
		switch (type) {
			case EnemyType.ROGUE: return enemyMarkers[0];
			default: Debug.Log("Unknown enemy type: " + type); return null;
		}
	}

    public static Sprite getLocationMarker (LocationType type) {
        switch (type) {
            case LocationType.ROUTINE: return locationMarkers[0];
            case LocationType.BANDIT_FORTRESS: return locationMarkers[1];
            default: Debug.Log("Unknown location type: " + type); return null;
        }
    }

	public static Sprite getItem (ItemData data) {
		switch (data.itemType) {
			case ItemType.SUPPLY: return getSupply(((SupplyData)data).type);
			case ItemType.WEAPON: return getWeapon(((WeaponData)data).type);
			case ItemType.SHIELD: return getShield(((ShieldData)data).type);
			case ItemType.HELMET: return getHelmet(((HelmetData)data).type);
			case ItemType.ARMOR: return getArmor(((ArmorData)data).type);
			case ItemType.GLOVE: return getGlove(((GloveData)data).type);
            case ItemType.AMULET: return getAmulet(((AmuletData)data).type);
            case ItemType.RING: return getRing(((RingData)data).type);
			case ItemType.MATERIAL: return getMaterial(((MaterialData)data).type);
			default: Debug.Log("Unknown item type: " + data.itemType); return null;
		}
	}

	public static Sprite getSupply (SupplyType type) {
		switch (type) {
			case SupplyType.HEALTH_POTION: return supplies[0];
			case SupplyType.ARMOR_POTION: return supplies[1];
			case SupplyType.REGENERATION_POTION: return supplies[2];
			case SupplyType.SPEED_POTION: return supplies[3];
			case SupplyType.BLINDING_POWDER: return supplies[4];
			case SupplyType.PARALIZING_DUST: return supplies[5];
			default: Debug.Log("Unknown supply type: " + type); return null;
		}
	}

	public static Sprite getMaterial (MaterialType type) {
		switch (type) {
			case MaterialType.WOOD_STICK: return materials[0];
			case MaterialType.STEEL_BAR: return materials[1];
			default: Debug.Log("Unknown material type: " + type); return null;
		}
	}

	public static Sprite getWeapon (WeaponType type) {
		switch (type) {
			case WeaponType.IRON_SWORD: return weapons[0];
			case WeaponType.SQUIRE_SWORD: return weapons[1];
			case WeaponType.NOBLE_SWORD: return weapons[2];
			default: Debug.Log("Unknown weapon type: " + type); return null;
		}
	}

	public static Sprite getShield (ShieldType type) {
		switch (type) {
			case ShieldType.WOOD: return shields[0];
			case ShieldType.IRON: return shields[1];
			case ShieldType.STEEL: return shields[2];
			default: Debug.Log("Unknown shield type: " + type); return null;
		}
	}

	public static Sprite getHelmet (HelmetType type) {
		switch (type) {
			case HelmetType.LEATHER: return helmets[0];
			case HelmetType.IRON: return helmets[1];
			case HelmetType.STEEL: return helmets[2];
			default: Debug.Log("Unknown helmet type: " + type); return null;
		}
	}

	public static Sprite getArmor (ArmorType type) {
		switch (type) {
			case ArmorType.LEATHER: return armors[0];
			case ArmorType.IRON: return armors[1];
			case ArmorType.STEEL: return armors[2];
			default: Debug.Log("Unknown armor type: " + type); return null;
		}
	}

	public static Sprite getGlove (GloveType type) {
		switch (type) {
			case GloveType.LEATHER: return gloves[0];
			case GloveType.IRON: return gloves[1];
			case GloveType.STEEL: return gloves[2];
			default: Debug.Log("Unknown glove type: " + type); return null;
		}
	}

    public static Sprite getAmulet (AmuletType type) {
		switch (type) {
            case AmuletType.RABBIT_LEG: return amulets[0];
			default: Debug.Log("Unknown amulet type: " + type); return null;
		}
    }

    public static Sprite getRing (RingType type) {
        switch (type) {
            case RingType.COPPER: return rings[0];
            default: Debug.Log("Unknown ring type: " + type); return null;
        }
    }
}