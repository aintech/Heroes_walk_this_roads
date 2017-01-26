using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class EnemyRepresentative : CharacterRepresentative {

    public Enemy enemy { get; private set; }

    private SpriteRenderer barRender, barBGRender, barHolderRender;//enemyRender, 

    private SpriteRenderer topStroke, leftStroke, rightStroke;

    private Transform healthBar, barHolder;

	private Vector3 initPosition = new Vector3(0, -3.5f, 1), healthScale = Vector3.one;

    private Vector3 smallScale = new Vector3(.6f, .6f, 1), normalScale = new Vector3(.7f, .7f, 1), smallScaleMirror, normalScaleMirror;

    private bool mirrorImage;

    private string backgroundLayerName = "Fight Screen", foregroundLayerName = "Fight Screen Foreground";

    private bool stroked, chosen;

    public EnemyRepresentativeAnimator animator { get; private set; }

	public EnemyRepresentative init (Transform holder) {
		hoverBorder = transform.Find("Stroke").gameObject;

		innerInit();

        smallScaleMirror = new Vector3(-smallScale.x, smallScale.y, smallScale.z);
        normalScaleMirror = new Vector3(-normalScale.x, normalScale.y, normalScale.z);

        animator = GetComponent<EnemyRepresentativeAnimator>().init();

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

        foreach (StatusEffect status in statuses) {
            if (status.holder != null) { status.holder.setRenderProperties(backgroundLayerName, order + 5); }
        }

        if (coll != null) {
            Destroy(coll);
        }
		coll = gameObject.AddComponent<PolygonCollider2D>();

        fillDescription();

        onHealModified();

        mirrorImage = Random.value > .5f;

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
        setColliderEnabled(false);
        imageRender.sortingLayerName = backgroundLayerName;
        barRender.sortingLayerName = backgroundLayerName;
        barBGRender.sortingLayerName = backgroundLayerName;
        barHolderRender.sortingLayerName = backgroundLayerName;

        foreach (StatusEffect status in statuses) {
            if (status.holder != null) { status.holder.setRenderLayer(backgroundLayerName); }
        }

        barHolder.gameObject.gameObject.SetActive(false);

        transform.localScale = mirrorImage? smallScaleMirror: smallScale;
    }

    public void sendToForeground () {
        setColliderEnabled(true);
        imageRender.sortingLayerName = foregroundLayerName;
        barRender.sortingLayerName = foregroundLayerName;
        barBGRender.sortingLayerName = foregroundLayerName;
        barHolderRender.sortingLayerName = foregroundLayerName;

        barHolder.gameObject.gameObject.SetActive(false);

        foreach (StatusEffect status in statuses) {
            if (status.holder != null) { status.holder.setRenderLayer(foregroundLayerName); }
        }

        transform.localScale = mirrorImage? smallScaleMirror: smallScale;
    }

    public void setAsCurrentEnemy () {
        setColliderEnabled(false);
        imageRender.sortingLayerName = foregroundLayerName;
        barRender.sortingLayerName = foregroundLayerName;
        barBGRender.sortingLayerName = foregroundLayerName;
        barHolderRender.sortingLayerName = foregroundLayerName;

        barHolder.gameObject.gameObject.SetActive(true);

        foreach (StatusEffect status in statuses) {
            if (status.holder != null) { status.holder.setRenderLayer(foregroundLayerName); }
        }

        transform.localScale = mirrorImage? normalScaleMirror: normalScale;
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

//    public void setAsActive (bool asActive) {
//        enabled = asActive;
//        if (coll != null) { coll.enabled = asActive; }
//    }
}