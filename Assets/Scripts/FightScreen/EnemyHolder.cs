using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class EnemyHolder : CharacterRepresentative {

	private SpriteRenderer render;

    public Enemy enemy { get; private set; }

	private FightScreen fightScreen;

    public EnemyHolder init (FightScreen fightScreen) {
		this.fightScreen = fightScreen;
		render = GetComponent<SpriteRenderer>();
        return this;
	}

    public Enemy initEnemy (EnemyType enemyType) {
        enemy = new Enemy().init(enemyType);
        enemy.representative = this;
        updateSprite();
		gameObject.SetActive(true);
        return enemy;
	}

	private void updateSprite () {
        render.sprite = ImagesProvider.getEnemy(enemy.type);// Imager.getEnemy(enemy.type, (float) enemy.health / (float) enemy.maxHealth);
	}

	public void playHit () {
//		animInAction = true;
//		endTime = Time.time + duration;
//		setSprite();
	}

    public override void choose (bool asActive) {
//        throw new System.NotImplementedException ();
    }

	public void destroyEnemy () {
		gameObject.SetActive(false);
	}
}