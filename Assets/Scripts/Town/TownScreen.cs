using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public abstract class TownScreen : MonoBehaviour, ButtonHolder, Closeable, Hideable {

    public Town.ScreenType screenType { get; private set; }

    protected Button closeBtn;

    protected Town town;

    public abstract TownScreen init (Town town);

    private List<Button> buttons = new List<Button>();

    protected void innerInit (Town.ScreenType screenType) {
        this.screenType = screenType;
        closeBtn = transform.Find("Close Button").GetComponent<Button>().init();

        Button btn;
        for (int i = 0; i < transform.childCount; i++) {
            btn = transform.GetChild(i).GetComponent<Button>();
            if (btn != null) { buttons.Add(btn); }
        }

        gameObject.SetActive(false);
    }

    public void show () {
        beforeShow();
        InputProcessor.add(this);
        Gameplay.topHideable = this;
        gameObject.SetActive(true);
    }

    public void close (bool byInputProcessor) {
        beforeClose();
        gameObject.SetActive(false);
        if (!byInputProcessor) { InputProcessor.removeLast(); }
        town.showScreen(Town.ScreenType.MAIN);
    }

    public virtual void beforeShow () {}
    public virtual void beforeClose () {}

    public void fireClickButton (Button btn) {
        if (btn == closeBtn) { close(false); }
        else { fireButton(btn); }
    }

    public void setVisible (bool visible) {
        foreach(Button btn in buttons) { btn.setVisible(visible); }
    }

    public abstract void fireButton (Button btn);
}