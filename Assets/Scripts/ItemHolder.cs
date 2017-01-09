using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public abstract class ItemHolder : MonoBehaviour, Describeable {
	[HideInInspector]
	public Item item;

	public abstract Item takeItem();

    public List<string> description () {
        return item == null? null: item.description();
    }
}