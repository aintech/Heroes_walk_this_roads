using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

public abstract class Character {

	private const int initHealth = 100, healthPerEndurance = 10;

	public int health { get; private set; }

	public int maxHealth { get; private set; }

	public int strength { get; private set; }

	public int endurance { get; private set; }

	public int agility { get; private set; }

	public int initiative { get { return agility; } private set{;} }

	public int armorClass { get; protected set; }

    public CharacterRepresentative representative { get; private set; }

	public Dictionary<StatusEffectType, StatusEffect> statusEffects = new Dictionary<StatusEffectType, StatusEffect>();

    [HideInInspector]
    public bool moveDone;

	[HideInInspector]
	public bool guarded;

    public bool alive { get; private set; }

	public void innerInit (int strength, int endurance, int agility) {
		this.strength = strength;
		this.endurance = endurance;
		this.agility = agility;

		health = maxHealth = initHealth + endurance * healthPerEndurance;

        alive = true;

        Array statusTypes = Enum.GetValues(typeof (StatusEffectType));

        foreach (StatusEffectType type in statusTypes) {
            statusEffects.Add(type, new StatusEffect().init(type, this));
        }
	}

	public abstract int damage ();

	public int randomDamage () {
		float dmg = damage();
		return (int)UnityEngine.Random.Range(dmg - (Mathf.RoundToInt(dmg * .2f)), dmg + (Mathf.RoundToInt(dmg * .2f)));
	}

	public int hit (int damageAmount)  {
		int armorAmount = armorClass + (armorClass > 0? (guarded? Mathf.RoundToInt(damageAmount * .5f): 0): 0);

//		if (statusEffects[StatusEffectType.ARMORED].inProgress) {
//			armorAmount += statusEffects[StatusEffectType.ARMORED].value;
//		}

		if (damageAmount <= armorAmount) {
			return 0;
		} else {
			health -= (damageAmount - armorAmount);
            if (health <= 0) {
                health = 0;
                alive = false;
                foreach (StatusEffect status in statusEffects.Values) {
                    status.endEffect();
                }
            }
			representative.onHealModified();
			return damageAmount - armorAmount;
		}
	}

	public int heal (int amount) {
		if (amount + health > maxHealth) {
			int heal = maxHealth - health;
			setHealthToMax();
			representative.onHealModified();
			return heal;
		} else {
			health += amount;
			representative.onHealModified();
			return amount;
		}
	}

    public void reveal (bool toFullHealth) {
        alive = true;
        if (toFullHealth) { setHealthToMax(); }
        else { health = 1; }
    }

	public void setHealthToMax () {
		health = maxHealth;
	}

    public void refreshRepresentative (CharacterRepresentative representative) {
        this.representative = representative;
        representative.statuses.Clear();
        foreach (StatusEffect eff in statusEffects.Values) {
            if (representative.statusHolders.ContainsKey(eff.type)) {
                representative.statusHolders[eff.type].setStatusEffect(eff);
            }
            representative.statuses.Add(eff);
        }
    }

    public void addStatus (StatusEffectType type, int value, int duration) {
        statusEffects[type].addStatus(value, duration);
    }

    public void refreshStatuses () {
        foreach (StatusEffect status in statusEffects.Values) { status.updateStatus(); }
    }

    public void clearStatuses () {
        foreach (StatusEffect eff in statusEffects.Values) {
            eff.endEffect();
        }
    }

    public abstract bool isHero();

    public abstract string name();
}