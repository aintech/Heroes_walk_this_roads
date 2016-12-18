using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class EnemyHolder : MonoBehaviour {

	private SpriteRenderer render;

	public int health { get; private set; }

	public int maxHealth { get; private set; }

	public int armor { get; private set; }

	public int damage { get; private set; }

	public int initiative { get; private set; }

	public EnemyType enemyType { get; private set; }

	private FightScreen fightScreen;

	public void init (FightScreen fightScreen) {
		this.fightScreen = fightScreen;
		render = GetComponent<SpriteRenderer>();
	}

	public void initEnemy (EnemyType enemyType) {
		this.enemyType = enemyType;
		damage = enemyType.strenght();
		health = enemyType.endurance() * 10;
		maxHealth = health;
		armor = enemyType.armor();
		initiative = enemyType.agility();
		setSprite();
		gameObject.SetActive(true);
	}

	private void setSprite () {
		render.sprite = Imager.getEnemy(enemyType, (float) health / (float)maxHealth);
	}

	public void playHit () {
//		animInAction = true;
//		endTime = Time.time + duration;
//		setSprite();
	}

	public int hitEnemy (int damageAmount, int iconsCount)  {
		int armorAmount = armor;
		if (fightScreen.getStatusEffectByType(StatusEffectType.ARMORED, false).inProgress) {
			armorAmount += fightScreen.getStatusEffectByType(StatusEffectType.ARMORED, false).value;
		}
		if (damageAmount <= armorAmount) {
			return 0;
		} else {
			health -= (damageAmount - armorAmount);
			setSprite();
//			Player.updatePerk(PerkType.SWORDSMAN, damageAmount * damageToPerkMultiplier);
			return damageAmount - armorAmount;
		}
	}

	public int heal (int amount) {
		if (amount + health > maxHealth) {
			int calc = maxHealth - health;
			health = maxHealth;
//			if (fightInterface.gameObject.activeInHierarchy) { fightInterface.updatePlayerBar(); }
			return calc;
		} else {
			health += amount;
//			if (fightInterface.gameObject.activeInHierarchy) { fightInterface.updatePlayerBar(); }
			return amount;
		}
	}

	public void destroyEnemy () {
		gameObject.SetActive(false);
	}
}