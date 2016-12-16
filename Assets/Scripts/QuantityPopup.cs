using UnityEngine;
using System.Collections;

public class QuantityPopup : MonoBehaviour, ButtonHolder, Closeable {

	private Transform bar;

	private Vector3 barScale = Vector3.one, barPos;

	private float barLeft = -2.78f, fullTrack, barRatio, mouseX, offsetX;

	private BoxCollider2D barCollider;

	private float[] zones;

	private Button decreaseBtn, increaseBtn, applyBtn, denyBtn, denyArea;

	private TextMesh text;

	private bool buying;

    private string startString;

    private PopupListener listener;

//	private LootDisplay display;

	private int count;

	private ItemHolder holder;

	private bool drag;

	public bool onScreen { get; private set; }

    public QuantityPopup init () {
		bar = transform.Find("Bar");
		barPos = bar.transform.localPosition;
		barLeft = bar.localPosition.x;
		fullTrack = -barLeft * 2;

		barCollider = transform.Find("Bar Holder").GetComponent<BoxCollider2D>();
		increaseBtn = transform.Find("Increase Button").GetComponent<Button>().init();
		decreaseBtn = transform.Find("Decrease Button").GetComponent<Button>().init();
		applyBtn = transform.Find("Apply Button").GetComponent<Button>().init();
		denyBtn = transform.Find("Deny Button").GetComponent<Button>().init();
		denyArea = transform.Find("Deny Area").GetComponent<Button>().init();

		text = transform.Find("Text").GetComponent<TextMesh>();
		MeshRenderer mesh = text.GetComponent<MeshRenderer>();
		mesh.sortingLayerName = transform.Find("Background").GetComponent<SpriteRenderer>().sortingLayerName;
		mesh.sortingOrder = 1;

		gameObject.SetActive(false);

        return this;
	}

//	public QuantityPopup init (LootDisplay display) {
//		this.display = display;
//		asLoot = true;
//		init();
//		return this;
//	}

	public void adjustPosition (Vector3 center) {
		offsetX = center.x;
		transform.position = center;
	}

	public void fireClickButton (Button btn) {
		if (btn == applyBtn) { apply(); }
		else if (btn == decreaseBtn) { decrease(); }
		else if (btn == increaseBtn) { increase(); }
		else if (btn == denyBtn || btn == denyArea) { close(false); }
	}

    public void show (PopupListener listener, ItemHolder holder) {
        this.listener = listener;
        this.holder = holder;

        buying = listener is Market && ((Market)listener).type == Inventory.InventoryType.MARKET_BUY;

        count = holder.item.quantity;
        barScale.x = 1f / (float)count;
        bar.localScale = barScale;
        barRatio = fullTrack / (float)count;
        zones = new float[count];
        for (int i = 0; i < count; i++) {
            zones[i] = barLeft + (barRatio * i);
        }
        updateValues();
        InputProcessor.add(this);
        gameObject.SetActive(true);
        onScreen = true;
	}

	void Update () {
		if (Input.GetKeyDown(KeyCode.Return) || Input.GetKeyDown(KeyCode.KeypadEnter)) {
			apply();
		}
//		else if (Input.GetKeyDown(KeyCode.Escape)) {
//			close();
//		}
		if (!drag && Input.GetMouseButtonDown(0) && Utils.hit != null && Utils.hit == barCollider) { drag = true; }
		if (drag) {
			if (Input.GetMouseButtonUp(0)) {
				drag = false;
			} else {
				adjustBarToMouse();
			}
		}
	}

	private void adjustBarToMouse () {
		mouseX = Utils.mousePos.x - offsetX;
		if (mouseX < zones[1]) {
			count = 1;
		} else if (mouseX > zones[zones.Length-1] + barRatio) {
			count = holder.item.quantity;
		} else {
			for (int i = 1; i < holder.item.quantity; i++) {
				if (mouseX > zones[i] && mouseX < zones[i] + barRatio) {
					count = i + 1;
				}
			}
		}
		updateValues();
	}

	private void increase () {
		count++;
		updateValues();
	}

	private void decrease () {
		count--;
		updateValues();
	}

	private void updateValues () {
//		if (asLoot) {
//			text.text = "Забрать <color=blue>" + count + "</color> " + item.itemName;
//		} else {
        text.text = (buying? "Купить <color=blue>": "Продать <color=blue>") + count + "</color> " + holder.item.itemName + " за <color=yellow>" + (count * holder.item.cost) + "$</color>";
//		}
		barPos.x = zones[count-1];
		bar.transform.localPosition = barPos;
		decreaseBtn.setActive(count > 1);
		increaseBtn.setActive(count < holder.item.quantity);
	}

	private void apply () {
        listener.checkPopupResult(holder, count);
//		if (asLoot) {
//			display.applyItemTake(count);
//		} else {
//			if (toBuy) {
//				market.buyItem(item, count);
//			} else {
//				market.sellItem(item, count);
//			}
//		}
		close(false);
	}

	public void close (bool byInputProcessor) {
        holder = null;
		onScreen = false;
//		if (asLoot) { display.applyItemTake(0); }
		gameObject.SetActive(false);
		if (!byInputProcessor) { InputProcessor.removeLast(); }
	}
}