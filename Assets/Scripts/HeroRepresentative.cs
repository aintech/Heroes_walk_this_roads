using UnityEngine;
using System.Collections;

public class HeroRepresentative : CharacterRepresentative {

	public Sprite normalBG, activeBG, noHeroBG;

	public HeroType type;

	private StatusScreen statusScreen;

    private SpriteRenderer backgroundRender, healthRender;//portraitRender, 

	private Transform healthBar;

    private Vector3 healthScale = Vector3.one;

    private Hero hero;

    private Color32 full = new Color32(0, 255, 0, 255), wounded = new Color32(255, 255, 0, 255), critical = new Color32(255, 0, 0, 255);

    public HeroRepresentativeAnimator animator { get; private set; }

    public HeroRepresentative init (string layerName) {
		statusScreen = StatusScreen.instance;
		hoverBorder = transform.Find("Hover Border").gameObject;

		innerInit();

        animator = GetComponent<HeroRepresentativeAnimator>().init();

        imageRender = transform.Find ("Portrait").GetComponent<SpriteRenderer> ();
		backgroundRender = transform.Find ("Background").GetComponent<SpriteRenderer> ();

		healthBar = transform.Find ("Health Bar");
        healthRender = healthBar.GetComponent<SpriteRenderer>();

		coll = GetComponent<BoxCollider2D> ();

		hero = Vars.heroes [type];
        character = hero;

        imageRender.sprite = ImagesProvider.getHeroRepresentative(type);
        imageRender.gameObject.SetActive (hero != null);
		healthBar.gameObject.SetActive (hero != null);
		coll.enabled = hero != null && statusScreen != null;
		backgroundRender.sprite = hero == null ? noHeroBG : normalBG;

        flyTextPoint = transform.position;

        fillDescription();

        rearrangeOnLayer(layerName);

		return this;
	}

    private void rearrangeOnLayer (string layerName) {
        foreach (SpriteRenderer render in transform.GetComponentsInChildren<SpriteRenderer>()) {
            render.sortingLayerName = layerName;
        }
    }

    public void updateRepresentative () {
        Vars.heroes[type].refreshRepresentative(this);
        setAsActive(hero.health > 0);
        updateDamageDescription();
    }

	public override void setChosen (bool asChosen) {
        backgroundRender.sprite = asChosen ? activeBG : normalBG;
	}

    public override void onHealModified () {
        if (hero.health <= 0) {
//            setAsActive(false);
            removeFromQueue();
            healthScale.y = 0;
        } else {
		    healthScale.y = (float) hero.health / (float) hero.maxHealth;
        }
        healthRender.color = hero.health == hero.maxHealth? full: ((float)hero.health / (float)hero.maxHealth) <= .25f? critical: wounded;
        healthBar.localScale = healthScale;
        updateHealthDescription();
    }
}