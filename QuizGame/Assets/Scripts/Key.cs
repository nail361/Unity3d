using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Key : MonoBehaviour {

	[SerializeField]
	private char key;

	public void KeyDown(){
		//GameManager.KeyPress(key);	
	}

	public void HideBtn(){
		gameObject.SetActive(false);
	}

}
