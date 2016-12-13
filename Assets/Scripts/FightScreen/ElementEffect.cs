using UnityEngine;
using System.Collections;

public class ElementEffect : MonoBehaviour {

	public Sprite fireElement, waterElement, earthElement, airElement, lightElement, darkElement;

	public Sprite[] damageSprites;

	private static float enemyX = 4.2f;

	private SpriteRenderer element, effectHit;

	private Transform trans, emitter, BG, elementTrans, hitTextHolder;

	private ElementType type;

	private int value, elementsCount;

	private bool effectActive;

	private Vector3 trailRot = Vector3.zero, initScale = new Vector3(1.5f, 1.5f, 1), scale, newPos, enemyCenter = new Vector3(enemyX, 0, 0);

	private float scaleSpeed = .05f, moveSpeed = .4f, txtSpeed = .07f, maxTxtY;

	private Quaternion trailQuater = new Quaternion(), idleQuater = new Quaternion();

	private Step step = Step.APPEAR;

	private Enemy enemy;

	private int index, frameTime = 2, frameCounter;

	private FightInterface fightInterface;

	private FightScreen fightScreen;

	private TextMesh hitText, hitTextBG;

	public void init (FightScreen fightScreen, Enemy enemy) {
		this.fightScreen = fightScreen;
		this.enemy = enemy;
		trans = transform;
		elementTrans = trans.Find("EffectSprite");
		BG = trans.Find("BG");
		emitter = trans.Find("EffectTrail");
		element = elementTrans.GetComponent<SpriteRenderer>();
		effectHit = trans.Find("HitSprite").GetComponent<SpriteRenderer>();
		trans.SetParent(fightScreen.transform);
		fightInterface = fightScreen.transform.Find("Fight Interface").GetComponent<FightInterface>();
		hitTextHolder = trans.Find("HitTextHolder");
		hitText = hitTextHolder.Find("HitText").GetComponent<TextMesh>();
		hitTextBG = hitTextHolder.Find("HitTextBG").GetComponent<TextMesh>();
		MeshRenderer mesh = hitText.GetComponent<MeshRenderer>();
		mesh.sortingLayerName = "FightEffectLayer";
		mesh.sortingOrder = 4;
		mesh = hitTextBG.GetComponent<MeshRenderer>();
		mesh.sortingLayerName = "FightEffectLayer";
		mesh.sortingOrder = 3;
	}

	void Update () {
		if (effectActive) {
			if (trailRot.y > -90) {
				trailRot.y -= 1;
				trailQuater.eulerAngles = trailRot;
				emitter.transform.localRotation = trailQuater;
			}
			if (step == Step.APPEAR) {
				appearEffect();
			} else if (step == Step.MOVE) {
				moveEffect();
			} else if (step == Step.PLAY) {
				playEffect();
			}
		}
	}

	public void activateEffect (ElementType type, int value, Vector2 pos, int elementsCount) {
		this.type = type;
		this.value = value;
		this.elementsCount = elementsCount;
		trailRot = Vector3.zero;
		trailQuater = idleQuater;
		emitter.transform.localRotation = trailQuater;
		scale = Vector3.zero;
		setSprite();
		trans.position = pos;
		trans.localRotation = idleQuater;
		elementTrans.localRotation = idleQuater;
		newPos = trans.localPosition;
		effectActive = true;
		step = Step.APPEAR;
		index = 0;
		emitter.gameObject.SetActive(true);
		BG.gameObject.SetActive(true);
		elementTrans.gameObject.SetActive(true);
		gameObject.SetActive(true);
	}

	private void appearEffect () {
		if (scale.x < initScale.x) {
			scale.x += scaleSpeed;
			scale.y += scaleSpeed;
		} else {
			scale = initScale;
			step = Step.MOVE;
		}
		BG.localScale = scale;
		elementTrans.localScale = scale;
	}

	private void moveEffect () {
		if (trans.localPosition.x >= enemyX) {
			emitter.gameObject.SetActive(false);
			BG.gameObject.SetActive(false);
			elementTrans.gameObject.SetActive(false);
			trans.localPosition = enemyCenter;
			newPos = hitTextHolder.localPosition;
			maxTxtY = newPos.y + 2;
			setHitText();
			fightInterface.updateEnemyBar();
			enemy.playHit();
			effectHit.gameObject.SetActive(true);
			step = Step.PLAY;
		} else {
			elementTrans.Rotate(-Vector3.forward, 10);
			newPos.x += moveSpeed;
			trans.localPosition = newPos;
		}
	}
	
	private void setHitText () {
//		if (type == ElementType.GRANADE) {
//			StatusEffectType status = EnumDescriptor.getGranadeStatusType(iconsCount);
//			enemy.getStatusEffectHolder().addStatusEffect(status, 0, EnumDescriptor.getGranadeTurns(iconsCount));
//			FightMessenger.addStatusMessage(enemy.getEnemyType().getName(), status, 0, EnumDescriptor.getGranadeTurns(iconsCount));
//			hitTxt.text = status == StatusEffectType.SLOWED? "SLOWED":
//						  status == StatusEffectType.BLINDED? "BLINDED":
//						  status == StatusEffectType.PARALYZED? "PARALYZED": "";
//		} else {
		int damage = enemy.hitEnemy(value, elementsCount);
		hitText.text = "-" + damage;
		hitTextBG.text = "-" + damage;
//			FightMessenger.addDamageMessage(enemy.getEnemyType().getName(), EnumDescriptor.getShotType(iconsCount), damage);
//		}
	}

	private void playEffect () {
		if (index != -1) {
			animate(damageSprites);
//			switch (type) {
//				case ElementType.NORMAL_SHOT: animate(damageSprites); break;
//				case ElementType.SPREAD_SHOT: animate(damageSprites); break;
//				case ElementType.ARMOR_PIERCING_SHOT: animate(damageSprites); break;
//				case ElementType.GRANADE: animate(granadeSprites); break;
//				default: Debug.Log("ERROR"); break;
//			}
		}
		newPos.y += txtSpeed;
		if (newPos.y > maxTxtY) {
			deactivateEffect();
		} else {
			hitTextHolder.localPosition = newPos;
		}
	}

	private void animate (Sprite[] array) {
		if (frameCounter >= frameTime) {
			frameCounter = 0;
			index++;
			if (index >= array.Length-1) {
				index = -1;
				effectHit.gameObject.SetActive(false);
			} else {
				effectHit.sprite = array[index];
			}
		} else {
			frameCounter++;
		}
	}

	private void setSprite () {
		switch (type) {
			case ElementType.FIRE: element.sprite = fireElement; break;
			case ElementType.WATER: element.sprite = waterElement; break;
			case ElementType.EARTH: element.sprite = earthElement; break;
			case ElementType.AIR: element.sprite = airElement; break;
			case ElementType.LIGHT: element.sprite = lightElement; break;
			case ElementType.DARK: element.sprite = darkElement; break;
			default: Debug.Log("ERROR"); break;
		}
	}

	private void deactivateEffect () {
		hitText.text = null;
		hitTextBG.text = null;
		effectHit.sprite = null;
		index = 0;
		hitTextHolder.localPosition = Vector3.zero;
		effectActive = false;
		gameObject.SetActive(false);
		fightScreen.getFightProcessor().checkEffectsActive();
	}

	public bool isEffectActive () {
		return effectActive;
	}

	private enum Step {
		APPEAR, MOVE, PLAY
	}
}