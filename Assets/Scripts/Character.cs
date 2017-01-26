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

    private Dictionary<StatusEffectType, StatusEffect> statusEffects = new Dictionary<StatusEffectType, StatusEffect>();

    [HideInInspector]
    public bool moveDone;

	[HideInInspector]
	public bool guardAbilityON;

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
        int armorAmount = armorClass;// + (armorClass > 0? (guarded? Mathf.RoundToInt(damageAmount * (HeroActionType.GUARD.value() * .01f)): 0): 0);
        int amount = damageAmount;
        if (isHero()) {
            if (statusEffects[StatusEffectType.HERO_HEAVY_GUARD].inProgress && armorAmount > 0) {
                armorAmount += Mathf.RoundToInt((float)armorAmount * .5f);
            } else if (statusEffects[StatusEffectType.HERO_INVULNERABILITY_SPHERE].inProgress) {
                statusEffects[StatusEffectType.HERO_INVULNERABILITY_SPHERE].endEffect();
                return 0;
            } else if (statusEffects[StatusEffectType.HERO_DODGE].inProgress) {
                if (UnityEngine.Random.value <= .25f) {
                    return 0;
                }
            } else if (statusEffects[StatusEffectType.HERO_SACRIFICE].inProgress) {
                Hero sacrHero = null;
                foreach (Hero hero in Vars.heroes.Values) {
                    if (hero.alive && hero.type != ((Hero)this).type) {
                        if (sacrHero == null) {
                            sacrHero = hero;
                        } else if (sacrHero.health < hero.health) {
                            sacrHero = hero;
                        }
                    }
                }
                if (sacrHero == null) {
                    statusEffects[StatusEffectType.HERO_SACRIFICE].endEffect();
                    hit(damageAmount);
                } else {
                    sacrHero.hit(Mathf.RoundToInt((float)damageAmount * .75f));
                    amount = Mathf.RoundToInt((float)damageAmount * .25f);
                }
            }
        }

//		if (statusEffects[StatusEffectType.ARMORED].inProgress) {
//			armorAmount += statusEffects[StatusEffectType.ARMORED].value;
//		}

		if (amount <= armorAmount) {
			return 0;
		} else {
			health -= (amount - armorAmount);
            if (health <= 0) {
                health = 0;
                alive = false;
                foreach (StatusEffect status in statusEffects.Values) {
                    status.endEffect();
                }
            }
            representative.playDamage(amount - armorAmount);
			return amount - armorAmount;
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

    public void addStatus (StatusEffectType type, int duration) {
        addStatus(type, 0, duration);
    }

    public void addStatus (StatusEffectType type, int value, int duration) {
        statusEffects[type].addStatus(value, duration);
        if (representative != null) { representative.updateStatusEffectsDescription(); }
    }

    public void refreshStatuses () {
        foreach (StatusEffect status in statusEffects.Values) { status.updateStatus(); }
        if (representative != null) { representative.updateStatusEffectsDescription(); }
    }

    public void clearStatuses () {
        foreach (StatusEffect eff in statusEffects.Values) {
            eff.endEffect();
        }
    }

    public abstract bool isHero();

    public abstract string name();
}