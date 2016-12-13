using UnityEngine;
using System.Collections;

public class Story : MonoBehaviour {

	public TextAsset introduction;

	public Texture textBG;

	public GUIStyle textStyle;

	private Texture background;

    private Town town;

	private StoryContainer storyContainer;

	private StoryContainer.StoryObject currentStory;

	private Rect backgroundArea = new Rect(0, 0, Screen.width, Screen.height),
				 textBGArea = new Rect(0, 0, Screen.width, Screen.height),
	textFlowArea = new Rect((float)Screen.width / 4.8f, Screen.height - (int)((float)Screen.height / 4.2f), (float)Screen.width / 1.6f, Screen.height),
				 portraitArea = new Rect(60, Screen.height - 200, 200, 200);

    public Story init (Town town) {
        this.town = town;
		gameObject.SetActive(false);
		return this;
	}

	void Update () {
		if (Input.GetMouseButtonDown(0) || Input.GetKeyDown(KeyCode.Space) || Input.GetKeyDown(KeyCode.Return) || Input.GetKeyDown(KeyCode.KeypadEnter)) { forward(); }
	}

	void OnGUI () {
		if (currentStory == null) { return; }
		GUI.DrawTexture(backgroundArea, background);
		GUI.DrawTexture(textBGArea, textBG);
		GUI.TextArea(textFlowArea,currentStory.text, textStyle);
		if (currentStory.portrait != null) { GUI.DrawTexture(portraitArea, currentStory.portrait); }
	}

	public void playNextChapter () {
		Vars.chapter++;
		switch(Vars.chapter) {
			case Chapter.INTRODUCTION: storyContainer = StoryParser.parseStory(introduction); break;
			default: Debug.Log("Unknown chapter: " + Vars.chapter); break;
		}
//		UserInterface.showInterface = false;
		gameObject.SetActive(true);
		forward();
	}

	private void forward () {
		currentStory = storyContainer.getNext();
		if (currentStory == null) {
			close();
		} else {
			if (currentStory.background != null) { background = currentStory.background; }
		}
	}

	private void close () {
        town.showScreen(Town.ScreenType.MAIN);
		currentStory = null;
//		UserInterface.showInterface = true;
		gameObject.SetActive(false);
	}

	public enum Chapter {
		NONE, INTRODUCTION
	}
}