using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

public class Title : MonoBehaviour {
	
	public void startNewGame () {
		SceneManager.LoadScene("TownMainScreen");
	}
}