using UnityEngine;
using System.Collections;

public class ShopItemHolder : ItemHolder {

	public ItemType itemType;

	private SpriteRenderer render, shadow;

    public PolygonCollider2D col { get; private set;}

    private bool _hided;
    public bool hided { get { return _hided; } set { _hided = value; gameObject.SetActive(!value);}}

	public void init () {
        item = Instantiate<Transform>(ItemFactory.itemPrefab).GetComponent<Item>();
        item.transform.SetParent(transform);
        item.transform.localPosition = Vector3.zero;
		render = GetComponent<SpriteRenderer>();
		shadow = transform.Find("Shadow").GetComponent<SpriteRenderer>();
	}

	public void initNewItem () {
        item.init(ItemFactory.createItemData(itemType));
		item.transform.SetParent(transform);
        render.sprite = ImagesProvider.getItemSprite(item.itemData);
		shadow.sprite = render.sprite;
		item.gameObject.SetActive(false);
		if (col != null) {
			Destroy(col);
		}
		col = gameObject.AddComponent<PolygonCollider2D>();
        hided = false;
	}

//	public void buyItem () {
//		if (item.getCost() <= Vars.gold) {
//			Vars.gold -= item.getCost();
//			UserInterface.updateGold();
//			item.gameObject.SetActive(true);
//			Vars.gameplay.getInventory().placeItemToFreeCell(item);
//			gameObject.SetActive(false);
//			item = null;
//		}
//	}

    public override Item takeItem () { return null; }

	public void setActive (bool active) {
		if (col != null) {
			col.enabled = active;
		}
	}
}