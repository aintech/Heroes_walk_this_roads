using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

public class FightProcessor : MonoBehaviour {

	public static FightProcessor instance { get; private set;}

	private List<TurnResult> turnResults = new List<TurnResult>();

	private StateMachine machineState = StateMachine.NOT_IN_FIGHT;

	private ElementsHolder elementsHolder;

	public static bool PLAYER_CHECKED_ELEMENTS = false;

	public static bool PLAYER_MOVE_DONE = false;

	public static bool FIGHT_ANIM_PLAYER_DONE = true, FIGHT_ANIM_ENEMY_DONE = true;

    private List<EnemyRepresentative> enemies;

    private List<Hero> heroes;

	private List<Character> queue;

    private Character currCharacter;

    private EnemyRepresentative currEnemy;

    [HideInInspector]
    public HeroAction heroAction;

    [HideInInspector]
    public Character[] actionTargets;

	public void init (ElementsHolder elementsHolder, List<EnemyRepresentative> enemies) {
		this.elementsHolder = elementsHolder;
		this.enemies = enemies;
		instance = this;
	}

    public void prepareQueue (List<EnemyType> enemyTypes) {
		queue = new List<Character>();
        foreach (Hero hero in Vars.heroes.Values) {
			if (hero.alive) {
				queue.Add(hero);
			}
        }
        foreach (EnemyRepresentative EnemyRepresentative in enemies) {
            queue.Add(EnemyRepresentative.character);
        }
		redefineQueue();
		FightInterface.instance.updateQueue(queue);
    }

	private void redefineQueue () {
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
        queue.Reverse();
	}

    public void removeFromQueue (Character character) {
        queue.Remove(character);
        if (character.isHero()) { heroes.Remove((Hero)character); }
        else { enemies.Remove((EnemyRepresentative)((Enemy)character).representative); }
		FightInterface.instance.updateQueue(queue);
    }

    public void startFight () {
		updateStatusEffects();
        heroes = new List<Hero>(Vars.heroes.Values);
        startNextTurn();

//        switchMachineState(StateMachine.BEFORE_FIGHT_START);
	}

	void Update () {
//        Debug.Log(machineState);
		switch (machineState) {
            case StateMachine.NOT_IN_FIGHT: break;
//            case StateMachine.BEFORE_FIGHT_START: checkForStart(); break;
			case StateMachine.CHECKING_ELEMENTS_POOL_ACTIONS: checkPlayerChoosenElements (); break;
            case StateMachine.AFTER_ELEMENTS_CHECKED: afterElementsChecked(); break;
            case StateMachine.PICK_NEXT_CHARACTER: pickNextCharacter(); break;
            case StateMachine.CHARACTER_TURN: checkCharacterTurn(); break;
            case StateMachine.FIGHT_ANIMATION: checkFightAnimation(); break;
            case StateMachine.AFTER_HERO_TURN: afterHeroTurn(); break;
            case StateMachine.AFTER_ENEMY_TURN: afterEnemyTurn(); break;
			case StateMachine.PLAYER_WIN: endFight(true); break;
			case StateMachine.ENEMY_WIN: endFight(false); break;
		}
	}

//    private void checkForStart () {
//        if (ElementsHolder.ELEMENTS_ANIM_DONE) {
//            currCharacter = null;
//            foreach(Character ch in queue) {
//                ch.moveDone = false;
//            }
//            fightScreen.fightInterface.updateQueue(queue);
//            foreach(EnemyRepresentative en in enemies) {
//                en.sendToBackground();
//            }
//
//            switchMachineState(StateMachine.CHECKING_ELEMENTS_POOL_ACTIONS);
//        }
//    }

    private void startNextTurn () {
        currCharacter = null;
        foreach(Character ch in queue) {
            ch.moveDone = false;
        }
		FightInterface.instance.updateQueue(queue);
        foreach(EnemyRepresentative en in enemies) {
            en.sendToBackground();
        }

        elementsHolder.gameObject.SetActive(true);
        elementsHolder.holderAnimator.playElementsApperance();
		FightScreen.instance.elementsPool.changeSize(false);

        switchMachineState(StateMachine.CHECKING_ELEMENTS_POOL_ACTIONS);
    }

	private void checkPlayerChoosenElements () {
		elementsHolder.checkPlayerInput();
		if (PLAYER_CHECKED_ELEMENTS) {
            PLAYER_CHECKED_ELEMENTS = false;
            switchMachineState(StateMachine.AFTER_ELEMENTS_CHECKED);
		}
	}

	private void afterElementsChecked () {
        if (ElementsHolder.ELEMENTS_ANIM_DONE) {
//            elementsHolder.gameObject.SetActive(false);
//            switchMachineState(StateMachine.CHECKING_ELEMENTS_POOL_ACTIONS);
            elementsHolder.holderAnimator.playElementsDisapperance();
            switchMachineState(StateMachine.WAIT);
        }
	}

    public void startFightPart () {
        elementsHolder.gameObject.SetActive(false);
		FightScreen.instance.elementsPool.changeSize(true);
        switchMachineState(StateMachine.PICK_NEXT_CHARACTER);
    }

    public void playElementEffects () {
        if (turnResults.Count == 0) { return; }

        foreach (TurnResult result in turnResults) {
//            int damage = Player.randomDamage;
//            if (result.count > 3) {
//                damage += Mathf.RoundToInt((float)Player.randomDamage * .5f) *  (result.count - 3);
//            }
			FightScreen.instance.elementEffectPlayer.addEffect(result.elementType, result.position, result.count);
        }
        turnResults.Clear();
    }

    private void pickNextCharacter () {
        actionTargets = null;
        PLAYER_MOVE_DONE = false;
        if (currCharacter != null) {
            currCharacter.representative.setChosen(false);
            currCharacter.moveDone = true;
			Character ch = queue[0];
			queue.RemoveAt(0);
			queue.Add(ch);
		}

        currCharacter = queue[0];
		currCharacter.guarded = false;
        currCharacter.refreshStatuses();
        if (currCharacter.moveDone) {
            startNextTurn();
        } else {
            currCharacter.representative.setChosen(true);

            if (currCharacter.isHero()) {
                foreach (EnemyRepresentative holder in enemies) {
                    holder.sendToForeground();
                    holder.enabled = true;
                }

				FightInterface.instance.setHeroActionsVisible((Hero)currCharacter);
            } else {
                currEnemy = (EnemyRepresentative)currCharacter.representative;
                currEnemy.setAsCurrentEnemy();
                foreach (EnemyRepresentative holder in enemies) {
                    holder.enabled = false;
                    if (holder != currEnemy) {
                        holder.sendToBackground();
                    }
                }
				FightInterface.instance.setHeroActionsVisible(null);
            }

			FightInterface.instance.updateQueue(queue);

            switchMachineState(StateMachine.CHARACTER_TURN);
        }
    }

    private void checkCharacterTurn () {
        if (currCharacter.isHero()) { 
            if (heroAction != null && actionTargets != null) {
				foreach (KeyValuePair<ElementType, int> pair in heroAction.elementsCost) {
					ElementsPool.instance.elements[pair.Key] -= pair.Value;
				}
				ElementsPool.instance.updateCounters();
                switch (heroAction.actionType) {
                    case HeroActionType.ATTACK:
                        actionTargets[0].hit(currCharacter.randomDamage());
                        break;
					case HeroActionType.GUARD:
						currCharacter.guarded = true;
						break;
					case HeroActionType.HEAL:
						actionTargets[0].heal(50);
						break;
					default: Debug.Log("Action: " + heroAction.actionType); break;
                }
                heroAction = null;
                actionTargets = null;
                switchMachineState(StateMachine.FIGHT_ANIMATION);
            }
        } else { calculateEnemyTurnResult(); }
    }

    private void checkFightAnimation () {
        if (FIGHT_ANIM_PLAYER_DONE && FIGHT_ANIM_ENEMY_DONE) {
            switchMachineState(StateMachine.AFTER_HERO_TURN);
        }
    }

    private void afterHeroTurn () {
        int totalHealth = 0;

        foreach (EnemyRepresentative holder in enemies) {
            totalHealth += holder.character.health;
        }

        if (totalHealth <= 0) {
            switchMachineState(StateMachine.PLAYER_WIN);
        } else {
            switchMachineState(StateMachine.PICK_NEXT_CHARACTER);
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
//        switchMachineState(StateMachine.AFTER_HERO_TURN);
	}

	private Vector2 getMiddlePoint (Vector2 pos1, Vector2 pos2) {
		float x = (pos1.x + pos2.x) * .5f;
		float y = (pos1.y + pos2.y) * .5f;
		return new Vector2(x, y);
	}

    private void calculateEnemyTurnResult () {
        Hero target = heroes[0];// heroes[UnityEngine.Random.Range(0, heroes.Count)];
		FightScreen.instance.fightEffectPlayer.playEffect(FightEffectType.DAMAGE, target.hit(currEnemy.character.randomDamage()));
        ((HeroRepresentative)target.representative).animator.playAnimation(HeroRepresentativeAnimator.AnimationType.DAMAGE);

        if (target.alive) {
            foreach(StatusEffect stat in target.statusEffects.Values) {
                if (!stat.isFired || !stat.inProgress) {
                    target.addStatus(stat.type, 5, 5);
                    break;
                    //                target.addStatus((StatusEffectType)Enum.GetValues(typeof (StatusEffectType)).GetValue(UnityEngine.Random.Range(0, Enum.GetValues(typeof (StatusEffectType)).Length - 1)), 5, 5);
                }
            }
        } else {
            ((HeroRepresentative)target.representative).setAsActive(false);
            removeFromQueue(target);
        }

//        if (fightScreen.getStatusEffectByType(StatusEffectType.BLINDED, false).inProgress && (Random.value < .5f)) {
//            fightScreen.fightEffectPlayer.playEffect(FightEffectType.MISS, 0);
//        } else {
//            fightScreen.fightEffectPlayer.playEffect(FightEffectType.DAMAGE, Player.hitPlayer(currEnemy.enemy.damage()));
//        }
		FIGHT_ANIM_PLAYER_DONE = false;
        switchMachineState(StateMachine.AFTER_ENEMY_TURN);
	}

	public void checkEffectsActive () {
		if (!FightScreen.instance.elementEffectPlayer.isPlayingEffect()) {
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
//        if (machineState != StateMachine.CHARACTER_TURN && !currCharacter.isHero()) {
//			return false;
//		}
//		if (supplyType == SupplyType.SPEED_POTION || supplyType == SupplyType.ARMOR_POTION || supplyType == SupplyType.REGENERATION_POTION) {
//			StatusEffectType type = supplyType.toStatusEffectType ();
//			foreach (StatusEffect eff in fightScreen.playerStatusEffects) {
//				if (eff.statusType == type && eff.isFired) {
//					Messenger.showMessage("Персонаж уже находится под действием эффекта '" + eff.statusType.name() + "'");
//					return false;
//				}
//			}
//		} else if (supplyType == SupplyType.BLINDING_POWDER || supplyType == SupplyType.PARALIZING_DUST) {
//			StatusEffectType type = supplyType.toStatusEffectType ();
//			foreach (StatusEffect eff in fightScreen.enemyStatusEffects) {
//				if (eff.statusType == type && eff.isFired) {
//					Messenger.showMessage("Противник уже находится под действием эффекта '" + eff.statusType.name() + "'");
//					return false;
//				}
//			}
//		}
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
		FightScreen.instance.finishFight(playerWin);
	}

	private enum StateMachine {
		NOT_IN_FIGHT,
        BEFORE_FIGHT_START,
		CHECKING_ELEMENTS_POOL_ACTIONS, AFTER_ELEMENTS_CHECKED,
		FIGHT_ANIMATION,
        PICK_NEXT_CHARACTER, CHARACTER_TURN, AFTER_HERO_TURN, AFTER_ENEMY_TURN,
		PLAYER_WIN, ENEMY_WIN,
        WAIT
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