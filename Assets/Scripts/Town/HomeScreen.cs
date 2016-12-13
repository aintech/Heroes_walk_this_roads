using UnityEngine;
using System.Collections;

public class HomeScreen : TownScreen {

    private Button restBtn;

    public override TownScreen init (Town town) {
        this.town = town;
        innerInit(Town.ScreenType.HOME);

        restBtn = transform.Find("Rest Button").GetComponent<Button>().init();

        return this;
    }

    public void rest () {
        
    }

    public override void fireButton (Button btn) {
        if (btn == restBtn) { rest(); }
        else { Debug.Log("Unknown button: " + btn.name); }
    }
}