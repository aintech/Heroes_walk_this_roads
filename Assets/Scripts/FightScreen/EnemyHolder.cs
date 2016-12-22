using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class EnemyHolder : CharacterRepresentative {

    public Enemy enemy { get; private set; }

	private SpriteRenderer enemyRender, barRender, barBGRender, barHolderRender;

    private Transform healthBar, barHolder;

	private FightScreen fightScreen;

    private Vector3 initPosition = Vector3.zero;

    private Vector3 smallScale = new Vector3(.7f, .7f, 1);

    private string backgroundLayerName = "Fight Screen", foregroundLayerName = "Fight Screen Foreground";

    public EnemyHolder init (FightScreen fightScreen) {
		this.fightScreen = fightScreen;
        enemyRender = GetComponent<SpriteRenderer>();

        Transform healthBarHolder = transform.Find("Health Bar");
        healthBar = healthBarHolder.Find("Bar");
        barHolder = healthBarHolder.Find("Holder");

        barRender = healthBar.GetComponent<SpriteRenderer>();
        barHolderRender = barHolder.GetComponent<SpriteRenderer>();
        barBGRender = healthBarHolder.Find("Background").GetComponent<SpriteRenderer>();

        return this;
	}

    public Enemy initEnemy (EnemyType enemyType, float xPosition, int order) {
        initPosition.x = xPosition;
        enemy = new Enemy().init(enemyType);
        enemy.representative = this;
        updateSprite();
        transform.position = initPosition;

        enemyRender.sortingOrder = order;
        barBGRender.sortingOrder = order + 1;
        barRender.sortingOrder = order + 2;
        barHolderRender.sortingOrder = order + 3;

        sendToBackground();

		gameObject.SetActive(true);
        return enemy;
	}

	private void updateSprite () {
        enemyRender.sprite = ImagesProvider.getEnemy(enemy.type);// Imager.getEnemy(enemy.type, (float) enemy.health / (float) enemy.maxHealth);
	}

    public void sendToBackground () {
        enemyRender.sortingLayerName = backgroundLayerName;
        barRender.sortingLayerName = backgroundLayerName;
        barBGRender.sortingLayerName = backgroundLayerName;
        barHolderRender.sortingLayerName = backgroundLayerName;

        barHolder.gameObject.gameObject.SetActive(false);

        transform.localScale = smallScale;
    }

    public void sendToForeground () {
        enemyRender.sortingLayerName = foregroundLayerName;
        barRender.sortingLayerName = foregroundLayerName;
        barBGRender.sortingLayerName = foregroundLayerName;
        barHolderRender.sortingLayerName = foregroundLayerName;

        barHolder.gameObject.gameObject.SetActive(false);

        transform.localScale = smallScale;
    }

    public void setAsCurrentEnemy () {
        enemyRender.sortingLayerName = foregroundLayerName;
        barRender.sortingLayerName = foregroundLayerName;
        barBGRender.sortingLayerName = foregroundLayerName;
        barHolderRender.sortingLayerName = foregroundLayerName;

        barHolder.gameObject.gameObject.SetActive(true);

        transform.localScale = Vector3.one;
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