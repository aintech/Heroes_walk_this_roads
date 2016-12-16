using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Town : MonoBehaviour {

    private TownMainScreen mainScreen;

    private Dictionary<ScreenType, TownScreen> screens = new Dictionary<ScreenType, TownScreen>();

    private World world;

    public Town init (World world) {
        this.world = world;
        world.town = this;

        mainScreen = transform.Find("Main").GetComponent<TownMainScreen>().init(this);

        TownScreen screen;
        for (int i = 0; i < transform.childCount; i++) {
            screen = transform.GetChild(i).GetComponent<TownScreen>();
            if (screen != null) {
                screen.init(this);
                screens.Add(screen.screenType, screen);
            }
        }

        return this;
    }

    public void walkInTown (LocationType type) {
        if (checkStoryline()) {
            Vars.gameplay.story.playNextChapter();
        } else {
            showScreen(ScreenType.MAIN);
            gameObject.SetActive(true);
        }
    }

    public void leaveTown () {
        world.showWorld(LocationType.ROUTINE);
        gameObject.SetActive(false);
    }

    private bool checkStoryline () {
        //      if (Vars.chapter == Story.Chapter.NONE) { return true; }
        return false;
    }

    public void showScreen (ScreenType type) {
        if (type == ScreenType.MAIN) { mainScreen.showScreen(); }
        else { screens[type].show(); }
    }

    public enum ScreenType {
        MAIN, MARKET, HEALER, HOME
    }
}