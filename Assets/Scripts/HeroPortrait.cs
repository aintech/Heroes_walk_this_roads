using UnityEngine;
using System.Collections;

public class HeroPortrait : CharacterRepresentative {

	public Sprite normalBG, activeBG, noHeroBG;

	public HeroType type;

	private StatusScreen statusScreen;

	private SpriteRenderer portraitRender, backgroundRender;

	private Transform healthBar;

	private BoxCollider2D coll;

    private Vector3 healthScale = Vector3.one;

    private Hero hero;

	public HeroPortrait init () {
		return init (null);
	}

    public HeroPortrait init (StatusScreen statusScreen) {
		this.statusScreen = statusScreen;
        this.hero = hero;

		portraitRender = transform.Find ("Portrait").GetComponent<SpriteRenderer> ();
		backgroundRender = transform.Find ("Background").GetComponent<SpriteRenderer> ();

		healthBar = transform.Find ("Health Bar");

		coll = backgroundRender.GetComponent<BoxCollider2D> ();

		hero = Vars.heroes [type];

		portraitRender.sprite = ImagesProvider.getHeroPortrait(type);
		portraitRender.gameObject.SetActive (hero != null);
		healthBar.gameObject.SetActive (hero != null);
		coll.enabled = hero != null && statusScreen != null;
		backgroundRender.sprite = hero == null ? noHeroBG : normalBG;

		return this;
	}

    public void updateRepresentative () {
        Vars.heroes[type].representative = this;
    }

	public override void setChosen (bool asChosen) {
        backgroundRender.sprite = asChosen ? activeBG : normalBG;
	}

	void Update () {
		if (Input.GetMouseButtonDown (0) && Utils.hit != null && Utils.hit == coll) {
            statusScreen.chooseHero (type);
		}
	}

    public override void onHealModified () {
		healthScale.y = (float) hero.health / (float) hero.maxHealth;
        healthBar.localScale = healthScale;
    }
}