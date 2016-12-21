using UnityEngine;
using System.Collections;

public class ElementsPool : MonoBehaviour {

    public ElementsPool init () {
        return this;
    }

    public void addElements () {
        Debug.Log("Add elements to Elements Pool");
    }
}