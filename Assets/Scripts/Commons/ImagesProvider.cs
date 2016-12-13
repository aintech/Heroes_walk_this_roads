using UnityEngine;
using System.Collections;

public class ImagesProvider : MonoBehaviour {

	public Sprite[] weaponSprites, shieldSprites, helmetSprites, armorSprites, gloveSprites, amuletSprites, ringSprites, materialSprites, supplySprites,
					enemyMarkerSprites;

	public static Sprite[] weapons, shields, helmets, armors, gloves, amulets, rings, materials, supplies,
						   enemyMarkers;

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

		enemyMarkers = enemyMarkerSprites;
	}

	public static Sprite getEnemyMarkerSprite (EnemyType type) {
		switch (type) {
			case EnemyType.ROGUE: return enemyMarkers[0];
			default: Debug.Log("Unknown enemy type: " + type); return null;
		}
	}

	public static Sprite getItemSprite (ItemData data) {
		switch (data.itemType) {
			case ItemType.SUPPLY: return getSupplySprite(((SupplyData)data).type);
			case ItemType.WEAPON: return getWeaponSprite(((WeaponData)data).type);
			case ItemType.SHIELD: return getShieldSprite(((ShieldData)data).type);
			case ItemType.HELMET: return getHelmetSprite(((HelmetData)data).type);
			case ItemType.ARMOR: return getArmorSprite(((ArmorData)data).type);
			case ItemType.GLOVE: return getGloveSprite(((GloveData)data).type);
            case ItemType.AMULET: return getAmuletSprite(((AmuletData)data).type);
            case ItemType.RING: return getRingSprite(((RingData)data).type);
			case ItemType.MATERIAL: return getMaterialSprite(((MaterialData)data).type);
			default: Debug.Log("Unknown item type: " + data.itemType); return null;
		}
	}

	public static Sprite getSupplySprite (SupplyType type) {
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

	public static Sprite getMaterialSprite (MaterialType type) {
		switch (type) {
			case MaterialType.WOOD_STICK: return materials[0];
			case MaterialType.STEEL_BAR: return materials[1];
			default: Debug.Log("Unknown material type: " + type); return null;
		}
	}

	public static Sprite getWeaponSprite (WeaponType type) {
		switch (type) {
			case WeaponType.IRON_SWORD: return weapons[0];
			case WeaponType.SQUIRE_SWORD: return weapons[1];
			case WeaponType.NOBLE_SWORD: return weapons[2];
			default: Debug.Log("Unknown weapon type: " + type); return null;
		}
	}

	public static Sprite getShieldSprite (ShieldType type) {
		switch (type) {
			case ShieldType.WOOD: return shields[0];
			case ShieldType.IRON: return shields[1];
			case ShieldType.STEEL: return shields[2];
			default: Debug.Log("Unknown shield type: " + type); return null;
		}
	}

	public static Sprite getHelmetSprite (HelmetType type) {
		switch (type) {
			case HelmetType.LEATHER: return helmets[0];
			case HelmetType.IRON: return helmets[1];
			case HelmetType.STEEL: return helmets[2];
			default: Debug.Log("Unknown helmet type: " + type); return null;
		}
	}

	public static Sprite getArmorSprite (ArmorType type) {
		switch (type) {
			case ArmorType.LEATHER: return armors[0];
			case ArmorType.IRON: return armors[1];
			case ArmorType.STEEL: return armors[2];
			default: Debug.Log("Unknown armor type: " + type); return null;
		}
	}

	public static Sprite getGloveSprite (GloveType type) {
		switch (type) {
			case GloveType.LEATHER: return gloves[0];
			case GloveType.IRON: return gloves[1];
			case GloveType.STEEL: return gloves[2];
			default: Debug.Log("Unknown glove type: " + type); return null;
		}
	}

    public static Sprite getAmuletSprite (AmuletType type) {
		switch (type) {
            case AmuletType.RABBIT_LEG: return amulets[0];
			default: Debug.Log("Unknown amulet type: " + type); return null;
		}
    }

    public static Sprite getRingSprite (RingType type) {
        switch (type) {
            case RingType.COPPER: return rings[0];
            default: Debug.Log("Unknown ring type: " + type); return null;
        }
    }
}