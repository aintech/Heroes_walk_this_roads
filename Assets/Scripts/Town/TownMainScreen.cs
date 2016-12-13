using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;
using System.Collections.Generic;

public class TownMainScreen : MonoBehaviour, ButtonHolder, Hideable {
    
	private Background background;

    private Button healerBtn, marketBtn, homeBtn, leaveBtn;

    private Town town;

    public TownMainScreen init (Town town) {
        this.town = town;

		background = transform.Find("Background").GetComponent<Background>().init();

        healerBtn = transform.Find("Healer Button").GetComponent<Button>().init();
        marketBtn = transform.Find("Market Button").GetComponent<Button>().init();
        homeBtn = transform.Find("Home Button").GetComponent<Button>().init();
		leaveBtn = transform.Find("Leave Button").GetComponent<Button>().init();

        Gameplay.topHideable = this;

        return this;
	}

    public void showScreen () {
        gameObject.SetActive(true);
        Gameplay.topHideable = this;
    }

    public void closeScreen () {
        gameObject.SetActive(false);
    }

	public void fireClickButton (Button btn) {
        if (btn == leaveBtn) { town.leaveTown(); }
        else {
            town.showScreen (btn == healerBtn? Town.ScreenType.HEALER:
                            btn == marketBtn? Town.ScreenType.MARKET:
                            btn == homeBtn? Town.ScreenType.HOME: Town.ScreenType.MAIN);
            closeScreen();
        }
	}



	private void sendToVars () {
//		statusScreen.sendToVars();
//		if (Vars.planetType.isColonized()) { market.buyMarket.sendToVars(); }
	}
	
	private void initFromVars () {
//		statusScreen.initFromVars();
//		if (Vars.planetType.isColonized()) { market.buyMarket.initFromVars(); }
	}

	public void setVisible (bool visible) {
        healerBtn.setVisible(visible);
        marketBtn.setVisible(visible);
        homeBtn.setVisible(visible);
        leaveBtn.setVisible(visible);
        if (visible) { Gameplay.topHideable = this; }
	}
}