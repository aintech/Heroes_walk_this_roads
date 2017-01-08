using UnityEngine;
using System.Collections;

public class UserInterface : MonoBehaviour, ButtonHolder {
    
	public static UserInterface instance { get; private set; }

    private static int day = 0;

    private static Color32  white = new Color32(255, 255, 255, 255),
                            green = new Color32(0, 255, 0, 255),
                            yellow = new Color32(255, 255, 0, 255),
                            red = new Color32(255, 0, 0, 255);

    private static TextMesh dayCount, questInfo, goldValue, healthValue, rankValue;

    private static Button statusBtn;

    private const float rankMaskMaxHeight = .36f, healthMaskMaxHeight = .34f;//.364f;

    private static Vector2 rankMaskDelta = new Vector2(.4f, 0), healthMaskDelta = new Vector2(.485f, 0);

    private static RectTransform rankMaskRect, healthMaskRect;

	public GUIStyle messengerStyle;

	private StatusScreen statusScreen;

	private string messageText = null;

	private int counter;

    private Color32 messengerTextColor = new Color32(122, 221, 41, 255);

	private Rect messengerRect = new Rect(10, Screen.height - 50, Screen.width, 50);

    private static bool canOpenStatusScreen;

    public UserInterface init () {
		instance = this;
        Transform maskCanvas = transform.Find("MaskCanvas");
        maskCanvas.gameObject.SetActive(true);

		statusScreen = StatusScreen.instance;

        rankMaskRect = maskCanvas.Find("RankMask").GetComponent<RectTransform>();
        healthMaskRect = maskCanvas.Find("HealthMask").GetComponent<RectTransform>();
        dayCount = transform.Find("DayCount").GetComponent<TextMesh>();
        TextMesh dayLabel = transform.Find("DayLabel").GetComponent<TextMesh>();
        questInfo = transform.Find("QuestInfo").GetComponent<TextMesh>();
        goldValue = transform.Find("GoldValue").GetComponent<TextMesh>();
        healthValue = transform.Find("HealthValue").GetComponent<TextMesh>();
        rankValue = transform.Find("RankValue").GetComponent<TextMesh>();

        string layerName = "User Interface";
        int sortingOrder = 1;

        MeshRenderer mesh = dayCount.GetComponent<MeshRenderer>();
        mesh.sortingLayerName = layerName;
        mesh.sortingOrder = sortingOrder;
        mesh = questInfo.GetComponent<MeshRenderer>();
        mesh.sortingLayerName = layerName;
        mesh.sortingOrder = sortingOrder;
        mesh = goldValue.GetComponent<MeshRenderer>();
        mesh.sortingLayerName = layerName;
        mesh.sortingOrder = sortingOrder;
        mesh = healthValue.GetComponent<MeshRenderer>();
        mesh.sortingLayerName = layerName;
        mesh.sortingOrder = sortingOrder;
        mesh = dayLabel.GetComponent<MeshRenderer>();
        mesh.sortingLayerName = layerName;
        mesh.sortingOrder = sortingOrder;
        mesh = rankValue.GetComponent<MeshRenderer>();
        mesh.sortingLayerName = layerName;
        mesh.sortingOrder = sortingOrder;

        statusBtn = transform.Find("Status Button").GetComponent<Button>().init();

        dayLabel.gameObject.SetActive(true);
        transform.Find("BG").gameObject.SetActive(true);

        dayCount.gameObject.SetActive(true);
        questInfo.gameObject.SetActive(true);
        goldValue.gameObject.SetActive(true);
        healthValue.gameObject.SetActive(true);
        statusBtn.gameObject.SetActive(true);

        addDay();
        updateGold();
        updateHealth();
        updateRank();
        updateStatusBtnText(false);

        return this;
    }

    public static void addDay () {
        day++;
        dayCount.text = day.ToString();
//        QuestBoard.reloadQuests = true;
    }

    public static void updateGold () {
        goldValue.text = Vars.gold.ToString();
    }

    public static void updateHealth () {
//        healthValue.text = Player.health.ToString();
//        if (Player.health == Player.maxHealth) {
//            healthValue.color = white;
//            healthMaskDelta.y = healthMaskMaxHeight;
//        } else {
//            float diff = (float)Player.health / (float)Player.maxHealth;
//            healthMaskDelta.y = Mathf.Max(.01f, diff * healthMaskMaxHeight);
//            if (diff > .6f) {
//                healthValue.color = green;
//            } else if (diff > .2f) {
//                healthValue.color = yellow;
//            } else {
//                healthValue.color = red;
//            }
//        }
//        healthMaskRect.sizeDelta = healthMaskDelta;
    }

    public static void updateRank () {
//        rankValue.text = Hero.getRank().ToString();
//        rankMaskDelta.y = Mathf.Max(.01f, Hero.getRankPointsDiff() * rankMaskMaxHeight);
//        rankMaskRect.sizeDelta = rankMaskDelta;
    }

    public static void showQuestInfo (string info) {
        questInfo.text = info;
    }

    public static void hideQuestInfo () {
        questInfo.text = "";
    }

    public void fireClickButton (Button btn) {
        if (statusScreen.gameObject.activeInHierarchy) {
            statusScreen.close(false);
        } else {
            statusScreen.showScreen();
        }
    }

    public static void updateStatusBtnText (bool asClose) {
        statusBtn.setText(asClose ? "Закрыть" : "Статус");
        canOpenStatusScreen = !asClose;
    }

    public static void setEquipmentBtnActive (bool active) {
        statusBtn.setActive(active);
        canOpenStatusScreen = active;
//        InputProcessor.canOpenEquipment = active;
    }

//	public UserInterface init (StatusScreen statusScreen) {
//		this.statusScreen = statusScreen;
//
//		textColor = messengerStyle.normal.textColor;
//
//		gameObject.SetActive(true);
//		showInterface = true;
//
//		return this;
//	}

	void Update () {
		if (Input.GetKeyDown(KeyCode.I)) {
            if (canOpenStatusScreen) { statusScreen.showScreen(); }
            else if (statusScreen.gameObject.activeInHierarchy) { statusScreen.close(false); }
//			if (statusScreen == null) { return; }
//			if (showInterface) { statusScreen.showScreen(); }
//			else if (statusScreen.gameObject.activeInHierarchy) { InputProcessor.closeToCurrent(statusScreen); }
		}
	}

	void OnGUI () {
//		if (showIsnterface) {
//			if (GUI.Button(statusBtnRect, "", statusBtnStyle)) {
//				statusScreen.showScreen();
//			}
//		}
		if (messageText != null) {
			GUI.Label(messengerRect, messageText, messengerStyle);
			if (--counter <= 0) {
                if (messengerTextColor.a > 100) {
                    messengerStyle.normal.textColor = messengerTextColor;
                    --messengerTextColor.a;
				} else {
					messageText = null;
				}
			}
		}
	}

	public void setMessageText (string text) {
		counter = 100;
        messengerTextColor.a = 255;
        messengerStyle.normal.textColor = messengerTextColor;
		messageText = text;
	}
}