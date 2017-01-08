using UnityEngine;
using System.Collections;

public class HeroRepresentative : CharacterRepresentative {

	public Sprite normalBG, activeBG, noHeroBG;

	public HeroType type;

	private StatusScreen statusScreen;

	private SpriteRenderer portraitRender, backgroundRender, healthRender;

	private Transform healthBar;

    private Vector3 healthScale = Vector3.one;

    private Hero hero;

    private Color32 full = new Color32(0, 255, 0, 255), wounded = new Color32(255, 255, 0, 255), critical = new Color32(255, 0, 0, 255);

    private Color32 normalColor = new Color32(255, 255, 255, 255), transparColor = new Color32(255, 255, 255, 100);

    public HeroRepresentativeAnimator animator { get; private set; }

	public HeroRepresentative init () {
		statusScreen = StatusScreen.instance;
		hoverBorder = transform.Find("Hover Border").gameObject;

		innerInit();

        animator = GetComponent<HeroRepresentativeAnimator>().init();

		portraitRender = transform.Find ("Portrait").GetComponent<SpriteRenderer> ();
		backgroundRender = transform.Find ("Background").GetComponent<SpriteRenderer> ();

		healthBar = transform.Find ("Health Bar");
        healthRender = healthBar.GetComponent<SpriteRenderer>();

		coll = backgroundRender.GetComponent<BoxCollider2D> ();

		hero = Vars.heroes [type];
        character = hero;

		portraitRender.sprite = ImagesProvider.getHeroRepresentative(type);
		portraitRender.gameObject.SetActive (hero != null);
		healthBar.gameObject.SetActive (hero != null);
		coll.enabled = hero != null && statusScreen != null;
		backgroundRender.sprite = hero == null ? noHeroBG : normalBG;

		return this;
	}

    public void updateRepresentative () {
        Vars.heroes[type].refreshRepresentative(this);
        setAsActive(hero.health > 0);
    }

	public override void setChosen (bool asChosen) {
        backgroundRender.sprite = asChosen ? activeBG : normalBG;
	}

//	void Update () {
//		if (Input.GetMouseButtonDown (0) && Utils.hit != null && Utils.hit == coll) {
//            statusScreen.chooseHero (type);
//		}
//	}

    public override void onHealModified () {
        if (hero.health <= 0) {
            healthScale.y = 0;
        } else {
		    healthScale.y = (float) hero.health / (float) hero.maxHealth;
        }
        healthRender.color = hero.health == hero.maxHealth? full: ((float)hero.health / (float)hero.maxHealth) <= .25f? critical: wounded;
        healthBar.localScale = healthScale;
    }

    public void setAsActive (bool asActive) {
        portraitRender.color = asActive? normalColor: transparColor;
    }
}