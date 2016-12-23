using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class FightScreen : MonoBehaviour, ButtonHolder {

    public Transform queuePortraitPrefab, enemyHolderPrefab;

	private ElementsHolder elementsHolder;

	private SpriteRenderer iconsHolderRender;

	private Color holderColor = new Color(1, 1, 1, 0);

	//private float enemyFinalX = 4.4f, enemyAppearSpeed = .2f;

	private Vector3 enemyPos;

	public FightEffectPlayer fightEffectPlayer { get; private set; }

    public ElementEffectPlayer elementEffectPlayer { get; private set; }

    public FightInterface fightInterface { get; private set; }

    private List<EnemyHolder> enemies = new List<EnemyHolder>();

    public FightProcessor fightProcessor { get; private set; }

	private bool fightStarted, startAnimDone, enemyDeadPlaying;

	private bool playerWin;

    private StatusScreen statusScreen;

	private List<SupplySlot> supplySlots = new List<SupplySlot>();

	private ItemDescriptor itemDescriptor;

    public List<StatusEffect> playerStatusEffects { get; private set; }

    public List<StatusEffect> enemyStatusEffects { get; private set; }

	private List<StatusEffect> effList;

	private Vector3 playerStatusStartPosition = new Vector3(6.8f, 0, 0),
					enemyStatusStartPosition = new Vector3(6.9f, 7.5f, 0);

	private float playerStatusStep = 1.1f, enemyStatusStep = -.9f;

	private Button captureBtn, releaseBtn;

    [HideInInspector]
    public World world;

    public ElementsPool elementsPool { get; private set; }

    private List<QueuePortrait> queuePortraits = new List<QueuePortrait>();

    private SpriteRenderer backgroundRender;

    private int topLayerOrder;

	public FightScreen init () {
        statusScreen = Vars.gameplay.statusScreen;
        itemDescriptor = Vars.gameplay.itemDescriptor;

        playerStatusEffects = new List<StatusEffect>();
        enemyStatusEffects = new List<StatusEffect>();

		fightProcessor = GetComponent<FightProcessor>();
		itemDescriptor.fightScreen = this;
		elementsHolder = transform.Find ("Elements Holder").GetComponent<ElementsHolder> ().init (this);;
		iconsHolderRender = elementsHolder.GetComponent<SpriteRenderer>();
		fightEffectPlayer = transform.Find("Fight Effect Player").GetComponent<FightEffectPlayer>().init();
		elementEffectPlayer = transform.Find("ElementEffectPlayer").GetComponent<ElementEffectPlayer>();
		fightInterface = transform.Find("Fight Interface").GetComponent<FightInterface>().init(this);
//		enemy = transform.Find("Enemy").GetComponent<EnemyHolder>();
//		enemyPos = enemy.transform.localPosition;
//		enemy.init(this);

        elementsPool = transform.Find("Elements Pool").GetComponent<ElementsPool>().init();

        elementEffectPlayer.init(this, elementsPool);
		fightProcessor.init(this, elementsHolder, enemies);

		elementsHolder.gameObject.SetActive(true);
		gameObject.SetActive(false);

		Transform supplyHolder = transform.Find("Supply Holder");
		SupplySlot slot;
		for (int i = 0; i < supplyHolder.childCount; i++) {
			slot = supplyHolder.GetChild(i).GetComponent<SupplySlot>();
			slot.init();
			supplySlots.Add(slot);
		}
		supplyHolder.gameObject.SetActive(true);

		captureBtn = transform.Find ("Capture Button").GetComponent<Button> ().init ();
		releaseBtn = transform.Find ("Release Button").GetComponent<Button> ().init ();

        backgroundRender = transform.Find("Background").GetComponent<SpriteRenderer>();
        backgroundRender.gameObject.SetActive(true);

//		Player.fightScreen = this;

		return this;
	}

    public void startFight (List<EnemyType> types) {
        initEnemies(types);
		SupplySlot supSlot;
        foreach (SupplySlot slot in statusScreen.supplySlots) {
			if (slot.item != null) {
				supSlot = getSlot (slot.index);
				supSlot.setItem(slot.takeItem());
				supSlot.item.transform.localScale = Vector3.one;
			}
        }
        itemDescriptor.setEnabled();
		playerWin = false;
        fightInterface.updateHeroActions();
//		enemy.initEnemy(types[0]);
		holderColor = new Color(1, 1, 1, 0);
		iconsHolderRender.color = holderColor;
		elementsHolder.initializeElements();
//		enemy.transform.localPosition = new Vector2(10, enemyPos.y);
//		enemyPos = enemy.transform.localPosition;
//		fightInterface.setEnemy(enemy);
		elementsHolder.setActive(true);

		fightStarted = startAnimDone = false;
//		foreach (StatusEffect eff in enemyStatusEffects) {
//			eff.initEnemy (enemy);
//		}

        fightInterface.updateHeroRepresentatives();
        fightProcessor.prepare(types);

		captureBtn.setVisible (false);
		releaseBtn.setVisible (false);

        fightInterface.setHeroActionsVisible(null);

		gameObject.SetActive(true);
	}

    private void initEnemies (List<EnemyType> types) {
        if (types.Count > enemies.Count) {
            while (enemies.Count < types.Count) {
                enemies.Add(Instantiate<Transform>(enemyHolderPrefab).GetComponent<EnemyHolder>().init(this));
            }
        }
        float scrWidth = Camera.main.ScreenToWorldPoint(new Vector2(Screen.width, Screen.height)).x * 2;
        float halfScreen = scrWidth * .5f;
        float offset = scrWidth / (float) (types.Count + 1);
//        float offset = scrWidth / (float) (types.Count * 2);
        for (int i = 0; i < types.Count; i++) {
            enemies[i].initEnemy(types[i], (offset * (i + 1)) - halfScreen, (i * 5 + 5));
//            enemies[i].initEnemy(types[i], (offset * ((i * 2) + 1)) - halfScreen);
        }

        topLayerOrder = (types.Count * 5);
//        backgroundRender.sortingOrder = topLayerOrder - 1;
    }

	private SupplySlot getSlot (int index) {
		foreach (SupplySlot slot in supplySlots) {
			if (slot.index == index) { return slot; }
		}
		Debug.Log("Unknown slot index: " + index);
		return null;
	}

	void Update () {
		if (!fightStarted) {
			if (!startAnimDone) {
				animatingFightStart();
            } else if (ElementsHolder.ELEMENTS_ANIM_DONE) {
				fightStarted = true;
				fightProcessor.startFight();
			}
		}
	}

	private void animatingFightStart () {
//		if (enemy.transform.localPosition.x > enemyFinalX) { enemyPos.x -= enemyAppearSpeed; }
//		else { enemyPos.x = enemyFinalX; }

		if (holderColor.a < 1) {
			holderColor.a += .03f;
			iconsHolderRender.color = holderColor;
		} else {
			startAnimDone = true;
		}
//		else { holderColor.a = 1;}

//		enemy.transform.localPosition = enemyPos;

//		if (holderColor.a >= 1 && enemy.transform.position.x <= enemyFinalX) {
//			startAnimDone = true;
//			elementsHolder.holderAnimator.playElementsApperance ();
////			elementsHolder.startElementsDrop();
//		}
	}

	public void finishFight (bool playerWin) {
		this.playerWin = playerWin;
//		ENEMY_DEAD_ANIM_DONE = !playerWin;

//		if (playerWin) {
//			enemyDeadAnimator.gameObject.SetActive(true);
//			enemyDeadAnimator.Play("EnemyDead");
//			enemy.destroyEnemy();
//			enemyDeadPlaying = true;
//		} else {
//			showFightResultScreen();
//		}

		foreach (StatusEffect eff in playerStatusEffects) { eff.endEffect(); }
		foreach (StatusEffect eff in enemyStatusEffects) { eff.endEffect(); }

		itemDescriptor.setDisabled();
		elementsHolder.holderAnimator.playElementsDisapperance ();
		elementsHolder.setActive(false);
	}

	public void showFightEndDisplay () {
		bool emptyChamber = false;
//		foreach (StasisChamber chamber in chambersHolder.chambers) {
//			if (chamber.isEmpty) { emptyChamber = true; break; }
//		}
		captureBtn.setText(emptyChamber? "В стазис камеру": "Нет свободных камер");
		captureBtn.setActive (emptyChamber);

		captureBtn.setVisible (true);
		releaseBtn.setVisible (true);
	}

	public void fireClickButton (Button btn) {
		if (btn == captureBtn) {
			captureEnemy ();
		} else if (btn == releaseBtn) {
			closeFightScreen ();
		}
	}

	private void captureEnemy () {
//		foreach (StasisChamber chamber in chambersHolder.chambers) {
//			if (chamber.isEmpty) { chamber.putInChamber(enemy.enemyType); break; }
//		}
		closeFightScreen ();
	}

//	private void showFightResultScreen () {
//		enemyDeadPlaying = false;
//		resultScreen.showFightResultScreen(playerWin? enemy: null);
//	}

	public void closeFightScreen () {
		gameObject.SetActive(false);
		SupplySlot supSlot;
		foreach (SupplySlot slot in supplySlots) {
			if (slot.item != null) {
                supSlot = statusScreen.getSupplySlot (slot.index);
				supSlot.setItem(slot.takeItem());
				supSlot.item.transform.localScale = Vector3.one;
			}
		}
        world.backFromFight(playerWin);
	}

	public void useSupply (SupplySlot slot) {
		if (slot.item != null) {
			SupplyData data = (SupplyData)slot.item.itemData;
			if (fightProcessor.canUseSupply (data.type)) {
				bool toPlayer = data.type != SupplyType.BLINDING_POWDER && data.type != SupplyType.PARALIZING_DUST;
//				StatusEffect statusEffect = getStatusEffectByType(data.type.toStatusEffectType(), toPlayer);
				FightEffectType fightEffectType = data.type.toFightEffectType();

//				if (data.type == SupplyType.HEALTH_POTION) {
//					fightEffectPlayer.playEffect(fightEffectType, Player.heal(data.value));
//				} else if (data.type == SupplyType.PARALIZING_DUST || data.type == SupplyType.BLINDING_POWDER) {
//					fightEffectPlayer.playEffectOnEnemy (fightEffectType, 0);
//					statusEffect.addStatus (data.value, data.duration);
//				} else if (data.type == SupplyType.ARMOR_POTION || data.type == SupplyType.REGENERATION_POTION || data.type == SupplyType.SPEED_POTION) {
//					fightEffectPlayer.playEffect (fightEffectType, data.value);
//					statusEffect.addStatus (data.value, data.duration);
//				} else {
//					Debug.Log("Unknown status effect type");
//				}

				Vector3 effectPos = toPlayer? playerStatusStartPosition: enemyStatusStartPosition;

				int activeEffects = -1;//вычитаем добавляемый эффект

				effList = toPlayer? playerStatusEffects: enemyStatusEffects;

				for (int i = 0; i < effList.Count; i++) {
					if (effList[i].isFired) { activeEffects++; }
				}

				if (toPlayer) { effectPos.x += activeEffects * playerStatusStep; }
				else { effectPos.y += activeEffects * enemyStatusStep; }

//				statusEffect.transform.localPosition = effectPos;

				fightProcessor.skipMove ();
				FightProcessor.FIGHT_ANIM_PLAYER_DONE = false;
				slot.takeItem ().dispose ();
			}
		}
	}

//	public StatusEffect getStatusEffectByType (StatusEffectType type, bool toPlayer) {
//		if (type.withoutStatusHolder()) { return null; }
//		effList = toPlayer? playerStatusEffects: enemyStatusEffects;
//		foreach (StatusEffect eff in effList) {
//			if (eff.statusType == type) {
//				return eff;
//			}
//		}
//		Debug.Log ("Cant find holder of effect type: " + type);
//		return null;
//	}
}