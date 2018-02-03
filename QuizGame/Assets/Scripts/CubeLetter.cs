using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;
using System;

public class CubeLetter : MonoBehaviour {

	[SerializeField]
	private Text letterText;

	private char letter;

	private bool isActive = false;

	public bool IsActive{
		get{return isActive;}
	}

	public void SetLetter (char letter) {
		this.letter = letter;
		letterText.text =  Char.ToString(letter);
		isActive = true;
	}

	public char GetLetter () {
		return letter;
	}

	public IEnumerator RotationCoroutine ()
	{
		int iterator = 18;
		while(iterator > 0){
			transform.Rotate(Vector3.forward,10);
			iterator--;
			yield return new WaitForSeconds (0.01f);
		}
	}

	public void Hide(){
		gameObject.SetActive(false);
		isActive = false;
	}
}
