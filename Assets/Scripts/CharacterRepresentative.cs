using UnityEngine;
using System.Linq;
using System.Collections;
using System.Collections.Generic;

public abstract class CharacterRepresentative : MonoBehaviour {
    public Character character { get; protected set; }
    public abstract void setChosen (bool asChosen);
    public abstract void onHealModified ();

    public Dictionary<StatusEffectType, StatusEffectHolder> statusHolders { get; private set; }

    public List<StatusEffect> statuses { get; private set; }

    private float yOffset = -.33f, xOffset = .33f;

    protected void innerInit () {
        statusHolders = new Dictionary<StatusEffectType, StatusEffectHolder>();
        Transform statusesHolder = transform.Find("Statuses");
        StatusEffectHolder holder;
        statuses = new List<StatusEffect>();
        for (int i = 0; i < statusesHolder.childCount; i++) {
            holder = statusesHolder.GetChild(i).GetComponent<StatusEffectHolder>().init();
            statusHolders.Add(holder.type, holder);
        }
    }

    public void repositionStatuses () {
        statuses.Sort((x, y) => (x.addingTime-y.addingTime));
        for (int i = 0; i < statuses.Count; i++) {
            statuses[i].holder.transform.localPosition = new Vector3((Mathf.FloorToInt(i / 3) * xOffset), (i % 3) * yOffset, 0);
        }
    }
}