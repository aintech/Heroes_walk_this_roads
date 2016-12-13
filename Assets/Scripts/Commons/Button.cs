using UnityEngine;
using System.Collections;

public class Button : MonoBehaviour {
	
	public Sprite normal, hover;

	public Color32 normalTextColor, hoverTextColor;

	private Color32 normalColor = new Color32(255, 255, 255, 255), notActiveColor = new Color32(255, 255, 255, 150);

	private SpriteRenderer render;
	
	private Collider2D coll;
	
	private TextMesh text;

	private MeshRenderer textRender;

	private State state = State.NORMAL;
	
	private ButtonHolder holder;

	private bool active = true, visible = true;

	private bool hideableText;

	public Button init () {
		render = GetComponent<SpriteRenderer>();
		coll = GetComponent<Collider2D>();
		if (holder == null) { holder = transform.parent.GetComponent<ButtonHolder>(); }
		text = transform.Find("BtnText").GetComponent<TextMesh>();
		textRender = text.GetComponent<MeshRenderer>();
		textRender.sortingLayerName = render.sortingLayerName;
		textRender.sortingOrder = render.sortingOrder + 1;

		gameObject.SetActive(true);

		return this;
	}

	public Button init (bool hideableText) {
		this.hideableText = hideableText;
		return init();
	}

	public Button initWithHolder (ButtonHolder holder) {
		this.holder = holder;
		return init();
	}

	void Update () {
		if (!active || !visible) { return; }
		if (Utils.hit != null && Utils.hit == coll) {
			if (state == State.NORMAL) {
				changeState(State.HOVER);
			}
			if (Input.GetMouseButtonDown(0)) {
				holder.fireClickButton(this);
			}
		} else if (Utils.hit != null && Utils.hit != coll && state == State.HOVER) {
			changeState(State.NORMAL);
		} else if (Utils.hit == null && state == State.HOVER) {
			changeState(State.NORMAL);
		}
	}
	
	private void changeState (State state) {
		this.state = state;
		switch (state) {
			case State.NORMAL:
				render.sprite = normal;
				text.color = normalTextColor;
				if (hideableText) { textRender.gameObject.SetActive(false); }
				break;
			case State.HOVER:
				render.sprite = hover;
				text.color = hoverTextColor;
				if (hideableText) { textRender.gameObject.SetActive(true); }
				break;
		}
	}

	public void setVisible (bool visible) {
		this.visible = visible;
		render.enabled = visible;
		coll.enabled = visible && active;
		textRender.enabled = visible;
	}

	public void setActive (bool active) {
		this.active = active;
		coll.enabled = active && visible;
		if (!active) { changeState(State.NORMAL); }
		render.color = active? normalColor: notActiveColor;
		text.color = active? normalTextColor: notActiveColor;
	}

	public void setText (string text) {
		this.text.text = text;
	}

	private enum State {
		NORMAL, HOVER
	}
}