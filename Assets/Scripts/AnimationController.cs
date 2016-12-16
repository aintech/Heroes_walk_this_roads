using UnityEngine;
using System.Collections;

public class AnimationController : MonoBehaviour {
    
    public void animationComplete (EventType eventType) {
//        switch (eventType) {
//            case EventType.ENEMY_DEAD: FightScreen.ENEMY_DEAD_ANIM_DONE = true; break;
//        }
    }
}

public enum EventType {
    ENEMY_DEAD
}