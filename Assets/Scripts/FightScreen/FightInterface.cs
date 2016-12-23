using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class FightInterface : MonoBehaviour {

    public Transform heroActionPrefab;

    private Transform emptyHolder;

	private FightScreen fightScreen;

	private List<HeroPortrait> portraits = new List<HeroPortrait>();

    private Transform actionsHolder;

    private Dictionary<Hero, Transform> heroActionHolders = new Dictionary<Hero, Transform>();

    private Dictionary<Hero, HeroAction[]> heroActions = new Dictionary<Hero, HeroAction[]>();

	public FightInterface init (FightScreen fightScreen) {
		this.fightScreen = fightScreen;

		Transform portraitsHolder = transform.Find("Portraits");
		for (int i = 0; i < portraitsHolder.childCount; i++) {
			portraits.Add(portraitsHolder.GetChild(i).GetComponent<HeroPortrait>().init());
		}

        actionsHolder = transform.Find("Actions Holder");
        emptyHolder = actionsHolder.Find("Empty Holder");

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

    public void setHeroActionsVisible (Hero hero) {
        foreach (KeyValuePair<Hero, Transform> pair in heroActionHolders) {
            pair.Value.gameObject.SetActive(hero != null && pair.Key == hero);
        }
//        if(hero != null) {
//            heroActions[hero][0].setChosen(true);
//        }
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