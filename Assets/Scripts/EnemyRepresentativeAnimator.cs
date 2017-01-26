using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class EnemyRepresentativeAnimator : MonoBehaviour {

    public Texture2D slashSheet;

    private const int SPRITE_WIDTH = 512;

    private const int SPRITE_HEIGHT = 512;

    private static List<Sprite> slash = new List<Sprite>();

    private SpriteRenderer render;

    private List<Sprite> currPlaying;

    private int playIndex = -1;

    private float playSpeed = .05f;

    private float nextFrameTime;

    private bool inPlaying;

    public EnemyRepresentativeAnimator init () {
        if (slash.Count == 0) { loadSpriteSheets(); }
        render = transform.Find("Effect Player Holder").GetComponent<SpriteRenderer>();
        enabled = false;

        return this;
    }

    private void loadSpriteSheets () {
        Rect rect = new Rect(0, 0, SPRITE_WIDTH, SPRITE_HEIGHT);
        Vector2 pivot = new Vector2(.5f, .5f);

        int spritesCount = Mathf.RoundToInt((float)slashSheet.width / (float)SPRITE_WIDTH);

        for (int i = 0; i < spritesCount; i++) {
            rect.x = SPRITE_WIDTH * i;
            slash.Add(Sprite.Create(slashSheet, rect, pivot));
        }
    }

    public void playAnimation (AnimationType type) {
        if (inPlaying) { finishAnimation(); }
        switch (type) {
            case AnimationType.SLASH: currPlaying = slash; break;
            default: Debug.Log("Animation not done yet!"); break;
        }
        render.transform.Rotate(new Vector3(0, 0, Random.Range(0, 360)));
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
        SLASH
    }
}