using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour {

	private int health = 0;
	public int Health{
		get { return health;}
	}

	private int score = 0;

	public static Player _instance;

	void Awake(){
		if (!_instance)
			_instance = this;
		else
			Destroy(gameObject);
	}

	void Start () {
		health = GameManager._instance.GetMaxHealth();
	}

	public bool SubHealth(){
		health--;

		if (health == 0)
			return false;
		else
			return true;
	}

	public int AddScore(){
		return score += health;
	}

	public int ResetHealth(){
		return health = GameManager._instance.GetMaxHealth();
	}

	public int ResetScore(){
		return score = 0;
	}

	void OnGUI() {
		Event e = Event.current;

		if (Input.anyKeyDown && e.isKey)
		if (e.keyCode >= KeyCode.A && e.keyCode <= KeyCode.Z) {

			char key = 'A';

			switch (e.keyCode)
			{
			case KeyCode.A:
				key = 'A';
				break;
			case KeyCode.B:
				key = 'B';
				break;
			case KeyCode.C:
				key = 'C';
				break;
			case KeyCode.D:
				key = 'D';
				break;
			case KeyCode.E:
				key = 'E';
				break;
			case KeyCode.F:
				key = 'F';
				break;
			case KeyCode.G:
				key = 'G';
				break;
			case KeyCode.H:
				key = 'H';
				break;
			case KeyCode.I:
				key = 'I';
				break;
			case KeyCode.J:
				key = 'J';
				break;
			case KeyCode.K:
				key = 'K';
				break;
			case KeyCode.L:
				key = 'L';
				break;
			case KeyCode.M:
				key = 'M';
				break;
			case KeyCode.N:
				key = 'N';
				break;
			case KeyCode.O:
				key = 'O';
				break;
			case KeyCode.P:
				key = 'P';
				break;
			case KeyCode.Q:
				key = 'Q';
				break;
			case KeyCode.R:
				key = 'R';
				break;
			case KeyCode.S:
				key = 'S';
				break;
			case KeyCode.T:
				key = 'T';
				break;
			case KeyCode.U:
				key = 'U';
				break;
			case KeyCode.V:
				key = 'V';
				break;
			case KeyCode.W:
				key = 'W';
				break;
			case KeyCode.X:
				key = 'X';
				break;
			case KeyCode.Y:
				key = 'Y';
				break;
			case KeyCode.Z:
				key = 'Z';
				break;
			}

			GameManager._instance.KeyPress(key);
		}

	}
}
