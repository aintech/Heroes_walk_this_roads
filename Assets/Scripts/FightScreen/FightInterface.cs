using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class FightInterface : MonoBehaviour {

	public static FightInterface instance { get; private set; }

    public Transform heroActionPrefab, queuePortraitPrefab;

    private Transform emptyHolder;

    public List<HeroRepresentative> portraits { get; private set; }

    private Transform actionsHolder;

    private Dictionary<Hero, Transform> heroActionHolders = new Dictionary<Hero, Transform>();

	private Dictionary<Hero, HeroAction[]> heroActions = new Dictionary<Hero, HeroAction[]>();

	private List<QueuePortrait> queue = new List<QueuePortrait>();

	private List<QueuePortrait> queuePool = new List<QueuePortrait>();

	private Transform queueHolder;

	public FightInterface init () {
		instance = this;

		Transform portraitsHolder = transform.Find("Portraits");
        portraits = new List<HeroRepresentative>();
		for (int i = 0; i < portraitsHolder.childCount; i++) {
			portraits.Add(portraitsHolder.GetChild(i).GetComponent<HeroRepresentative>().init());
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

	public void updateQueue (List<Character> characters) {
        foreach (QueuePortrait qp in queuePool) {
            qp.gameObject.SetActive(false);
        }
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
			foreach(HeroAction act in heroActions[hero]) {
				act.checkElementsIsEnouth();
                act.refillDescription();
			}
            heroActions[hero][0].setChosen(true);
        }
    }

    public void updateHeroRepresentatives () {
        foreach (HeroRepresentative port in portraits) {
            port.updateRepresentative();
            port.onHealModified();
        }
    }

    public void chooseAction (HeroAction action) {
        foreach(HeroAction act in heroActions[action.hero]) {
            if (act != action) { act.setChosen(false); }
        }
		FightProcessor.instance.heroAction = action;
		switch (action.targetType) {
			case TargetType.SELF: chooseTargets(new Character[]{action.hero}); break;
            case TargetType.ALLIES: 
                List<Character> allies = new List<Character>();
                foreach (Hero hero in Vars.heroes.Values) {
                    if (hero.alive) { allies.Add(hero); }
                }
                chooseTargets(allies.ToArray());
                break;
            case TargetType.ENEMIES:
                List<Character> enemies = new List<Character>();
                foreach (EnemyRepresentative enemy in FightProcessor.instance.enemies) {
                    if (enemy.character.alive) {
                        enemies.Add(enemy.character);
                    }
                }
                chooseTargets(enemies.ToArray());
                break;
        }
    }

    public void chooseTargets (Character[] targets) {
		FightProcessor.instance.actionTargets = targets;
    }
}