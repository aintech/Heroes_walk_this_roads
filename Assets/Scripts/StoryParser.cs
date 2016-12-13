using UnityEngine;
using System.Collections;

public class StoryParser : MonoBehaviour {
	
	private static char[] msgObjSepar = {'\n'}, instrDataSepar = {';'};//, msgSepar = {"__"}, btnSepar = {"--"}, btnDescSepar = {"::"}, btnParamsSepar = {","};

	private static string[] instSepar = {"<>"};

	public static StoryContainer parseStory(TextAsset storyText) {
		StoryContainer container = new StoryContainer();
		string[] msgObjs = storyText.text.Split(msgObjSepar, System.StringSplitOptions.RemoveEmptyEntries);
		string[] msgData;
		string[] instrData;
		string background = null;
		string portrait = null;
		for(int i = 0; i < msgObjs.Length; i++) {
			msgData = msgObjs[i].Split(instSepar, System.StringSplitOptions.None);
			if (msgData[0] != null && msgData[0].Length > 0) {
				instrData = msgData[0].Split(instrDataSepar, System.StringSplitOptions.RemoveEmptyEntries);
				for (int j = 0; j < instrData.Length; j++) {
					if (instrData[j].StartsWith("prt")) { portrait = Vars.EROTIC? instrData[j].Substring(4): null; }
					else if (instrData[j].StartsWith("bg")) { background = instrData[j].Substring(3); }
				}
			}
			container.addStoryObject(new StoryContainer.StoryObject(msgData[1], background, portrait));
		}

		return container;
	}
}