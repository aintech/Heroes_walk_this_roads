using UnityEngine;
using System.Linq;
using System.Collections;
using System.Collections.Generic;

public class StatusEffect {

    public StatusEffectType type { get; private set; }

	public int value { get; private set; }

	public int duration { get; private set; }

	public bool inProgress { get; private set; }

	public bool isFired { get; private set; }

    public StatusEffectHolder holder;

    public int addingTime { get; private set; }

    private Character owner;

    public bool isSpecialStatus { get; private set; }

    public StatusEffect init (StatusEffectType type, Character owner) {
        this.type = type;
        this.owner = owner;
		inProgress = false;
        addingTime = int.MaxValue;
		return this;
	}

	public void addStatus (int value, int duration) {
        if (isFired || inProgress) {
            this.duration += duration;
        } else {
    		this.value = value;
    		this.duration = duration;
            isSpecialStatus = value == 0 && duration == 0;
    		isFired = true;
            inProgress = isSpecialStatus;
            addingTime = (int)Time.time;
            owner.representative.repositionStatuses();
            if (holder != null) { holder.show(); }
        }
	}

	public void updateStatus () {
        if (isSpecialStatus) { return; }
		if (isFired && !inProgress) {
			inProgress = true;
            if (holder != null) { holder.setAsEnabled(); }
		}
		if (!inProgress) {
			return;
		}
		duration--;
		if (duration >= 0) {
			applyEffect ();
		}
//		if (duration == 1) { turnsText.gameObject.SetActive (false); } else
		if (duration == 0 && !type.isStatusActiveOnNextTurn()) { endEffect (); }
		else if (duration < 0 && type.isStatusActiveOnNextTurn()) { endEffect(); }

//		turnsText.setText (duration.ToString ());
	}

	private void applyEffect () {
//		switch (statusType) {
//			case StatusEffectType.REGENERATION:
//				if (asPlayer) {
//					Player.heal (value);
//				} else {
//                    enemy.enemy.heal (value);
//				}
//				break;
//		}
	}

	public void endEffect () {
		inProgress = false;
		isFired = false;
        holder.hide();
        addingTime = int.MaxValue;
        owner.representative.repositionStatuses();
	}
}