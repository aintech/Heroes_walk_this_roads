using UnityEngine;
using System.Collections;

public class QueuePortrait : MonoBehaviour {

    public Sprite background;

    public Sprite activeBackground;

	private SpriteRenderer backgroundRender;

    private SpriteRenderer portraitRender;

	private Transform holder;

	private float yOffset = -1.15f;

	private Vector3 newPosition = Vector3.zero;

	public QueuePortrait init (Transform holder) {
		this.holder = holder;
		backgroundRender = transform.Find("Background").GetComponent<SpriteRenderer>();
		portraitRender = transform.Find("Portrait").GetComponent<SpriteRenderer>();

		transform.SetParent(holder);
		transform.position = Vector3.zero;

        return this;
    }

	public QueuePortrait setCharacter (Character character, int index) {
		portraitRender.sprite = character.isHero()? ImagesProvider.getHeroQueue(((Hero)character).type): ImagesProvider.getEnemyQueue(((Enemy)character).type);
		newPosition.y = yOffset * index;
		transform.localPosition = newPosition;
		setAsActive(index == 0);
		return this;
	}

	public void disable (bool dis) {
		portraitRender.color = dis? new Color32(255, 255, 255, 100): new Color32(255, 255, 255, 255);
	}

	public void setAsActive (bool asActive) {
		backgroundRender.sprite = asActive? activeBackground: background;
	}
}