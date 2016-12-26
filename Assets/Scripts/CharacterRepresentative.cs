using UnityEngine;
using System.Collections;

public abstract class CharacterRepresentative : MonoBehaviour {
    public abstract void setChosen (bool asChosen);
    public abstract void onHealModified ();
}