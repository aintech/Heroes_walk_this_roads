using UnityEngine;
using System.Collections;

public abstract class ItemData {
	[HideInInspector]
	public Item item;
	public ItemType itemType { get; protected set; }
	public string name { get; protected set; }
	public string description { get; protected set; }
	public float volume { get; protected set; }
	public bool isBuffable { get; private set; }
	public int quantity = 1;

	public ItemQuality quality { get; private set; }
	public float level { get; private set; }

	public int cost { get; private set; }

	public int sortWeight { get; protected set; }

	protected ItemData (ItemQuality quality, float level) {
		this.quality = quality;
		this.level = level;
	}

	public void initCommons (int cost) {
		this.cost = cost;
		isBuffable = itemType == ItemType.WEAPON || itemType == ItemType.ARMOR || itemType == ItemType.SHIELD;
	}
}

public interface ArmorModifier {
    int armorClass ();
}

public class SupplyData : ItemData {
	public SupplyType type { get; private set; }
	public int value { get; private set; }
	public int duration { get; private set; }

	public SupplyData (ItemQuality quality, float level, SupplyType type, int value, int duration) : base (quality, level) {
		this.type = type;
		this.value = value;
		this.duration = duration;

		name = type.name();
		description = type.description();
		volume = type.volume();
		sortWeight = (int)type * 10000;
		itemType = ItemType.SUPPLY;
	}
}

public class WeaponData : ItemData {
	public WeaponType type { get; private set; }
	public int damage { get; private set; }

	public WeaponData (ItemQuality quality, float level, WeaponType type, int damage) : base(quality, level) {
		this.type = type;
		this.damage = damage;

		name = type.name();
		description = type.description();
		volume = type.volume();
		sortWeight = (int)type * 10000;
		itemType = ItemType.WEAPON;
	}
}

public class ShieldData : ItemData, ArmorModifier {
	public ShieldType type { get; private set; }
    private int armor;
    public int armorClass () { return armor; }

    public ShieldData (ItemQuality quality, float level, ShieldType type, int armor) : base(quality, level) {
		this.type = type;
        this.armor = armor;

		name = type.name();
		description = type.description();
		volume = type.volume();
		sortWeight = (int)type * 10000;
		itemType = ItemType.SHIELD;
	}
}

public class HelmetData : ItemData, ArmorModifier {
    public HelmetType type { get; private set; }
    private int armor;
    public int armorClass () { return armor; }

	public HelmetData (ItemQuality quality, float level, HelmetType type, int armor) : base(quality, level) {
		this.type = type;
		this.armor = armor;

		name = type.name();
		description = type.description();
		volume = type.volume();
		sortWeight = (int)type * 10000;
		itemType = ItemType.HELMET;
	}
}

public class ArmorData : ItemData, ArmorModifier {
    public ArmorType type { get; private set; }
    private int armor;
    public int armorClass () { return armor; }

	public ArmorData (ItemQuality quality, float level, ArmorType type, int armor) : base(quality, level) {
		this.type = type;
        this.armor = armor;

		name = type.name();
		description = type.description();
		volume = type.volume();
		sortWeight = (int)type * 10000;
		itemType = ItemType.ARMOR;
	}
}

public class GloveData : ItemData, ArmorModifier {
    public GloveType type { get; private set; }
    private int armor;
    public int armorClass () { return armor; }

	public GloveData (ItemQuality quality, float level, GloveType type, int armor) : base(quality, level) {
		this.type = type;
        this.armor = armor;

		name = type.name();
		description = type.description();
		volume = type.volume();
		sortWeight = (int)type * 10000;
		itemType = ItemType.GLOVE;
	}
}

public class AmuletData : ItemData {
    public AmuletType type { get; private set; }

	public AmuletData (ItemQuality quality, float level, AmuletType type) : base(quality, level) {
		this.type = type;

		name = type.name();
		description = type.description();
		volume = type.volume();
		sortWeight = (int)type * 10000;
        itemType = ItemType.AMULET;
	}
}

public class RingData : ItemData {
    public RingType type { get; private set; }

    public RingData (ItemQuality quality, float level, RingType type) : base(quality, level) {
        this.type = type;

        name = type.name();
        description = type.description();
        volume = type.volume();
        sortWeight = (int)type * 10000;
        itemType = ItemType.RING;
    }
}

public class MaterialData : ItemData {
	public MaterialType type { get; private set; }

	public MaterialData (MaterialType type, int quantity) : base(ItemQuality.COMMON, 1) {
		this.type = type;
		this.quantity = quantity;

		name = type.name();
		description = type.description();
		volume = type.volume();
		sortWeight = (int)type * 10000;
		itemType = ItemType.MATERIAL;
	}
}

public class DataCopier {
	public static ItemData copy (ItemData source) {
		ItemData copy = null;
		switch (source.itemType) {
			case ItemType.SUPPLY:
				SupplyData sud = (SupplyData)source;
				copy = new SupplyData(sud.quality, sud.level, sud.type, sud.value, sud.duration);
				break;
			case ItemType.MATERIAL:
				MaterialData md = (MaterialData)source;
				copy = new MaterialData(md.type, md.quantity);
				break;
			case ItemType.WEAPON:
				WeaponData wd = (WeaponData)source;
                copy = new WeaponData(source.quality, source.level, wd.type, wd.damage);
				break;
			case ItemType.SHIELD:
				ShieldData sd = (ShieldData)source;
                copy = new ShieldData(source.quality, source.level, sd.type, sd.armorClass());
				break;
			case ItemType.HELMET:
				HelmetData hd = (HelmetData)source;
                copy = new HelmetData(source.quality, source.level, hd.type, hd.armorClass());
				break;
			case ItemType.ARMOR:
				ArmorData ad = (ArmorData)source;
                copy = new ArmorData(source.quality, source.level, ad.type, ad.armorClass());
				break;
			case ItemType.GLOVE:
				GloveData gd = (GloveData)source;
                copy = new GloveData(source.quality, source.level, gd.type, gd.armorClass());
                break;
            case ItemType.AMULET:
                AmuletData amd = (AmuletData)source;
                copy = new AmuletData(source.quality, source.level, amd.type);
                break;
            case ItemType.RING:
				RingData rd = (RingData)source;
                copy = new RingData(source.quality, source.level, rd.type);
				break;
			default:
				Debug.Log("Unknown item type to copy from: " + source.itemType); break;
		}

		copy.initCommons(source.cost);

		return copy;
	}
}