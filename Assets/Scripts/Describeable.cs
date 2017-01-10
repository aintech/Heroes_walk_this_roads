using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public abstract class Describeable : MonoBehaviour {
    protected long descrId = -1;
    protected List<string> descr = new List<string>();

    abstract protected void fillDescription ();

    virtual public long descriptionId () { return descrId; }
    virtual public List<string> description () { return descr; }
}