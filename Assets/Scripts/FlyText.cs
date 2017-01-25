using UnityEngine;
using System.Collections;

public class FlyText : MonoBehaviour {

    private StrokeText text;

    public bool fired { get; private set; }

    private Transform trans;

    private Vector3 transPos;

//    private float uprisingTo, uprisingSpeed = .05f;

    private float travelSpeed = .05f, traveledDistance, maxDistance = 1.3f;

    private Vector2 targetPoint;

    private float addX, addY;

    public FlyText init () {
        trans = transform;
        text = transform.Find("Text").GetComponent<StrokeText>().init("Fight Interface", 50);
        gameObject.SetActive(false);
        return this;
    }

    public void fire (string message, Color32 color, Vector2 pos) {
        text.text = message;
        text.color = color;
        transform.position = pos;
        fired = true;
        transPos = pos;
        trans.position = transPos;
        traveledDistance = 0;
//        uprisingTo = transPos.y + 1.5f;
        findTargetPoint();
        gameObject.SetActive(true);
    }

    private void findTargetPoint () {
        if (traveledDistance >= maxDistance) {
            stop();
        } else {
            addX = UnityEngine.Random.Range(-.2f, .2f);
            addY = UnityEngine.Random.Range(.3f, .8f);
            targetPoint.x = transPos.x + addX;
            targetPoint.y = transPos.y + addY;
            traveledDistance += Mathf.Sqrt(addX * addX + addY * addY);
        }
    }

    private void stop () {
        fired = false;
        gameObject.SetActive(false);
    }

    void Update () {
        if (fired) {
            transPos.x += (addX * travelSpeed);
            transPos.y += (addY * travelSpeed);
//            transPos = Vector2.Lerp(transPos, targetPoint, travelSpeed);
            trans.position = transPos;
            if (Vector2.Distance(transPos, targetPoint) < .05f) {
                findTargetPoint();
            }
//            transPos.y += uprisingSpeed;
//            trans.position = transPos;
//            if (transPos.y >= uprisingTo) {
//                stop();
//            }
        }
    }
}