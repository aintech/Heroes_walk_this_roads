using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class EnemyRepresentative : CharacterRepresentative {

    public Enemy enemy { get; private set; }

    private SpriteRenderer barRender, barBGRender, barHolderRender;//enemyRender, 

    private SpriteRenderer topStroke, leftStroke, rightStroke;

    private Transform healthBar, barHolder;

	private Vector3 initPosition = new Vector3(0, -3.5f, 1), healthScale = Vector3.one;

    private Vector3 smallScale = new Vector3(.6f, .6f, 1), normalScale = new Vector3(.7f, .7f, 1);

    private string backgroundLayerName = "Fight Screen", foregroundLayerName = "Fight Screen Foreground";

    private bool stroked, chosen;

	public EnemyRepresentative init (Transform holder) {
		hoverBorder = transform.Find("Stroke").gameObject;
		innerInit();

        transform.SetParent(holder);

        imageRender = GetComponent<SpriteRenderer>();

        Transform healthBarHolder = transform.Find("Health Bar");
        healthBar = healthBarHolder.Find("Bar");
        barHolder = healthBarHolder.Find("Holder");

        barRender = healthBar.GetComponent<SpriteRenderer>();
        barHolderRender = barHolder.GetComponent<SpriteRenderer>();
        barBGRender = healthBarHolder.Find("Background").GetComponent<SpriteRenderer>();

		topStroke = hoverBorder.transform.Find("Top").GetComponent<SpriteRenderer>();
		leftStroke = hoverBorder.transform.Find("Left").GetComponent<SpriteRenderer>();
		rightStroke = hoverBorder.transform.Find("Right").GetComponent<SpriteRenderer>();

        setAsActive(false);

        return this;
	}

    public Enemy initEnemy (EnemyType enemyType, float xPosition, int order) {
        initPosition.x = xPosition;
        enemy = new Enemy().init(enemyType);
        character = enemy;
        character.refreshRepresentative(this);
        updateSprite();
        transform.position = initPosition;

        topStroke.sortingOrder = order;
        leftStroke.sortingOrder = order;
        rightStroke.sortingOrder = order;

        imageRender.sortingOrder = order + 1;
        barBGRender.sortingOrder = order + 2;
        barRender.sortingOrder = order + 3;
        barHolderRender.sortingOrder = order + 4;

        if (coll != null) {
            Destroy(coll);
        }
		coll = gameObject.AddComponent<PolygonCollider2D>();

        fillDescription();

        onHealModified();

        sendToBackground();

        flyTextPoint = barHolder.position;

        return enemy;
	}

    private void setStroked (bool stroked) {
        this.stroked = stroked;
		hoverBorder.SetActive(stroked);
    }

	private void updateSprite () {
        imageRender.sprite = ImagesProvider.getEnemy(enemy.type);// Imager.getEnemy(enemy.type, (float) enemy.health / (float) enemy.maxHealth);
	}

    public void sendToBackground () {
        setAsActive(false);
        imageRender.sortingLayerName = backgroundLayerName;
        barRender.sortingLayerName = backgroundLayerName;
        barBGRender.sortingLayerName = backgroundLayerName;
        barHolderRender.sortingLayerName = backgroundLayerName;

        barHolder.gameObject.gameObject.SetActive(false);

        transform.localScale = smallScale;
    }

    public void sendToForeground () {
        setAsActive(true);
        imageRender.sortingLayerName = foregroundLayerName;
        barRender.sortingLayerName = foregroundLayerName;
        barBGRender.sortingLayerName = foregroundLayerName;
        barHolderRender.sortingLayerName = foregroundLayerName;

        barHolder.gameObject.gameObject.SetActive(false);

        transform.localScale = smallScale;
    }

    public void setAsCurrentEnemy () {
        setAsActive(true);
        imageRender.sortingLayerName = foregroundLayerName;
        barRender.sortingLayerName = foregroundLayerName;
        barBGRender.sortingLayerName = foregroundLayerName;
        barHolderRender.sortingLayerName = foregroundLayerName;

        barHolder.gameObject.gameObject.SetActive(true);

        transform.localScale = normalScale;
    }

    public override void onHealModified () {
        if (character.health <= 0) {
			removeFromQueue();
            gameObject.SetActive(false);
        } else {
            healthScale.x = (float)character.health / (float) character.maxHealth;
            healthBar.localScale = healthScale;
        }
        updateHealthDescription();
    }

	public void playHit () {
//		animInAction = true;
//		endTime = Time.time + duration;
//		setSprite();
	}

    public override void setChosen (bool asChosen) {
//        throw new System.NotImplementedException ();
    }

    public void setAsActive (bool asActive) {
        enabled = asActive;
        if (coll != null) { coll.enabled = asActive; }
    }
}