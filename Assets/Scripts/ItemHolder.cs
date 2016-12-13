using UnityEngine;
using System.Collections;

public abstract class ItemHolder : MonoBehaviour {
	[HideInInspector]
	public Item item;

	public abstract Item takeItem();
}