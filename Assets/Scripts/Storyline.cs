using UnityEngine;
using System.Collections;

public class Storyline : MonoBehaviour {

	public TextAsset introduction;

	private MessageContainer introductionMC;

	public void init () {
		introductionMC = TextAssetParser.parseTextAsset(introduction);
	}

	public MessageContainer getMessageContainer (StoryPart storyPart) {
		switch (storyPart) {
			case StoryPart.INTRODUCTION: return introductionMC;
			default: Debug.Log("Неизвестная часть"); return null;
		}
	}

	public enum StoryPart {
		INTRODUCTION
	}
}