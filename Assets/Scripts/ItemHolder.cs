using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public abstract class ItemHolder : Describeable {
	[HideInInspector]
	public Item item;

	public abstract Item takeItem();

    public override long descriptionId() { return item == null? -1: item.descriptionId(); }
    public override List<string> description () { return item == null? null: item.description(); }
    protected override void fillDescription () {}
}