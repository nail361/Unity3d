using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Key : MonoBehaviour {

	public char GetKey{
		get {return key;}
	}

	[SerializeField]
	private char key;

	private static GameObject[] btns = null;

	public void KeyDown(){
		GameManager._instance.KeyPress(key);
	}

	public static bool HideBtn(char keyPressed){
		if (btns == null) btns = GameObject.FindGameObjectsWithTag("btn");

		foreach(GameObject btn in btns){
			if (btn.activeSelf && btn.GetComponent<Key> ().GetKey == keyPressed) {
				btn.SetActive (false);
				return false;
			}
		}

		return true;
	}

	public static void ShowAllBtns(){

		if (btns == null) btns = GameObject.FindGameObjectsWithTag("btn");

		foreach(GameObject btn in btns){
			btn.SetActive(true);
		}
	}

}
