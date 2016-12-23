using UnityEngine;
using System.Collections;

public class HeroAction : MonoBehaviour {
    
    public HeroActionType actionType { get; private set; }

    private BoxCollider2D coll;

    private Transform border;

    private float offset = 1.35f;

    private FightInterface fightInterface;

    public Hero hero { get; private set; }

    public HeroAction init (FightInterface fightInterface, Hero hero, HeroActionType actionType, Transform holder, int positionInLine) {
        this.fightInterface = fightInterface;
        this.hero = hero;
        this.actionType = actionType;

        transform.Find("Image").GetComponent<SpriteRenderer>().sprite = ImagesProvider.getHeroAction(actionType);

        border = transform.Find("Border");

        coll = transform.Find("Image").GetComponent<BoxCollider2D>();

        transform.SetParent(holder);
        transform.localPosition = new Vector3(positionInLine * offset, 0, 0);

        setChosen(false);

        return this;
    }

    void Update () {
        if (Input.GetMouseButtonDown(0) && Utils.hit != null && Utils.hit == coll) {
            setChosen(true);
        }
    }

    public void setChosen (bool chosen) {
        border.gameObject.SetActive(chosen);
        if (chosen) {
            fightInterface.chooseAction(this);
        }
    }
}