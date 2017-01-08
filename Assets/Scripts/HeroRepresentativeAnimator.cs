using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class HeroRepresentativeAnimator : MonoBehaviour {

    public Texture2D damageSheet;

    private const int SPRITE_WIDTH = 110;

    private const int SPRITE_HEIGHT = 130;

    private static List<Sprite> damage = new List<Sprite>();

    private SpriteRenderer render;

    private List<Sprite> currPlaying;

    private int playIndex = -1;

    private float playSpeed = .1f;

    private float nextFrameTime;

    private bool inPlaying;

    public HeroRepresentativeAnimator init () {
        if (damage.Count == 0) {
            loadSpriteSheets();
        }

        render = transform.Find("Hero Effect").GetComponent<SpriteRenderer>();
        enabled = false;

        return this;
    }

    private void loadSpriteSheets () {
        Rect rect = new Rect(0, 0, SPRITE_WIDTH, SPRITE_HEIGHT);
        Vector2 pivot = new Vector2(.5f, .5f);

        int spritesCount = Mathf.RoundToInt((float)damageSheet.width / (float)SPRITE_WIDTH);

        for (int i = 0; i < spritesCount; i++) {
            rect.x = SPRITE_WIDTH * i;
            damage.Add(Sprite.Create(damageSheet, rect, pivot));
        }
    }

    public void playAnimation (AnimationType type) {
        if (inPlaying) { finishAnimation(); }
        switch (type) {
            case AnimationType.DAMAGE: currPlaying = damage; break;
            default: Debug.Log("Animation not done yet!"); break;
        }
        inPlaying = true;
        playIndex = -1;
        enabled = true;
    }

    private void finishAnimation () {
        inPlaying = false;
        enabled = false;
        currPlaying = null;
        render.sprite = null;
    }

    void Update () {
        if (inPlaying) {
            if (nextFrameTime <= Time.time) {
                playIndex++;
                nextFrameTime = Time.time + playSpeed;
                if (playIndex == currPlaying.Count) {
                    finishAnimation();
                } else {
                    render.sprite = currPlaying[playIndex];
                }
            }
        }
    }

    public enum AnimationType {
        DAMAGE
    }
}