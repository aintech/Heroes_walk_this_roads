﻿using UnityEngine;
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
            case HeroActionType.ATTACK: descr[2] = "Урон противнику: <color=#00FF00FF>" + hero.damage() + "</color>"; break;
            case HeroActionType.GUARD: descr[2] = "Повышение защиты: <color=#00FF00FF>" + HeroActionType.GUARD.value() + "%</color>"; break;
            case HeroActionType.HEAL: descr[2] = "Лечение: <color=#00FF00FF>" + HeroActionType.HEAL.value() + "</color>"; break;
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