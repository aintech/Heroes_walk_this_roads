using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class StoryContainer {

	private Queue<StoryObject> storyline = new Queue<StoryObject>();

	public void addStoryObject (StoryObject storyObject) {
		storyline.Enqueue(storyObject);
	}

	public StoryObject getNext () {
		if (storyline.Count == 0) { return null; }
		return storyline.Dequeue();
	}

	public class StoryObject {
		public string text { get; private set; }
		public Texture portrait { get; private set; }
		public Texture background { get; private set; }

		public StoryObject (string text, string backgroundName, string portraitName) {
			this.text = text;
			loadBackground(backgroundName);
			loadPortrait(portraitName);
		}

		private void loadBackground (string backgroundName) {
			background = backgroundName == null? null: Imager.getStoryBackground(backgroundName);
		}

		private void loadPortrait (string portraitName) {
			portrait = portraitName == null? null: Imager.getPortrait(CharacterDescriptor.nameToType(portraitName));
		}
	}
}