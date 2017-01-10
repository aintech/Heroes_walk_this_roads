using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Utils : MonoBehaviour {
	
	public static Collider2D hit;
	
	public static Vector2 mousePos;
	
	private static Camera cam;
	
	private static Vector2 zeroV = Vector2.zero;

	private static float seed = 0;

	private static List<Item> disposedItems = new List<Item>();

//	public static HashSet<WorkbenchSchemeType> foundSchemes { get; private set; }

	public void init () {
		cam = GetComponent<Camera>();
//		foundSchemes = new HashSet<WorkbenchSchemeType>();
	}
	
	void Update () {
		if (cam == null) { Debug.Log("Camera is null"); cam = GetComponent<Camera>(); }
		mousePos = cam.ScreenToWorldPoint(Input.mousePosition);
		hit = Physics2D.Raycast(mousePos, zeroV, 1).collider;
	}

	public static float getRandomValue (float value, float percent) {
		seed = value * 0.01f * percent;
		return Mathf.Round(Random.Range(value - seed, value + seed) * 10) * 0.1f;
	}

	public static int getRandomValue (int value, int percent) {
		return Mathf.RoundToInt(getRandomValue((float)value, (float)percent));
	}

	public static string breakLines (string value, int charsInLine) {
		if (value.Length <= charsInLine) {
			return value;
		}
		string str = value;
		for (int i = charsInLine; i < str.Length; i += charsInLine) {
			while (!char.IsWhiteSpace(str, i)) {
				i--;
			}
			str = str.Remove(i, 1).Insert(i, "\n");
		}
		return str;
	}

	public static void disposeItem (Item item) {
		disposedItems.Add(item);
		item.gameObject.SetActive(false);
	}

	public static Item getDisposedItem (ItemType itemType) {
		Item returnedItem = null;
		foreach (Item item in disposedItems) {
			if (item.type == itemType) {
				returnedItem = item;
			}
		}
		disposedItems.Remove(returnedItem);
		return returnedItem;
	}

    public static float calcMeshLength (MeshRenderer mesh, float multyplier) {
        return mesh.bounds.size.x * multyplier;
    }
}