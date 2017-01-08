using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class HeroAction : MonoBehaviour {
    
    public HeroActionType actionType { get; private set; }

    private BoxCollider2D coll;

    private Transform border;

    private float offset = 1.35f;

    private FightInterface fightInterface;

    public Hero hero { get; private set; }

	public TargetType targetType { get; private set; }

	public Dictionary<ElementType, int> elementsCost { get; private set;}

	private bool haveEnouthElements;

	private SpriteRenderer imageRender;

	private Color32 normalColor = new Color32(255, 255, 255, 255), transparentColor = new Color32(255, 255, 255, 100);

    public HeroAction init (FightInterface fightInterface, Hero hero, HeroActionType actionType, Transform holder, int positionInLine) {
        this.fightInterface = fightInterface;
        this.hero = hero;
        this.actionType = actionType;

		targetType = actionType.targetType();

		imageRender = transform.Find("Image").GetComponent<SpriteRenderer>();
		imageRender.sprite = ImagesProvider.getHeroAction(actionType);

        border = transform.Find("Border");

        coll = transform.Find("Image").GetComponent<BoxCollider2D>();

        transform.SetParent(holder);
        transform.localPosition = new Vector3(positionInLine * offset, 0, 0);

		elementsCost = actionType.elementsCost();

        setChosen(false);

        return this;
    }

    void Update () {
		if (Input.GetMouseButtonDown(0) && haveEnouthElements && Utils.hit != null && Utils.hit == coll) {
			setChosen(true);
        }
    }

	public void checkElementsIsEnouth () {
		haveEnouthElements = true;
		foreach (KeyValuePair<ElementType, int> pair in elementsCost) {
			if (ElementsPool.instance.elements[pair.Key] < pair.Value) {
				haveEnouthElements = false;
				break;
			}
		}
		imageRender.color = haveEnouthElements? normalColor: transparentColor;
		enabled = haveEnouthElements;
	}

    public void setChosen (bool chosen) {
        border.gameObject.SetActive(chosen);
        if (chosen) {
            fightInterface.chooseAction(this);
        }
    }
}