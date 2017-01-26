using UnityEngine;
using System.Collections;

public class StatusEffectHolder : MonoBehaviour {

    public StatusEffectType type;

    private SpriteRenderer render;

//    private Color32 disabledColor = new Color32(255, 255, 255, 120), enabledColor = new Color32(255, 255, 255, 255);

    private StatusEffect statusEffect;

    public StatusEffectHolder init () {
        render = GetComponent<SpriteRenderer>();
        gameObject.SetActive(false);
        return this;
    }

    public void setStatusEffect (StatusEffect statusEffect) {
        this.statusEffect = statusEffect;
        statusEffect.holder = this;
        if (statusEffect.inProgress) {
            show();
//            setAsEnabled();
        }
    }

    public void setRenderProperties (string layerName, int sortingOrder) {
        render.sortingOrder = sortingOrder;
        setRenderLayer(layerName);
    }

    public void setRenderLayer (string layerName) {
        render.sortingLayerName = layerName;
    }

    public void show () {
//        render.color = disabledColor;
        gameObject.SetActive(true);
    }
//
//    public void setAsEnabled () {
//        render.color = enabledColor;
//    }

    public void hide () {
        gameObject.SetActive(false);
    }
}