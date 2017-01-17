using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class HeroAction : Describeable {
    
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

    private static Dictionary<ElementType, string> elementColors;

    public HeroAction init (FightInterface fightInterface, Hero hero, HeroActionType actionType, Transform holder, int positionInLine) {
        this.fightInterface = fightInterface;
        this.hero = hero;
        this.actionType = actionType;

        if (elementColors == null) {
            elementColors = new Dictionary<ElementType, string>();
            elementColors.Add(ElementType.AIR, "<color=lightblue>");
            elementColors.Add(ElementType.WATER, "<color=blue>");
            elementColors.Add(ElementType.EARTH, "<color=green>");
            elementColors.Add(ElementType.FIRE, "<color=yellow>");
        }

		targetType = actionType.targetType();

		imageRender = transform.Find("Image").GetComponent<SpriteRenderer>();
		imageRender.sprite = ImagesProvider.getHeroAction(actionType);

        border = transform.Find("Border");

        coll = GetComponent<BoxCollider2D>();

        transform.SetParent(holder);
        transform.localPosition = new Vector3(positionInLine * offset, 0, 0);

		elementsCost = actionType.elementsCost();

        fillDescription();

        setChosen(false);

        return this;
    }

    protected override void fillDescription () {
        descrId = Vars.describeableId++;
        descr.Add(actionType.name());
        descr.Add("Цель: <color=#00FF00FF>" + actionType.targetType().name() + "</color>");
        descr.Add("");
        refillDescription();
        foreach (KeyValuePair<ElementType, int> pair in elementsCost) {
            descr.Add(elementColors[pair.Key] + pair.Key.name() + ":</color> <color=#00FF00FF>" + pair.Value + "</color>");
        }
    }

    public void refillDescription () {
        switch (actionType) {
            case HeroActionType.SWORD_SWING:
            case HeroActionType.MAGIC_ARROW:
            case HeroActionType.STAFF_ATTACK:
            case HeroActionType.DAGGERS_CUT:
                descr[2] = "Урон противнику <color=#00FF00FF>" + hero.damage() + "</color>";
                break;
                
            case HeroActionType.HEAVY_GUARD: descr[2] = "Повышение брони <color=#00FF00FF>+50%</color>"; break;
            case HeroActionType.CRUSHING: descr[2] = "Урон противнику (200%) <color=#00FF00FF>" + (hero.damage() * 2) + "</color>"; break;
                
            case HeroActionType.INVULNERABILITY_SPHERE: descr[2] = "Полностью нейтрализует один удар противника"; break;
            case HeroActionType.FIRE_WALL: descr[2] = "Наносит всем противникам по <color=#00FF00FF>" + (Mathf.RoundToInt((float)hero.damage() * .5f)) + "</color> и поджигает их"; break;
                
            case HeroActionType.SACRIFICE: descr[2] = "Перенапрявляет <color=#00FF00FF>75%</color> урона на союзника с самым большим здоровьем"; break;
            case HeroActionType.HEAL: descr[2] = "Излечивает <color=#00FF00FF>" + (hero.damage() * 2) + "</color> здоровья союзнику"; break;
                
            case HeroActionType.DODGE: descr[2] = "Шанс уворота от атаки противника <color=#00FF00FF>25%</color>"; break;
            case HeroActionType.DUST_IN_EYES: descr[2] = "Накладывает на противника статус <color=#00FF00FF>" + StatusEffectType.BLINDED.name() + "</color> на 3 хода"; break;
            default: Debug.Log("Unknown action type: " + actionType); break;
        }
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