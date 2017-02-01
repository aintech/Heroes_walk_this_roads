using UnityEngine;
using System.Collections;

public class SubMenuButton : DescriptionLine {

    public BoxCollider2D coll { get; private set; }

    public int index { get; private set; }

    public SubMenuButton init (Transform holder, float pos) {
        base.init (holder, pos);
        return this;
    }

    public void show (string text, int index) {
        this.index = index;
        float val = setText(text);
        if (coll != null) {
            Destroy(GetComponent<BoxCollider2D>());
        }
        coll = gameObject.AddComponent<BoxCollider2D>();
    }
}