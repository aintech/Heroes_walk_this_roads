using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class FightProcessor : MonoBehaviour {
	
	private List<TurnResult> turnResults = new List<TurnResult>();

	private StateMachine machineState = StateMachine.NOT_IN_FIGHT;

	private ElementsHolder elementsHolder;

	private List<EnemyHolder> enemies;

	public static bool PLAYER_CHECKED_ELEMENTS = false;

	public static bool PLAYER_MOVE_DONE = false;

	public static bool ELEMENTS_ANIM_DONE = true;

	public static bool FIGHT_ANIM_PLAYER_DONE = true, FIGHT_ANIM_ENEMY_DONE = true;

	private FightScreen fightScreen;

    private List<Character> queue;

    private Character currCharacter;

    private int characterIndex;

    private List<Hero> heroes;

    private EnemyHolder currEnemy;

	public void init (FightScreen fightScreen, ElementsHolder elementsHolder, List<EnemyHolder> enemies) {
		this.fightScreen = fightScreen;
		this.elementsHolder = elementsHolder;
		this.enemies = enemies;
	}

    public void prepare (List<EnemyType> enemyTypes) {
        queue = new List<Character>();
        foreach (Hero hero in Vars.heroes.Values) {
            queue.Add(hero);
        }
        foreach (EnemyHolder enemyHolder in enemies) {
            queue.Add(enemyHolder.enemy);
        }

        Character temp;
        for (int write = 0; write < queue.Count; write++) {
            for (int sort = 0; sort < queue.Count-1; sort++) {
                if (queue[sort].initiative > queue[sort+1].initiative || (queue[sort].initiative == queue[sort+1].initiative && !queue[sort].isHero() && queue[sort+1].isHero())) {
                    temp = queue[sort+1];
                    queue[sort+1] = queue[sort];
                    queue[sort] = temp;
                }
            }
        }
        currEnemy = enemies[0];
    }

    public void startFight () {
        characterIndex = -1;
		updateStatusEffects();
        heroes = new List<Hero>(Vars.heroes.Values);
		switchMachineState(StateMachine.CHECKING_ELEMENTS_POOL_ACTIONS);
	}

	void Update () {
		switch (machineState) {
			case StateMachine.NOT_IN_FIGHT: break;
			case StateMachine.CHECKING_ELEMENTS_POOL_ACTIONS: checkPlayerChoosenElements (); break;
			case StateMachine.AFTER_ELEMENTS_CHECKED: break;
            case StateMachine.PICK_NEXT_CHARACTER: pickNextCharacter(); break;
            case StateMachine.CHARACTER_TURN: checkCharacterTurn(); break;
            case StateMachine.ICONS_ANIMATION: checkFightAndIconAnimation(); break;
            case StateMachine.AFTER_HERO_TURN: afterHeroTurn(); break;
            case StateMachine.ICONS_POSITIONING: checkIconPositioning(); break;
            case StateMachine.AFTER_ENEMY_TURN: afterEnemyTurn(); break;
			case StateMachine.PLAYER_WIN: endFight(true); break;
			case StateMachine.ENEMY_WIN: endFight(false); break;
		}
	}

	private void checkPlayerChoosenElements () {
		elementsHolder.checkPlayerInput();
		if (PLAYER_CHECKED_ELEMENTS) {
			switchMachineState(StateMachine.PICK_NEXT_CHARACTER);
		}
	}

	private void afterElementsChecked () {
		Debug.Log("Lets move on!");
	}

    private void pickNextCharacter () {
        PLAYER_MOVE_DONE = false;
        if (currCharacter != null) { currCharacter.representative.choose(false); }
        characterIndex++;
        if (characterIndex >= queue.Count) {
            characterIndex = 0;
        }
        currCharacter = queue[characterIndex];
        currCharacter.representative.choose(true);
        switchMachineState(StateMachine.CHARACTER_TURN);
    }

    private void checkCharacterTurn () {
        if (currCharacter.isHero()) {
            elementsHolder.checkPlayerInput();
            if (PLAYER_MOVE_DONE) {
                switchMachineState(StateMachine.ICONS_ANIMATION);
            }
        } else { calculateEnemyTurnResult(); }
    }

    private void checkFightAndIconAnimation () {
        if (FIGHT_ANIM_PLAYER_DONE && FIGHT_ANIM_ENEMY_DONE) {
            switchMachineState(StateMachine.AFTER_HERO_TURN);
        }
    }

    private void afterHeroTurn () {
        elementsHolder.refreshSortingOrder();
        elementsHolder.repositionMatchingElements();
        elementsHolder.setElementsGoToCenter();

        int totalHealth = 0;

        foreach (EnemyHolder holder in enemies) {
            totalHealth += holder.enemy.health;
        }

        if (totalHealth == 0) {
            switchMachineState(StateMachine.PLAYER_WIN);
        } else {
            switchMachineState(StateMachine.ICONS_POSITIONING);
        }
	}

    private void checkIconPositioning () {
        if (elementsHolder.isAllElementsOnCells()) {
//            if (enemy.enemy.health <= 0) { switchMachineState(StateMachine.PLAYER_WIN); }
//            else if (Player.health <= 0) { switchMachineState(StateMachine.ENEMY_WIN); }
            /*else*/ if (elementsHolder.checkElementsMatch()) {
                switchMachineState(StateMachine.ICONS_ANIMATION);
            } else {
                if (FIGHT_ANIM_PLAYER_DONE && FIGHT_ANIM_ENEMY_DONE) {
                    switchMachineState(StateMachine.PICK_NEXT_CHARACTER);
                }
            }
        }
    }

    private void afterEnemyTurn () {
        if (FIGHT_ANIM_PLAYER_DONE && FIGHT_ANIM_ENEMY_DONE) {
            int totalHealth = 0;

            foreach (Hero hero in heroes) {
                totalHealth += hero.health;
            }

            if (totalHealth == 0) {
                switchMachineState(StateMachine.ENEMY_WIN);
            } else {
                switchMachineState(StateMachine.PICK_NEXT_CHARACTER);
            }
        }    
    }

	private void rearrangeIcons () {
		elementsHolder.rearrangeElements();
	}

	public void addToTurnResult (ElementType type, int count, Vector2 pos) {
		foreach (TurnResult result in turnResults) {
			if (result.elementType == type) {
				result.addCount(count);
				result.position = getMiddlePoint(result.position, pos);
				return;
			}
		}
		turnResults.Add(new TurnResult(type, count, pos));
        switchMachineState(StateMachine.AFTER_HERO_TURN);
	}

	private Vector2 getMiddlePoint (Vector2 pos1, Vector2 pos2) {
		float x = (pos1.x + pos2.x) * .5f;
		float y = (pos1.y + pos2.y) * .5f;
		return new Vector2(x, y);
	}

	public void calculateHeroTurnResults () {
		if (turnResults.Count == 0) { return; }

		foreach (TurnResult result in turnResults) {
			int damage = Player.randomDamage;
			if (result.count > 3) {
				damage += Mathf.RoundToInt((float)Player.randomDamage * .5f) *  (result.count - 3);
			}
			fightScreen.getIconEffectPlayer().addEffect(result.elementType, damage, result.position, result.count);
		}

		turnResults.Clear();
	}

	private void calculateEnemyTurnResult () {
        if (fightScreen.getStatusEffectByType(StatusEffectType.BLINDED, false).inProgress && (Random.value < .5f)) {
            fightScreen.fightEffectPlayer.playEffect(FightEffectType.MISS, 0);
        } else {
            fightScreen.fightEffectPlayer.playEffect(FightEffectType.DAMAGE, Player.hitPlayer(currEnemy.enemy.damage()));
        }
		FIGHT_ANIM_PLAYER_DONE = false;
        switchMachineState(StateMachine.AFTER_ENEMY_TURN);
	}


	public void checkEffectsActive () {
		if (!fightScreen.getIconEffectPlayer().isPlayingEffect()) {
			FIGHT_ANIM_ENEMY_DONE = true;
		}
	}

	private void switchMachineState (StateMachine machineState) {
		this.machineState = machineState;
	}

	public void skipMove () {
		PLAYER_MOVE_DONE = true;
	}

	public bool canUseSupply (SupplyType supplyType) {
        if (machineState != StateMachine.CHARACTER_TURN && !currCharacter.isHero()) {
			return false;
		}
		if (supplyType == SupplyType.SPEED_POTION || supplyType == SupplyType.ARMOR_POTION || supplyType == SupplyType.REGENERATION_POTION) {
			StatusEffectType type = supplyType.toStatusEffectType ();
			foreach (StatusEffect eff in fightScreen.playerStatusEffects) {
				if (eff.statusType == type && eff.isFired) {
					Messenger.showMessage("Персонаж уже находится под действием эффекта '" + eff.statusType.name() + "'");
					return false;
				}
			}
		} else if (supplyType == SupplyType.BLINDING_POWDER || supplyType == SupplyType.PARALIZING_DUST) {
			StatusEffectType type = supplyType.toStatusEffectType ();
			foreach (StatusEffect eff in fightScreen.enemyStatusEffects) {
				if (eff.statusType == type && eff.isFired) {
					Messenger.showMessage("Противник уже находится под действием эффекта '" + eff.statusType.name() + "'");
					return false;
				}
			}
		}
		return true;
	}

    private void updateStatusEffects () {
        //          foreach (StatusEffect eff in fightScreen.playerStatusEffects) {
        //              eff.updateStatus ();
        //          }
        //          foreach (StatusEffect eff in fightScreen.enemyStatusEffects) {
        //              eff.updateStatus ();
        //          }
    }

	private void endFight (bool playerWin) {
		switchMachineState(StateMachine.NOT_IN_FIGHT);
		fightScreen.finishFight(playerWin);
	}

	private enum StateMachine {
		NOT_IN_FIGHT,
		CHECKING_ELEMENTS_POOL_ACTIONS, AFTER_ELEMENTS_CHECKED,
		ICONS_ANIMATION, ICONS_POSITIONING,
        PICK_NEXT_CHARACTER, CHARACTER_TURN, AFTER_HERO_TURN, AFTER_ENEMY_TURN,
		PLAYER_WIN, ENEMY_WIN
	}

	public class TurnResult {
		public ElementType elementType {get; private set; }
		public int count { get; private set; }
		public Vector2 position;

		public TurnResult (ElementType elementType, int count, Vector2 position) {
			this.elementType = elementType;
			this.count = count;
			this.position = position;
		}

		public void addCount (int value) {
			this.count += value;
		}
	}
}