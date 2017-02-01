using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Item : Describeable {

	private SpriteRenderer render;

	public TextMesh quantityText { get; private set; }

	private MeshRenderer quantityRender;

    [HideInInspector]
	public int index;

	[HideInInspector]
    public InventoryCell cell;

	[HideInInspector]
	public Slot slot;
    
	public ItemData itemData { get; private set; }

	public int quantity { get { return itemData.quantity; } set {itemData.quantity = value; updateQuantityText(); } }

	public float volume { get { return itemData.volume; } private set {;} }

	public int cost { get { return itemData.cost; } private set {;} }

	public ItemType type { get { return itemData.itemType; } private set {;} }

	public ItemQuality quality { get { return itemData.quality; } private set {;} }

	public float level { get { return itemData.level; } private set {;} }

	public string itemName { get { return itemData.name; } private set {;} }

	public Item init (ItemData itemData, int index) {
		this.index = index;
		return init(itemData);
	}

	public Item init (ItemData itemData) {
		this.itemData = itemData;
		itemData.item = this;

		render = GetComponent<SpriteRenderer>();

		quantityText = transform.Find("Quantity").GetComponent<TextMesh>();
		quantityRender = quantityText.GetComponent<MeshRenderer>();
		quantityRender.sortingLayerName = "Inventory";
		quantityRender.sortingOrder = 3;

		updateQuantityText();

		switch (itemData.itemType) {
    		case ItemType.WEAPON: render.sprite = ImagesProvider.getWeapon(((WeaponData)itemData).type); break;
    		case ItemType.SHIELD: render.sprite = ImagesProvider.getShield(((ShieldData)itemData).type); break;
    		case ItemType.HELMET: render.sprite = ImagesProvider.getHelmet(((HelmetData)itemData).type); break;
            case ItemType.ARMOR: render.sprite = ImagesProvider.getArmor(((ArmorData)itemData).type); break;  
            case ItemType.GLOVE: render.sprite = ImagesProvider.getGlove(((GloveData)itemData).type); break;
            case ItemType.AMULET: render.sprite = ImagesProvider.getAmulet(((AmuletData)itemData).type); break;
            case ItemType.RING: render.sprite = ImagesProvider.getRing(((RingData)itemData).type); break;
            case ItemType.MATERIAL: render.sprite = ImagesProvider.getMaterial(((MaterialData)itemData).type); break;
			case ItemType.SUPPLY: render.sprite = ImagesProvider.getSupply(((SupplyData)itemData).type); break;
			default: Debug.Log("Unknown item type: " + itemData.itemType); break;
		}

        fillDescription();

		return this;
	}

    override protected void fillDescription () {
        descrId = Vars.describeableId++;

        descr.Add((itemData.quality != ItemQuality.COMMON? itemData.quality.richName() + " ": "") + itemData.name);

        switch (itemData.itemType) {
            case ItemType.WEAPON:
                descr.Add("Урон: <color=orange>" + ((WeaponData)itemData).damage + "</color>");
                break;
            case ItemType.ARMOR:
            case ItemType.SHIELD:
            case ItemType.GLOVE:
            case ItemType.HELMET:
                descr.Add("Защита: <color=orange>" + ((ArmorModifier)itemData).armorClass() + "</color>");
                break;
            case ItemType.AMULET:
            case ItemType.RING:
            case ItemType.MATERIAL:
                break;
            case ItemType.SUPPLY:
                SupplyData sud = (SupplyData)itemData;
                string val = "";
                switch (sud.type) {
                    case SupplyType.HEALTH_POTION: val = "Восстанавливает <color=white>" + sud.value + "</color> HP"; break;
                    case SupplyType.SPEED_POTION: val = "Увеличивает инициативу на <color=white>" + sud.value + "</color>% на <color=white>" + sud.duration + "</color> ходов"; break;
                    case SupplyType.REGENERATION_POTION: val = "Восстановление по <color=white>" + sud.value + "</color> HP в течении <color=white>" + sud.duration + "</color> ходов"; break;
                    case SupplyType.ARMOR_POTION: val = "Повышение защиты на <color=white>" + sud.value + "</color> в течении <color=white>" + sud.duration + "</color> ходов"; break;
                    case SupplyType.STRENGTH_POTION: val = "Увеличивает силу на <color=white>" + sud.value + "</color>% на <color=white>" + sud.duration + "</color> ходов"; break;
//                    case SupplyType.BLINDING_POWDER: val = StatusEffectType.BLINDED.name() + " на <color=white>" + sud.duration + "</color> ходов"; break;
//                    case SupplyType.PARALIZING_DUST: val = StatusEffectType.PARALIZED.name() + " на <color=white>" + sud.duration + "</color> ходов"; break;
                    default: Debug.Log("Unknown supply type: " + sud.type); val = ""; break;
                }
                descr.Add("Эффект: <color=orange>" + val + "</color>");
                break;
        }
    }

	public void changeSortOrder (int newOrder) {
		render.sortingOrder = newOrder;
		quantityRender.sortingOrder = newOrder + 1;
	}

	private void updateQuantityText () {
		quantityText.text = itemData.quantity.ToString();
        quantityText.gameObject.SetActive(itemData.quantity > 1);
	}

	public void returnToParent () {
		if (cell != null) {
			cell.inventory.addItemToCell (this, cell);
		} else if (slot != null) {
			slot.setItem(this);
		} else {
			Debug.Log("Dont know where return item: " + itemName);
		}
	}

	public void dispose () {
		Utils.disposeItem(this);
//		Destroy(gameObject);
	}
}