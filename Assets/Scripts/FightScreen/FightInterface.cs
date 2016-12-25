﻿using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class FightInterface : MonoBehaviour {

    public Transform heroActionPrefab, queuePortraitPrefab;

    private Transform emptyHolder;

	private FightScreen fightScreen;

	private List<HeroPortrait> portraits = new List<HeroPortrait>();

    private Transform actionsHolder;

    private Dictionary<Hero, Transform> heroActionHolders = new Dictionary<Hero, Transform>();

	private Dictionary<Hero, HeroAction[]> heroActions = new Dictionary<Hero, HeroAction[]>();

	private List<QueuePortrait> queue = new List<QueuePortrait>();

	private List<QueuePortrait> queuePool = new List<QueuePortrait>();

	private Transform queueHolder;

	public FightInterface init (FightScreen fightScreen) {
		this.fightScreen = fightScreen;

		Transform portraitsHolder = transform.Find("Portraits");
		for (int i = 0; i < portraitsHolder.childCount; i++) {
			portraits.Add(portraitsHolder.GetChild(i).GetComponent<HeroPortrait>().init());
		}

        actionsHolder = transform.Find("Actions Holder");
        emptyHolder = actionsHolder.Find("Empty Holder");

		queueHolder = transform.Find("Queue").Find("Queue Holder");

        updateHeroActions();

		gameObject.SetActive(true);

		return this;
	}

    public void updateHeroActions () {
        Transform holder;
        HeroActionType[] actionTypes;
        HeroAction[] actions;
        foreach (Hero hero in Vars.heroes.Values) {
            if (!heroActions.ContainsKey(hero)) {
                holder = Instantiate<Transform>(emptyHolder);
                holder.name = hero.type.ToString() + " Actions Holder";
                holder.SetParent(actionsHolder);
                holder.localPosition = Vector3.zero;

                heroActionHolders.Add(hero, holder);

                actionTypes = hero.type.heroActions();
                actions = new HeroAction[actionTypes.Length];

                for (int i = 0; i < actions.Length; i++) {
                    actions[i] = Instantiate<Transform>(heroActionPrefab).GetComponent<HeroAction>().init(this, hero, actionTypes[i], holder, i);
                }

                heroActions.Add(hero, actions);
            }
        }
    }

	public void prepareQueue (List<Character> characters) {
		int diff = characters.Count - queuePool.Count;
		if (diff > 0) {
			QueuePortrait port;
			for (int i = 0; i < diff; i++) {
				port = Instantiate<Transform>(queuePortraitPrefab).GetComponent<QueuePortrait>().init(queueHolder);
				queuePool.Add(port);
			}
		}
		queue.Clear();
		for (int i = 0; i < characters.Count; i++) {
			queue.Add(queuePool[i].setCharacter(characters[i], i));
		}
	}

    public void setHeroActionsVisible (Hero hero) {
        foreach (KeyValuePair<Hero, Transform> pair in heroActionHolders) {
            pair.Value.gameObject.SetActive(hero != null && pair.Key == hero);
        }
        if(hero != null) {
            heroActions[hero][0].setChosen(true);
        }
    }

    public void updateHeroRepresentatives () {
        foreach (HeroPortrait port in portraits) {
            port.updateRepresentative();
        }
    }

    public void chooseAction (HeroAction action) {
        foreach(HeroAction act in heroActions[action.hero]) {
            if (act != action) { act.setChosen(false); }
        }
        fightScreen.fightProcessor.heroAction = action;
        switch (action.actionType) {
            case HeroActionType.GUARD: chooseTargets(new Character[]{action.hero}); break;
        }
    }

    public void chooseTargets (Character[] targets) {
        fightScreen.fightProcessor.actionTargets = targets;
    }
}