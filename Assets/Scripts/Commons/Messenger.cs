using UnityEngine;
using System.Collections;

public class Messenger : MonoBehaviour {

	public static void showMessage (string message) {
//		Debug.Log(message);
		UserInterface.instance.setMessageText(message);
	}

	public static void inventoryCapacityLow (string itemName, int count) {
		UserInterface.instance.setMessageText("Объёма инвентаря не достаточно для добавления предмета(ов): " + (count == 1? "": count + " X ") + itemName);
	}

	public static void notEnoughtCash (string itemName, int count) {
		UserInterface.instance.setMessageText("Не достаточно кредитов на покупку: " + (count == 1? "": count + " X ") + itemName);
	}
}