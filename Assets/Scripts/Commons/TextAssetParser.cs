using UnityEngine;
using System.Collections;

public class TextAssetParser {
	
	private static string[] msgObjSepar = {"||"}, msgSepar = {"__"}, btnSepar = {"--"}, btnDescSepar = {"::"}, btnParamsSepar = {","};
	
	public static MessageContainer parseTextAsset (TextAsset textAsset) {
		MessageContainer messageContainer = new MessageContainer();
		string[] msgObjs = textAsset.text.Split (msgObjSepar, System.StringSplitOptions.None);
		messageContainer.initContainer (msgObjs.Length);
		foreach (string msgObj in msgObjs) {
			string[] parts = msgObj.Split(msgSepar, System.StringSplitOptions.None);
			string[] btns = parts[2].Replace("\r","").Replace("\n", "").Trim().Split(btnSepar, System.StringSplitOptions.None);
			
			MessageContainer.MessageObject messageObject = new MessageContainer.MessageObject(int.Parse(parts[0]), parts[1], btns.Length-1);
			
			foreach(string btnDesc in btns) {
				if (btnDesc.Length == 0) {
					continue;
				}
				string[] btnParts = btnDesc.Split(btnDescSepar, System.StringSplitOptions.None);

				string btnText = btnParts[0];
				string btnInstruct = btnParts[1];
				string[] instructionParams = (btnParts.Length > 2? btnParts[2].Split(btnParamsSepar, System.StringSplitOptions.None): null);
				messageObject.addButton(btnText, btnInstruct, instructionParams);
			}
			
			messageContainer.addMessageObject(messageObject);
		}
		
		return messageContainer;
	}
}