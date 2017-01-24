using UnityEngine;
using System.Collections;

public class SupplySlot : Slot {
    
    public int index;

    public override Item takeItem ()
    {
        throw new System.NotImplementedException ();
    }

    public override void setItem (Item newItem)
    {
        throw new System.NotImplementedException ();
    }

    public override void hideItem () {
        if (item != null) {
            Item temp = base.takeItem();
            temp.gameObject.SetActive(false);
        }
    }
}