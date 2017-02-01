using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class SubMenu : MonoBehaviour {

    public static SubMenu instance { get; private set; }

    public Transform subMenuButtonPrefab;

    private List<SubMenuButton> menuButtons = new List<SubMenuButton>();

    private Transform buttonsHolder;

    private SubMenuCaller caller;

    public void init () {
        
    }

    public void show (SubMenuCaller caller, string[] values) {
        this.caller = caller;
        if (menuButtons.Count < values.Length) {
            menuButtons.Add(Instantiate<Transform>(subMenuButtonPrefab).GetComponent<SubMenuButton>().init(buttonsHolder, 0));
        }
        for (int i = 0; i < values.Length; i++) {
            menuButtons[i].show(values[i], i);
        }
        buttonsHolder.gameObject.SetActive(true);
    }

    void Update () {
        if (Input.GetMouseButtonDown(0)) {
            if (Utils.hit != null) {
                foreach (SubMenuButton btn in menuButtons) {
                    if (Utils.hit == btn.coll) {
                        
                    }
                }
            }
            hide();
        }
    }

    public void hide () {
        foreach (SubMenuButton btn in menuButtons) {
            btn.gameObject.SetActive(false);
        }
        buttonsHolder.gameObject.SetActive(false);
    }
}