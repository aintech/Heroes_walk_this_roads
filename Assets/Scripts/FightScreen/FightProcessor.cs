using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class FightProcessor : MonoBehaviour {
	
	private List<TurnResult> turnResults = new List<TurnResult>();

	private StateMachine machineState = StateMachine.NOT_IN_FIGHT;

	private ElementsHolder elementsHolder;

	private Enemy enemy;

	public static bool PLAYER_MOVE_DONE = false;

	public static bool ELEMENTS_ANIM_DONE = true;

	public static bool FIGHT_ANIM_PLAYER_DONE = true, FIGHT_ANIM_ENEMY_DONE = true;

	private FightScreen fightScreen;

	private int playerActions, enemyActions;

    private float playerInitiativeDiff, enemyInitiativeDiff, playerInitiativeCounter, enemyInitiativeCounter;

	public void init (FightScreen fightScreen, ElementsHolder elementsHolder, Enemy enemy) {
		this.fightScreen = fightScreen;
		this.elementsHolder = elementsHolder;
		this.enemy = enemy;
	}

    public void startFight () {
        playerInitiativeCounter = 0;
        enemyInitiativeCounter = 0;
        calcInitiative();
		calcActions();
		updateStatusEffects();
		switchMachineState(StateMachine.PLAYER_TURN);
	}

    private void calcInitiative () {
        playerInitiativeDiff = Mathf.Max(1f, (float)Player.initiative / (float)enemy.initiative) - 1;
        enemyInitiativeDiff = Mathf.Max(1f, (float)enemy.initiative / (float)Player.initiative) - 1;
    }

	private void calcActions () {
		calcActions(true);
		calcActions(false);
	}

	private void calcActions (bool asPlayer) {
		if (fightScreen.getStatusEffectByType(StatusEffectType.PARALIZED, asPlayer).inProgress) {
			if (asPlayer) { playerActions = 0; }
			else { enemyActions = 0; }
			fightScreen.updateActionTexts(playerActions, enemyActions);
			return;
		}

        int actions = 1;
		if (asPlayer) {
            playerInitiativeCounter += playerInitiativeDiff;
            if (playerInitiativeCounter > 1) {
                actions += Mathf.FloorToInt(playerInitiativeCounter);
                playerInitiativeCounter -= Mathf.FloorToInt(playerInitiativeCounter);
            }
        } else {
            enemyInitiativeCounter += enemyInitiativeDiff;
            if (enemyInitiativeCounter > 1) {
                actions += Mathf.FloorToInt(enemyInitiativeCounter);
                enemyInitiativeCounter -= Mathf.FloorToInt(enemyInitiativeCounter);
            }
        }

		if (fightScreen.getStatusEffectByType(StatusEffectType.SPEED, asPlayer).inProgress) {
			actions += fightScreen.getStatusEffectByType(StatusEffectType.SPEED, asPlayer).value;
		}

		if (asPlayer) { playerActions = actions; }
		else { enemyActions = actions; }

		fightScreen.updateActionTexts(playerActions, enemyActions);
	}

	void Update () {
		switch (machineState) {
			case StateMachine.NOT_IN_FIGHT: break;
			case StateMachine.PLAYER_TURN:
				if (playerActions == 0) {
					PLAYER_MOVE_DONE = false;
					switchMachineState(StateMachine.ICONS_ANIMATION);
				} else if (PLAYER_MOVE_DONE) {
					PLAYER_MOVE_DONE = false;
					playerActions--;
					fightScreen.updateActionTexts(playerActions, enemyActions);
					switchMachineState(StateMachine.ICONS_ANIMATION);
				} else {
					elementsHolder.checkPlayerInput();
				}
				break;
			case StateMachine.ICONS_ANIMATION:
				if (FIGHT_ANIM_PLAYER_DONE && FIGHT_ANIM_ENEMY_DONE) {
					switchMachineState(StateMachine.PLAYER_MOVE_DONE);
				}
				break;
			case StateMachine.PLAYER_MOVE_DONE:
				afterPlayerMove();
				switchMachineState(StateMachine.ICONS_POSITIONING);
				break;
			case StateMachine.ICONS_POSITIONING:
				if (elementsHolder.isAllElementsOnCells()) {
					if (enemy.health <= 0) { switchMachineState(StateMachine.PLAYER_WIN); }
					else if (Player.health <= 0) { switchMachineState(StateMachine.ENEMY_WIN); }
					else if (elementsHolder.checkElementsMatch()) {
						switchMachineState(StateMachine.ICONS_ANIMATION);
					} else {
						if (playerActions == 0) {
							switchMachineState(StateMachine.ENEMY_TURN);
						} else {
							switchMachineState(StateMachine.PLAYER_TURN);
						}
					}
				}
				break;
			case StateMachine.ENEMY_TURN:
				if (enemyActions > 0) { calculateEnemyTurnResult(); }
				switchMachineState(StateMachine.ENEMY_MOVE_DONE);
				break;
			case StateMachine.ENEMY_MOVE_DONE:
				if (FIGHT_ANIM_PLAYER_DONE && FIGHT_ANIM_ENEMY_DONE) {
					if (Player.health <= 0) { switchMachineState(StateMachine.ENEMY_WIN); }
					else if (enemyActions > 0) { switchMachineState(StateMachine.ENEMY_TURN); }
					else {
						updateStatusEffects();
						calcActions();
						switchMachineState(StateMachine.PLAYER_TURN);
					}
				}
				break;
			case StateMachine.PLAYER_WIN:
				endFight(true);
				break;
			case StateMachine.ENEMY_WIN:
				endFight(false);
				break;
		}
	}

	private void updateStatusEffects () {
			foreach (StatusEffect eff in fightScreen.playerStatusEffects) {
				eff.updateStatus ();
			}
			foreach (StatusEffect eff in fightScreen.enemyStatusEffects) {
				eff.updateStatus ();
			}
	}

	private void afterPlayerMove () {
		elementsHolder.refreshSortingOrder();
		elementsHolder.repositionMatchingElements();
		elementsHolder.setElementsGoToCenter();
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
            fightScreen.fightEffectPlayer.playEffect(FightEffectType.DAMAGE, Player.hitPlayer(enemy.damage));
        }
		FIGHT_ANIM_PLAYER_DONE = false;
		enemyActions--;
		fightScreen.updateActionTexts(playerActions, enemyActions);
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
		if (machineState != StateMachine.PLAYER_TURN || playerActions == 0) {
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

	private void endFight (bool playerWin) {
		switchMachineState(StateMachine.NOT_IN_FIGHT);
		fightScreen.finishFight(playerWin);
	}

	private enum StateMachine {
		NOT_IN_FIGHT,
		ICONS_ANIMATION, ICONS_POSITIONING,
		PLAYER_TURN, PLAYER_MOVE_DONE,
		ENEMY_TURN, ENEMY_MOVE_DONE,
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