using UnityEngine;
using System.Collections;

public class HeroPortrait : MonoBehaviour {

	public Sprite normalBG, activeBG, noHeroBG;

	public HeroType type;

	private StatusScreen statusScreen;

	private SpriteRenderer portraitRender, backgroundRender;

	private Transform healthBar;

	private BoxCollider2D coll;

	public HeroPortrait init () {
		return init (null);
	}

	public HeroPortrait init (StatusScreen statusScreen) {
		this.statusScreen = statusScreen;

		portraitRender = transform.Find ("Portrait").GetComponent<SpriteRenderer> ();
		backgroundRender = transform.Find ("Background").GetComponent<SpriteRenderer> ();

		healthBar = transform.Find ("Health Bar");

		coll = backgroundRender.GetComponent<BoxCollider2D> ();

		Hero hero = Vars.heroes [type];

		portraitRender.sprite = ImagesProvider.getHeroPortrait(type);
		portraitRender.gameObject.SetActive (hero != null);
		healthBar.gameObject.SetActive (hero != null);
		coll.enabled = hero != null && statusScreen != null;
		backgroundRender.sprite = hero == null ? noHeroBG : normalBG;

		return this;
	}

	public void choose (bool asActive) {
		backgroundRender.sprite = asActive ? activeBG : normalBG;
	}

	void Update () {
		if (Input.GetMouseButtonDown (0) && Utils.hit != null && Utils.hit == coll) {
			statusScreen.chooseHero (this);
		}
	}
}