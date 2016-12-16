using UnityEngine;
using System.Collections;

public class FightEffectPlayer : MonoBehaviour {

	public Sprite damageSprite, healSprite, blindSprite, paralizeSprite, armorSprite, regenerateSprite, speedSprite, missSprite;

	private Transform bg, effectImage;

	private SpriteRenderer effectRender;

	private TextMesh effectTxt, effectTxtBG;

	private Vector3 scale = Vector3.one;

	private float scaleSpeed = .05f;

	private bool scaling, scaleBack;

	private bool effectIsPlaying;

	private Color red = new Color(1, 0, 0, 1), alfa = new Color(1, 1, 1, 1), green = new Color(0, 1, 0, 1), blue = new Color(0, 0, 1, 1);

    private Quaternion idleRot = new Quaternion();

	private Vector3 bgRotSpeed = new Vector3(0, 0, -2);

	private float playStart, playTime = .5f;

	private bool damageEffect;

	private float damageAppearTime, damageEffectDuration = .2f;

	public FightEffectPlayer init () {
		bg = transform.FindChild("BG");
		bg.GetComponent<SpriteRenderer>().enabled = true;
		effectImage = transform.FindChild("EffectImage");
		effectRender = effectImage.GetComponent<SpriteRenderer>();
		effectTxt = transform.FindChild("ValueTxt").GetComponent<TextMesh>();
		effectTxtBG = transform.FindChild("ValueTxtBG").GetComponent<TextMesh>();
		MeshRenderer rend = effectTxt.transform.GetComponent<MeshRenderer>();
		rend.sortingLayerName = "FightEffectLayer";
		rend.sortingOrder = 4;
		rend = effectTxtBG.transform.GetComponent<MeshRenderer>();
		rend.sortingLayerName = "FightEffectLayer";
		rend.sortingOrder = 3;
		bg.gameObject.SetActive(false);
		effectTxt.gameObject.SetActive(false);

		gameObject.SetActive(true);

		return this;
	}

	void Update () {
		if (effectIsPlaying) {
			if (damageEffect) {
				if (!scaleBack) {
					scaleBack = true;
					damageAppearTime = Time.time + damageEffectDuration;
				} else {
					if (damageAppearTime < Time.time) {
						alfa.a -= .03f;
						if (alfa.a <= 0) {
							alfa.a = 1;
							endPlay();
						}
					}
				}
				effectRender.color = alfa;
			} else {
				bg.Rotate(bgRotSpeed);
				if (scaling && scale.x >= 1) {
					scaling = false;
					playStart = Time.time;
				}
				if (scaling) {
					scale.x += scaleSpeed;
					scale.y += scaleSpeed;
					effectImage.localScale = scale;
				} else if (!scaleBack) {
					if (Time.time > playStart + playTime) {
						scaleBack = true;
					}
				} else {
					if (scale.x <= 0) {
						endPlay();
					}
					scale.x -= scaleSpeed * 2;
					scale.y -= scaleSpeed * 2;
					effectImage.localScale = scale;
				}
			}
		}
	}

	/*
	 * Вместо медкита и армора - эффект от разных зелий
	 */

//	public void playMedkitEffect (MedkitElementType type, int value) {
//		switch(type) {
//			case MedkitElementType.SMALL: effectRender.sprite = medkitSmallSprite; break;
//			case MedkitElementType.MEDIUM: effectRender.sprite = medkitMediumSprite; break;
//			case MedkitElementType.LARGE: effectRender.sprite = medkitLargeSprite; break;
//			case MedkitElementType.SUPERIOR: effectRender.sprite = medkitSuperiorSprite; break;
//		}
//		effectTxt.color = green;
//		effectTxt.text = "+" + value;
//		effectSetup();
//	}
//
//	public void playArmorEffect (ArmorElementType type, int value) {
//		switch (type) {
//			case ArmorElementType.LIGHT: effectRender.sprite = armorLightSprite; break;
//			case ArmorElementType.MODERATE: effectRender.sprite = armorModerateSprite; break;
//			case ArmorElementType.HEAVY: effectRender.sprite = armorHeavySprite; break;
//			case ArmorElementType.ULTRA: effectRender.sprite = armorUltraSprite; break;
//		}
//
//		effectTxt.color = blue;
//		effectTxt.text = "+" + value;
//		effectSetup();
//	}

	public void playEffectOnEnemy (FightEffectType type, int value) {
		playEffect (type, value);
	}

	public void playEffect (FightEffectType type, int value) {
		damageEffect = type == FightEffectType.DAMAGE;
		switch (type) {
			case FightEffectType.HEAL:
				effectRender.sprite = healSprite;
				effectTxt.color = green;
				effectTxt.text = "Здоровье +" + value;
				break;
			case FightEffectType.REGENERATION:
				effectRender.sprite = regenerateSprite;
				effectTxt.color = green;
				effectTxt.text = "Регенерация";
				break;
			case FightEffectType.ARMORED:
				effectRender.sprite = armorSprite;
				effectTxt.color = blue;
				effectTxt.text = "Защита +" + value;
				break;
			case FightEffectType.SPEED:
				effectRender.sprite = speedSprite;
				effectTxt.color = blue;
				effectTxt.text = "Ускорение";
				break;
			case FightEffectType.BLIND:
				effectRender.sprite = blindSprite;
				effectTxt.color = red;
				effectTxt.text = "Ослепление";
				break;
			case FightEffectType.DAMAGE:
				effectRender.sprite = damageSprite;
				effectTxt.color = red;
				effectTxt.text = "-" + value;
				break;
			case FightEffectType.PARALIZED:
				effectRender.sprite = paralizeSprite;
				effectTxt.color = red;
				effectTxt.text = "Паралич";
				break;
            case FightEffectType.MISS:
                effectRender.sprite = missSprite;
				effectTxt.color = red;
				effectTxt.text = "Промах";
				break;
			default: Debug.Log("Unknown effect type: " + type); break;
		}

		effectSetup();
	}

	private void effectSetup () {
		if (!damageEffect) {
			scaling = true;
			
			scale.x = .1f;
			scale.y = .1f;
			
			effectImage.localScale = scale;
		}
		effectIsPlaying = true;
		effectTxtBG.text = effectTxt.text;

//		bg.gameObject.SetActive(!damageEffect);
		effectImage.gameObject.SetActive(true);
		effectTxt.gameObject.SetActive(true);
		effectTxtBG.gameObject.SetActive(true);
	}

	private void endPlay () {
		effectIsPlaying = false;
		damageEffect = false;
		scaleBack = false;
		scale.x = scale.y = 1;
		effectImage.localScale = scale;
		bg.localRotation = idleRot;
		bg.gameObject.SetActive(false);
		effectImage.gameObject.SetActive(false);
		effectTxt.gameObject.SetActive(false);
		effectTxtBG.gameObject.SetActive(false);
		FightProcessor.FIGHT_ANIM_PLAYER_DONE = true;
	}
}