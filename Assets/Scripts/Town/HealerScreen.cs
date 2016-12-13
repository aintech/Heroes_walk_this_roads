using UnityEngine;
using System.Collections;

public class HealerScreen : ShopScreen {

    private Button healBtn;

    private int healCost;

    public override TownScreen init (Town town) {
        this.town = town;
        innerInit(Town.ScreenType.HEALER);

        healBtn = transform.Find("Heal Button").GetComponent<Button>().init();

        return this;
    }

    public override void beforeShow () {
        healCost = Player.maxHealth - Player.health;
        updateHealBtn();
    }

    private void heal () {
        Vars.gold -= healCost;
        UserInterface.updateGold();
        Player.setHealthToMax();
        updateHealBtn();
    }

    private void updateHealBtn () {
        healBtn.setActive(healCost > 0 && healCost <= Vars.gold);
        healBtn.setText("Лечение" + (healCost == 0? "": (" (" + healCost.ToString() + ")")));
    }

    public override void closeShop () {
        updateHealBtn();
        setVisible(true);
    }

    public override void fireButton (Button btn) {
        if (btn == healBtn) { heal(); }
        else { Debug.Log("Unknown button: " + btn.name); }
    }
}