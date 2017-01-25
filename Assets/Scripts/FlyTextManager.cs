using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class FlyTextManager : MonoBehaviour {

    public Transform flyTextPrefab;

    private List<FlyText> texts = new List<FlyText>();

    private FlyText flyText;

    public FlyTextManager init () {
        return this;
    }

    public void fireText (string message, Color32 color, Vector2 pos) {
        flyText = null;
        foreach (FlyText text in texts) {
            if (!text.fired) { flyText = text; break; }
        }
        if (flyText == null) {
            flyText = Instantiate<Transform>(flyTextPrefab).GetComponent<FlyText>().init();
            flyText.transform.SetParent(transform);
            texts.Add(flyText);
        }
        flyText.fire(message, color, pos);
    }
}