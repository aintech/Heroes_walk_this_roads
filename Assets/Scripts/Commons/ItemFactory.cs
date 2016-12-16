using UnityEngine;
using System;
using System.Collections;

public static class ItemFactory {

	public static Transform itemPrefab;

	public static ItemData createItemData (ItemType type) {
		switch (type) {
			case ItemType.WEAPON: return createWeaponData ();
			case ItemType.SHIELD: return createShieldData();
			case ItemType.HELMET: return createHelmetData();
			case ItemType.ARMOR: return createArmorData ();
			case ItemType.GLOVE: return createGloveData();
            case ItemType.AMULET: return createAmuletData();
            case ItemType.RING: return createRingData();
			case ItemType.MATERIAL: return createMaterialData(UnityEngine.Random.Range(1, 5));
			case ItemType.SUPPLY: return createSupplyData ();
			default: Debug.Log("Unknown type: " + type); return null;
		}
	}

	private static ItemQuality randQuality () {
		float rand = UnityEngine.Random.value;
		return rand <= .5f ? ItemQuality.COMMON : rand <= .7f ? ItemQuality.GOOD : rand <= .85f ? ItemQuality.SUPERIOR : rand <= .95 ? ItemQuality.RARE : ItemQuality.UNIQUE;
	}

	private static float randLevel () {
		return 1 + (UnityEngine.Random.value * .3f);
	}

	private static float qualityMultiplier (ItemQuality quality) {
		return quality == ItemQuality.UNIQUE? 2f: quality == ItemQuality.RARE? 1.7f: quality == ItemQuality.SUPERIOR? 1.4f: quality == ItemQuality.GOOD? 1.2f: 1;
	}

	private static int calculateCost (ItemData data) {
		int cost = 0;
		switch (data.itemType) {
			case ItemType.WEAPON: cost = Mathf.RoundToInt(data.level * ((WeaponData)data).type.cost()); break;
			case ItemType.SHIELD: cost = Mathf.RoundToInt(data.level * ((ShieldData)data).type.cost()); break;
			case ItemType.HELMET: cost = Mathf.RoundToInt(data.level * ((HelmetData)data).type.cost()); break;
			case ItemType.ARMOR: cost = Mathf.RoundToInt(data.level * ((ArmorData)data).type.cost()); break;
			case ItemType.GLOVE: cost = Mathf.RoundToInt(data.level * ((GloveData)data).type.cost()); break;
            case ItemType.AMULET: cost = Mathf.RoundToInt(data.level * ((AmuletData)data).type.cost()); break;
            case ItemType.RING: cost = Mathf.RoundToInt(data.level * ((RingData)data).type.cost()); break;
			case ItemType.SUPPLY: cost = Mathf.RoundToInt(data.level * ((SupplyData)data).type.cost()); break;
			case ItemType.MATERIAL: cost = ((MaterialData)data).type.cost(); break;
			default: Debug.Log("Unknown type: " + data.itemType); break;
		}
		return Mathf.RoundToInt (cost * qualityMultiplier (data.quality));
	}

	public static SupplyData createSupplyData () {
		return createSupplyData ((SupplyType)Enum.GetValues(typeof(SupplyType)).GetValue(UnityEngine.Random.Range(0, Enum.GetNames(typeof(SupplyType)).Length)));
	}

	public static SupplyData createSupplyData (SupplyType type) {
		return createSupplyData(type, randQuality());
	}

	public static SupplyData createSupplyData (SupplyType type, ItemQuality quality) {
		float level = randLevel();
		int value = 0;
		int duration = 0;

		switch (type) {
			case SupplyType.ARMOR_POTION:
				value = Mathf.RoundToInt(type.value() * level * qualityMultiplier(quality)) * 2;
				duration = quality == ItemQuality.UNIQUE? 20: quality == ItemQuality.RARE? 14: quality == ItemQuality.SUPERIOR? 10: quality == ItemQuality.GOOD? 7: 5;
				break;
			case SupplyType.BLINDING_POWDER:
				duration = quality == ItemQuality.UNIQUE? 12: quality == ItemQuality.RARE? 10: quality == ItemQuality.SUPERIOR? 8: quality == ItemQuality.GOOD? 6: 4;
				break;
			case SupplyType.HEALTH_POTION:
				value = Mathf.RoundToInt(type.value() * level * qualityMultiplier(quality));
				break;
			case SupplyType.PARALIZING_DUST:
				duration = quality == ItemQuality.UNIQUE? 7: quality == ItemQuality.RARE? 6: quality == ItemQuality.SUPERIOR? 5: quality == ItemQuality.GOOD? 4: 3;
				break;
            case SupplyType.SPEED_POTION:
                value = quality == ItemQuality.UNIQUE || quality == ItemQuality.RARE? 2: 1;
                duration = quality == ItemQuality.UNIQUE? 15: quality == ItemQuality.RARE? 10: quality == ItemQuality.SUPERIOR? 10: quality == ItemQuality.GOOD? 7: 5;
				break;
			case SupplyType.REGENERATION_POTION:
				value = Mathf.RoundToInt(type.value() * level * qualityMultiplier(quality));
				duration = quality == ItemQuality.UNIQUE? 7: quality == ItemQuality.RARE? 6: quality == ItemQuality.SUPERIOR? 5: quality == ItemQuality.GOOD? 4: 3;
				break;
			default: Debug.Log("Unknown supply type: " + type); break;
		}

		SupplyData data = new SupplyData(quality, level, type, value, duration);
		data.initCommons(calculateCost(data));

		return data;
	}

	public static MaterialData createMaterialData (int quantity) {
		return createMaterialData (quantity, (MaterialType)Enum.GetValues(typeof(MaterialType)).GetValue(UnityEngine.Random.Range(0, Enum.GetNames(typeof(MaterialType)).Length)));
	}

	public static MaterialData createMaterialData (int quantity, MaterialType type) {
		MaterialData data = new MaterialData(type, quantity);
		data.initCommons(type.cost());

		return data;
	}

	public static WeaponData createWeaponData () {
		return createWeaponData ((WeaponType)Enum.GetValues(typeof(WeaponType)).GetValue(UnityEngine.Random.Range(0, Enum.GetNames(typeof(WeaponType)).Length)));
	}

	public static WeaponData createWeaponData (WeaponType type) {
		ItemQuality quality = randQuality();
		float level = randLevel();

		int damage = Mathf.RoundToInt(type.damage() * level * qualityMultiplier(quality));

		WeaponData data = new WeaponData(quality, level, type, damage);
		data.initCommons(calculateCost(data));

		return data;
	}

	public static ArmorData createArmorData () {
		return createArmorData ((ArmorType)Enum.GetValues(typeof(ArmorType)).GetValue(UnityEngine.Random.Range(0, Enum.GetNames(typeof(ArmorType)).Length)));
	}

	public static ArmorData createArmorData (ArmorType type) {
		ItemQuality quality = randQuality();
		float level = randLevel();

		int armorClass = Mathf.RoundToInt(type.armorClass() * level * qualityMultiplier(quality));

		ArmorData data = new ArmorData(quality, level, type, armorClass);
		data.initCommons(calculateCost(data));

		return data;
	}

	public static ShieldData createShieldData () {
		return createShieldData ((ShieldType)Enum.GetValues(typeof(ShieldType)).GetValue(UnityEngine.Random.Range(0, Enum.GetNames(typeof(ShieldType)).Length)));
	}

	public static ShieldData createShieldData (ShieldType type) {
		ItemQuality quality = randQuality();
		float level = randLevel();

		int shieldLevel = Mathf.RoundToInt(type.armorClass() * level * qualityMultiplier(quality));

		ShieldData data = new ShieldData(quality, level, type, shieldLevel);
		data.initCommons(calculateCost(data));

		return data;
	}

	public static HelmetData createHelmetData () {
		return createHelmetData ((HelmetType)Enum.GetValues(typeof(HelmetType)).GetValue(UnityEngine.Random.Range(0, Enum.GetNames(typeof(HelmetType)).Length)));
	}

	public static HelmetData createHelmetData (HelmetType type) {
		ItemQuality quality = randQuality();
		float level = randLevel();

		int armorClass = Mathf.RoundToInt(type.armorClass() * level * qualityMultiplier(quality));

		HelmetData data = new HelmetData(quality, level, type, armorClass);
		data.initCommons(calculateCost(data));

		return data;
	}

	public static GloveData createGloveData () {
		return createGloveData ((GloveType)Enum.GetValues(typeof(GloveType)).GetValue(UnityEngine.Random.Range(0, Enum.GetNames(typeof(GloveType)).Length)));
	}

	public static GloveData createGloveData (GloveType type) {
		ItemQuality quality = randQuality();
		float level = randLevel();

		int armorClass = Mathf.RoundToInt(type.armorClass() * level * qualityMultiplier(quality));

		GloveData data = new GloveData(quality, level, type, armorClass);
		data.initCommons(calculateCost(data));

		return data;
	}

	public static AmuletData createAmuletData () {
        return createAmuletData ((AmuletType)Enum.GetValues(typeof(AmuletType)).GetValue(UnityEngine.Random.Range(0, Enum.GetNames(typeof(AmuletType)).Length)));
	}

	public static AmuletData createAmuletData (AmuletType type) {
		ItemQuality quality = randQuality();
		float level = randLevel();

		AmuletData data = new AmuletData(quality, level, type);
		data.initCommons(calculateCost(data));

		return data;
	}

    public static RingData createRingData () {
        return createRingData ((RingType)Enum.GetValues(typeof(RingType)).GetValue(UnityEngine.Random.Range(0, Enum.GetNames(typeof(RingType)).Length)));
    }

    public static RingData createRingData (RingType type) {
        ItemQuality quality = randQuality();
        float level = randLevel();

        RingData data = new RingData(quality, level, type);
        data.initCommons(calculateCost(data));

        return data;
    }
}