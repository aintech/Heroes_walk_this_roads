using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class FightInterface : MonoBehaviour, ButtonHolder {

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

    private Button supplyBtn, perksBtn;

    private Transform supplyHolder;

    private SupplySlot[] supplySlots = new SupplySlot[6];

    private Hero chosenHero;

    public HeroAction heroAction { get; private set; }

	public FightInterface init () {
		instance = this;

		Transform portraitsHolder = transform.Find("Portraits");
        portraits = new List<HeroRepresentative>();
		for (int i = 0; i < portraitsHolder.childCount; i++) {
			portraits.Add(portraitsHolder.GetChild(i).GetComponent<HeroRepresentative>().init("Fight Interface"));
		}

        supplyHolder = transform.Find("Supply Holder");
        SupplySlot slot;
        for (int i = 0; i < supplyHolder.childCount; i++) {
            slot = supplyHolder.GetChild(i).GetComponent<SupplySlot>();
            slot.init();
            supplySlots[slot.index] = slot;
        }
        supplyHolder.gameObject.SetActive(false);

        actionsHolder = transform.Find("Actions Holder");
        emptyHolder = actionsHolder.Find("Empty Holder");

		queueHolder = transform.Find("Queue").Find("Queue Holder");

        supplyBtn = transform.Find("Supply Button").GetComponent<Button>().init();
        perksBtn = transform.Find("Perks Button").GetComponent<Button>().init();

        updateHeroActions();

        hideSuppliesAndPerksBtns();

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

    public void setChosenHero (Hero hero) {
        chosenHero = hero;
        foreach (KeyValuePair<Hero, Transform> pair in heroActionHolders) {
            pair.Value.gameObject.SetActive(hero != null && pair.Key == hero);
        }
        if(hero != null) {
            foreach(HeroAction act in heroActions[hero]) {
                act.checkElementsIsEnouth();
                act.refillDescription();
            }
            heroActions[hero][0].setChosen(true);
            foreach (SupplySlot slot in supplySlots) {
                if (slot.item != null) { slot.hideItem(); }
                if (hero.supplies[slot.index] != null) { slot.setItem(hero.supplies[slot.index].item); }
            }
        } else {
            foreach (SupplySlot slot in supplySlots) {
                if (slot.item != null) { slot.hideItem(); }
            }
        }
    }

    public void updateHeroRepresentatives () {
        foreach (HeroRepresentative port in portraits) {
            port.updateRepresentative();
            port.onHealModified();
        }
    }

    public void chooseAction (HeroAction action) {
        heroAction = action;
        if (heroAction != null) {
            foreach(HeroAction act in heroActions[action.hero]) {
                if (act != action) { act.setChosen(false); }
            }
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
    }

    public void chooseTargets (Character[] targets) {
		FightProcessor.instance.actionTargets = targets;
    }

    public void fireClickButton (Button btn) {
        if (btn == supplyBtn) { showSupplies(); }
        else if (btn == perksBtn) { showPerks(); }
        else { Debug.Log("Unknown button: " + btn.name); }
    }

    public void showSupplies () {
        supplyBtn.setVisible(false);
        perksBtn.setVisible(true);
        supplyHolder.gameObject.SetActive(true);
        actionsHolder.gameObject.SetActive(false);
        chooseAction(null);
        foreach (EnemyRepresentative enemy in FightProcessor.instance.enemies) {
            if (enemy.character.alive) {
                enemy.setColliderEnabled(false);
            }
        }
    }

    public void showPerks () {
        perksBtn.setVisible(false);
        supplyBtn.setVisible(true);
        supplyHolder.gameObject.SetActive(false);
        actionsHolder.gameObject.SetActive(true);
        heroActions[chosenHero][0].setChosen(true);
        foreach (EnemyRepresentative enemy in FightProcessor.instance.enemies) {
            if (enemy.character.alive) {
                enemy.setColliderEnabled(true);
            }
        }
    }

    public void hideSuppliesAndPerksBtns () {
        supplyBtn.setVisible(false);
        perksBtn.setVisible(false);
    }
}