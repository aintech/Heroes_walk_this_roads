using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class MessageBox : MonoBehaviour {
	
	public GUIStyle msgStyle, btnStyle;
	
	private GUIStyle appearMsgStyle;
	
	private MessageContainer messageContainer;
	
	private MessageContainer.MessageObject messageObject;
	
	private Rect scrollViewRect, msgRect, btnRect;
	
	private const float screenTopOffsetRate = .03f /*отступ сверху*/, textAreaHeightRate = .95f /*Высота текстового блока*/,
						screenLeftOffsetRate = .085f, textAreaWidthRate = .83f,
						btnSpacing = 2;

	private const float scrollSpeed = 20;
	
	private Vector2 scrollPos = Vector2.zero;
	
	private List<ContentWrapper> contents = new List<ContentWrapper>();
	
	private float contentWidth, contentHeight, btnHeight = 30, nextObjY, msgToBtnOffset, msgStartY, msgMaxY, btnLowerY;
	
	private RectOffset rectOffset;
	
	private bool needToScrollToLast = false;
	
	private Color appearColor;
	
	private static bool hided = false;

    private TownMainScreen townScreen;

    public void init (TownMainScreen townScreen) {
        this.townScreen = townScreen;
		float yPos = Screen.height * screenTopOffsetRate;
		float xPos = Screen.width * screenLeftOffsetRate;
		Vector2 pos = new Vector2(xPos, yPos);
		float textWidth = Screen.width * textAreaWidthRate;
		float textHeight = Screen.height * textAreaHeightRate;
		Vector2 size = new Vector2(textWidth, textHeight);
		msgRect = new Rect(pos, size);
		btnRect = new Rect (pos, size);
		scrollViewRect = new Rect(pos, size);
		btnRect.height = btnHeight;
		contentWidth = size.x;
		msgToBtnOffset = textHeight * .03f;
		msgStartY = msgRect.y;
		msgMaxY = msgStartY;
		transform.Find("BG").gameObject.SetActive(true);
		appearMsgStyle = new GUIStyle(msgStyle);
		appearColor = appearMsgStyle.normal.textColor;
		appearColor.a = 0;
		btnLowerY = Screen.height - (Screen.height * screenTopOffsetRate) - btnHeight;
		hideMessageBox();
	}
	
	void OnGUI () {
		if (messageObject == null) {
			return;
		}
		msgRect.y = msgStartY;
		GUI.BeginScrollView(scrollViewRect, scrollPos, scrollViewRect);
		foreach (ContentWrapper wrapper in contents) {
			if (wrapper.isAppearing()) {
				GUI.Label(msgRect, wrapper.getContent(), appearMsgStyle);
				appearColor.a += .02f;
				appearMsgStyle.normal.textColor = appearColor;
				if (appearColor.a >= 1) {
					wrapper.setAppearing(false);
				}
			} else {
				GUI.Label(msgRect, wrapper.getContent(), msgStyle);
			}
			msgRect.y += wrapper.getContentHeight();
		}
//		btnRect.y = msgRect.y + msgToBtnOffset;
		btnRect.y = btnLowerY;
		MessageContainer.ButtonObject btn;
		for (int i = messageObject.getButtons().Length-1; i >= 0; i--) {
			if (messageObject == null) { break; }
			btn = messageObject.getButtons()[i];
			if (GUI.Button(btnRect, btn.getBtnText(), btnStyle)) {
				clickButton (btn);
			}
			btnRect.y -= btnHeight + btnSpacing;
		}
//		foreach (MessageContainer.ButtonObject btn in messageObject.getButtons()) {
//			if (GUI.Button(btnRect, btn.getBtnText(), btnStyle)) {
//				clickButton (btn);
//			}
//			btnRect.y -= btnHeight + btnSpacing;
//		}
		GUI.EndScrollView();
	}
	
	void Update () {
		if (Input.mouseScrollDelta.y < 0) {
			needToScrollToLast = false;
			scrollMsg (false);
		} else if (Input.mouseScrollDelta.y > 0) {
			needToScrollToLast = false;
			scrollMsg (true);
		}
		if (needToScrollToLast) {
			scrollMsg(false, scrollSpeed * .5f);
		}
	}
	
	private void addContentWrapper (string txt, Texture image) {
		appearColor.a = 0;
		ContentWrapper wrapper = new ContentWrapper(txt, image, msgStyle, contentWidth);
		contents.Add(wrapper);
		contentHeight += wrapper.getContentHeight();
	}
	
	private void scrollMsg(bool up) {
		scrollMsg (up, scrollSpeed);
	}
	
	private void scrollMsg(bool up, float speed) {
		if (up) {
			adjustTextOffset(speed);
		} else {
			adjustTextOffset(-speed);
		}
	}
	
	private void scrollToLastMessage() {
		needToScrollToLast = true;
	}
	
	private void adjustTextOffset (float offset) {
		msgStartY += offset;
		if (msgStartY > msgMaxY) {
			msgStartY = msgMaxY;
		} else if (contentHeight < scrollViewRect.height) {
			msgStartY -= offset;
			needToScrollToLast = false;
		} else {
			if (contentHeight < (scrollViewRect.height + scrollViewRect.y - msgStartY)) {
				msgStartY = -(contentHeight - scrollViewRect.height - scrollViewRect.y);
				needToScrollToLast = false;
			}
		}
	}
	
	public void showNewMessage (MessageContainer messageContainer) {
		showNewMessage(messageContainer, 1);
	}
	
	public void showNewMessage (MessageContainer messageContainer, int messageIndex) {
		this.messageContainer = messageContainer;
        townScreen.setVisible(false);
		gameObject.SetActive(true);
		displayNextMessage(messageIndex);
	}
	
	public void hideMessageBox () {
		hided = true;
		gameObject.SetActive (false);
	}
	
	public void unhideMessageBox () {
		hided = false;
		gameObject.SetActive (true);
	}

	public static bool isHided () {
		return hided;
	}
	
	public void closeMessageBox (bool enablePlanet) {
		this.messageContainer = null;
		this.messageObject = null;
		contents.Clear ();
		msgStartY = msgMaxY;
		contentHeight = 0;
		gameObject.SetActive(false);
//		UserInterface.showInterface = true;
        townScreen.setVisible(enablePlanet);
	}
	
	private void displayNextMessage (int messageIndex) {
		displayNextMessage (messageContainer.getMessageObject (messageIndex));
	}
	
	private void displayNextMessage (MessageContainer.MessageObject message) {
		messageObject = message;
		addContentWrapper(message.getMessageText(), message.getPortrait());
		contentHeight = 0;
		foreach(ContentWrapper wrapper in contents) {
			contentHeight += wrapper.getContentHeight();
		}
		if (message.getButtons().Length > 0) {
			contentHeight += msgToBtnOffset + (btnHeight * message.getButtons().Length) + (btnSpacing * (message.getButtons().Length - 1));
		}
		scrollToLastMessage();
	}
	
	private void clickButton(MessageContainer.ButtonObject btn) {
		switch (btn.getInstructionType()) {
			case MessageContainer.InstructionType.CLOSE:
				closeMessageBox(true);
				break;
	//		case MessageContainer.InstructionType.GOTO:
	//			addContentWrapper("\n<color=orange>" + btn.getBtnText() + "</color>\n", null);
	//			displayNextMessage(int.Parse(btn.getInstructionParams()[0]));
	//			break;
			case MessageContainer.InstructionType.GOTO:
				displayNextMessage(int.Parse(btn.getInstructionParams()[0]));
				break;
			default: Debug.Log("Неизвестная инструкция"); break;
		}
	}
	
	private class ContentWrapper {
		private GUIContent content;
		private float contentHeight;
		private bool appearing;
		
		public ContentWrapper(string txt, Texture image, GUIStyle msgStyle, float contentWidth) {
			this.content = new GUIContent(image == null? txt: "<color=orange>" + txt + "</color>", image);
			contentHeight = msgStyle.CalcHeight(this.content, contentWidth);
			appearing = true;
		}
		
		public GUIContent getContent () {
			return content;
		}
		
		public float getContentHeight () {
			return contentHeight;
		}
		
		public bool isAppearing () {
			return appearing;
		}
		
		public void setAppearing (bool appearing) {
			this.appearing = appearing;
		}
	}
}