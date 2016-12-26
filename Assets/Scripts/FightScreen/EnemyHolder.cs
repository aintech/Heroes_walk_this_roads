using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class EnemyHolder : CharacterRepresentative {

    public Enemy character { get; private set; }

	private SpriteRenderer enemyRender, barRender, barBGRender, barHolderRender;

    private SpriteRenderer topStroke, leftStroke, rightStroke;

    private Transform healthBar, barHolder;

	private FightScreen fightScreen;

    private Vector3 initPosition = Vector3.zero, healthScale = Vector3.one;

    private Vector3 smallScale = new Vector3(.7f, .7f, 1);

    private string backgroundLayerName = "Fight Screen", foregroundLayerName = "Fight Screen Foreground";

	private PolygonCollider2D coll;

    private GameObject stroke;

    private bool stroked, chosen;

    public EnemyHolder init (FightScreen fightScreen) {
		this.fightScreen = fightScreen;
        enemyRender = GetComponent<SpriteRenderer>();

        Transform healthBarHolder = transform.Find("Health Bar");
        healthBar = healthBarHolder.Find("Bar");
        barHolder = healthBarHolder.Find("Holder");

        barRender = healthBar.GetComponent<SpriteRenderer>();
        barHolderRender = barHolder.GetComponent<SpriteRenderer>();
        barBGRender = healthBarHolder.Find("Background").GetComponent<SpriteRenderer>();

        stroke = transform.Find("Stroke").gameObject;
        topStroke = stroke.transform.Find("Top").GetComponent<SpriteRenderer>();
        leftStroke = stroke.transform.Find("Left").GetComponent<SpriteRenderer>();
        rightStroke = stroke.transform.Find("Right").GetComponent<SpriteRenderer>();

        stroke.SetActive(false);

        setAsActive(false);

        return this;
	}

    public Enemy initEnemy (EnemyType enemyType, float xPosition, int order) {
        initPosition.x = xPosition;
        character = new Enemy().init(enemyType);
        character.representative = this;
        updateSprite();
        transform.position = initPosition;

        topStroke.sortingOrder = order;
        leftStroke.sortingOrder = order;
        rightStroke.sortingOrder = order;

        enemyRender.sortingOrder = order + 1;
        barBGRender.sortingOrder = order + 2;
        barRender.sortingOrder = order + 3;
        barHolderRender.sortingOrder = order + 4;

        if (coll != null) {
            Destroy(coll);
        }
		coll = gameObject.AddComponent<PolygonCollider2D>();

        onHealModified();

        sendToBackground();

        return character;
	}

    void Update () {
        if (Input.GetMouseButtonDown(0) && Utils.hit != null && Utils.hit == coll) {
            if (fightScreen.fightProcessor.heroAction != null) {
                fightScreen.fightProcessor.actionTargets = new Character[]{character};
            }
        }
        if (Utils.hit != null && Utils.hit == coll) {
            if (!stroked) {
                setStroked(true);
            }
        } else if (stroked) {
            setStroked(false);
        }
    }

    private void setStroked (bool stroked) {
        this.stroked = stroked;
        stroke.SetActive(stroked);
    }

	private void updateSprite () {
        enemyRender.sprite = ImagesProvider.getEnemy(character.type);// Imager.getEnemy(enemy.type, (float) enemy.health / (float) enemy.maxHealth);
	}

    public void sendToBackground () {
        setAsActive(false);
        enemyRender.sortingLayerName = backgroundLayerName;
        barRender.sortingLayerName = backgroundLayerName;
        barBGRender.sortingLayerName = backgroundLayerName;
        barHolderRender.sortingLayerName = backgroundLayerName;

        barHolder.gameObject.gameObject.SetActive(false);

        transform.localScale = smallScale;
    }

    public void sendToForeground () {
        setAsActive(true);
        enemyRender.sortingLayerName = foregroundLayerName;
        barRender.sortingLayerName = foregroundLayerName;
        barBGRender.sortingLayerName = foregroundLayerName;
        barHolderRender.sortingLayerName = foregroundLayerName;

        barHolder.gameObject.gameObject.SetActive(false);

        transform.localScale = smallScale;
    }

    public void setAsCurrentEnemy () {
        setAsActive(true);
        enemyRender.sortingLayerName = foregroundLayerName;
        barRender.sortingLayerName = foregroundLayerName;
        barBGRender.sortingLayerName = foregroundLayerName;
        barHolderRender.sortingLayerName = foregroundLayerName;

        barHolder.gameObject.gameObject.SetActive(true);

        transform.localScale = Vector3.one;
    }

    public override void onHealModified () {
        if (character.health <= 0) {
            fightScreen.fightProcessor.removeFromQueue(character);
            gameObject.SetActive(false);
        } else {
            healthScale.x = (float)character.health / (float) character.maxHealth;
            healthBar.localScale = healthScale;
        }
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