using UnityEngine;
using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;

public abstract class CharacterRepresentative : Describeable {

    public Character character { get; protected set; }

    public Dictionary<StatusEffectType, StatusEffectHolder> statusHolders { get; private set; }

    public List<StatusEffect> statuses { get; private set; }

	protected Collider2D coll;

	protected GameObject hoverBorder;

	private bool hovered;

    private float yOffset = -.33f, xOffset = .33f;

    private int beforeStatusesCount;

    protected SpriteRenderer imageRender;

    protected Color32 normalColor = new Color32(255, 255, 255, 255),
                      redColor = new Color32(255, 0, 0, 255),
                      grayColor = new Color32(100, 100, 100, 255),
                      tempColor = new Color32();

    private byte colorValue;

    private bool hitAnimationInProgress;

    protected Vector2 flyTextPoint;

	protected void innerInit () {
        statusHolders = new Dictionary<StatusEffectType, StatusEffectHolder>();
        Transform statusesHolder = transform.Find("Statuses");
        StatusEffectHolder holder;
        statuses = new List<StatusEffect>();
        for (int i = 0; i < statusesHolder.childCount; i++) {
            holder = statusesHolder.GetChild(i).GetComponent<StatusEffectHolder>().init();
            statusHolders.Add(holder.type, holder);
        }

		setHovered(false);
    }

    override protected void fillDescription () {
        descrId = Vars.describeableId++;
        descr.Add(character.name());
        descr.Add("");
        descr.Add("");
        updateHealthDescription();
        updateDamageDescription();
        beforeStatusesCount = descr.Count;
//        foreach (StatusEffectType statusType in Enum.GetValues(typeof(StatusEffectType))) {
//            statusEffectPositions.Add(statusType, beforeStatusesCount++);
//            descr.Add("");
//        }
    }

    public void repositionStatuses () {
        statuses.Sort((x, y) => (x.addingTime-y.addingTime));
        for (int i = 0; i < statuses.Count; i++) {
            statuses[i].holder.transform.localPosition = new Vector3((Mathf.FloorToInt(i / 3) * xOffset), (i % 3) * yOffset, 0);
        }
    }

    public void updateStatusEffects () {
        if (descr.Count > beforeStatusesCount) {
            descr.RemoveRange(beforeStatusesCount, descr.Count - beforeStatusesCount);
        }
        foreach (StatusEffect status in statuses) {
            if (status.isFired || status.inProgress) {
                descr.Add(status.type.color() + status.type.name() + "</color> - " + status.duration + (status.duration == 1? " ход": status.duration > 1 && status.duration < 5? " хода": " ходов"));
            }
        }
    }

	public void Update () {
        if (hitAnimationInProgress) {
            colorValue += (byte)(colorValue > 245? (255 - colorValue): 10);
            tempColor.g = colorValue;
            tempColor.b = colorValue;
            imageRender.color = tempColor;
            if (colorValue == 255) {
                hitAnimationInProgress = false;
                setAsActive(character.health > 0);
            }
        }
		if (Input.GetMouseButtonDown(0) && Utils.hit != null && Utils.hit == coll) {
			if (character.isHero() && StatusScreen.instance.gameObject.activeInHierarchy) {
				StatusScreen.instance.chooseHero (((HeroRepresentative)this).type);
			} else {
				if (FightProcessor.instance.heroAction != null) {
					if ((FightProcessor.instance.heroAction.targetType == TargetType.ALLY && character.isHero()) ||
						(FightProcessor.instance.heroAction.targetType == TargetType.ENEMY && !character.isHero())) {
						FightProcessor.instance.actionTargets = new Character[]{character};
					}
				}
			}
		}
		if (Utils.hit != null && Utils.hit == coll) {
			if (!hovered) {
				setHovered(true);
			}
		} else if (hovered) {
			setHovered(false);
		}
	}

	public void setHovered (bool hovered) {
		this.hovered = hovered;
		hoverBorder.SetActive(hovered);
	}

	protected void removeFromQueue () {
		FightProcessor.instance.removeFromQueue(character);
	}

	public abstract void setChosen (bool asChosen);
	public abstract void onHealModified ();

    protected void updateHealthDescription () {
        descr[1] = "HP: " + (character.health != character.maxHealth? (((float)character.health / (float)character.maxHealth) < .25f? "<color=red>": "<color=yellow>") + character.health + "</color>/": "") + "<color=#00FF00FF>" + character.maxHealth + "</color>";
        ItemDescriptor.instance.recheckValues(this);
    }

    protected void updateDamageDescription () {
        descr[2] = "Сила удара: <color=#00FF00FF>" + character.damage() + "</color>";
        ItemDescriptor.instance.recheckValues(this);
    }

    public void playDamage (int amount) {
        onHealModified();
        tempColor = new Color32(redColor.r, redColor.g, redColor.b, redColor.a);
        imageRender.color = tempColor;
        hitAnimationInProgress = true;
        colorValue = 0;
        FightScreen.instance.flyTextManager.fireText("-" + amount, ColorType.RED, flyTextPoint);
    }

    public void refreshColor () {
        imageRender.color = character.health > 0? normalColor: grayColor;
    }

    public void setAsActive (bool asActive) {
        imageRender.color = asActive? normalColor: grayColor;
    }
}