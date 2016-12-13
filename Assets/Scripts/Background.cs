using UnityEngine;
using System.Collections;

public class Background : MonoBehaviour {

	private SpriteRenderer render;

	private Transform trans;

	private Vector3 pos;

	private float scrWidth, diff;

	private float xOffset;

	private bool moveBg;

	public Background init () {
		trans = transform;
		pos = trans.localPosition;
		render = GetComponent<SpriteRenderer>();
		scrWidth = Camera.main.ScreenToWorldPoint(new Vector2(Screen.width, Screen.height)).x;
//		diff = (Imager.getAreaBackground(AreaType.PLAINS).bounds.size.x - scrWidth * 2f) / 2f;
		gameObject.SetActive(true);
		moveBg = true;

		return this;
	}

	public void setBackground (AreaType type) {
		render.sprite = Imager.getAreaBackground(type);
//		followMouse();
	}

	public void setMoveBg (bool moveBg) {
		this.moveBg = moveBg;
	}

//	void Update () {
////		if (!fightScreen.isFightOver()) {
//		if (moveBg) { followMouse(); }
////		}
//	}
//
//	private void followMouse () {
//		xOffset = Utils.mousePos.x / scrWidth;
//		pos.x = -(xOffset < -1? -1: xOffset > 1? 1: xOffset) * diff;
//		trans.localPosition = pos;	
//	}
}