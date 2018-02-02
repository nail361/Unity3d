using System.Collections;
using UnityEngine;

[CreateAssetMenu(fileName = "GameSettings", menuName = "MySettings/GameSettings")]
public class GameSettings : ScriptableObject {
	public int minWordLength = 3;
	public int maxHealth = 3;
	public bool sortWords = false;

	public enum OPTIONS
	{
		Up = 0,
		Down = 1
	}

	public OPTIONS OrderBy;
}
