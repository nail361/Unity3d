using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;

public class GameManager : MonoBehaviour {

	public static GameManager _instance;

	public GameSettings gameSettings;

	[SerializeField]
	private GameObject gamePanel;

	[SerializeField]
	private Text infoField;

	[SerializeField]
	private Text scoreField;

	[SerializeField]
	private Text healthField;

	[SerializeField]
	private GameObject cubePref;
	[SerializeField]
	private GameObject cubeBorder;

	[SerializeField]
	private CubeLetter[] cubes;

	private List<Word> wordsList = new List<Word>();
	private TextAsset textData;

	private string curWord = "";
	private int wordCounter = 0;
	private int letterCounter = 0;

	private bool isReady = false;

	void Awake(){
		if (!_instance)
			_instance = this;
		else
			Destroy (gameObject);
	}

	void Start () {
		gamePanel.SetActive(true);
		infoField.text = "LOADING...";

		StartCoroutine("DownloadAssetBundle");
	}

	public IEnumerator DownloadAssetBundle() {
		while (!Caching.ready){
			OnBundleLoaded();
			yield return null;
		}

		using(WWW www = WWW.LoadFromCacheOrDownload ("file://" + Application.dataPath + "/AssetBundles/wordsbank", 0)){
			yield return www;
			if (www.error != null) throw new Exception("WWW download:" + www.error + " url:" + www.url);

			AssetBundle assetBundle = www.assetBundle;
			
			AssetBundleRequest request = assetBundle.LoadAssetAsync("alice30.txt", typeof(TextAsset));
			yield return request;
			textData = request.asset as TextAsset;
			OnBundleLoaded();
			assetBundle.Unload(false);
		}
	}

	private void OnBundleLoaded(){
		FillWords();
		if (gameSettings.sortWords) SortWords();
		StartGame();
	}

	private void FillWords(){

		//string[] wordsArray = textData.text.Split(new[] { ' ', ';' ,'.', ',', ':', '?', '!', '(', ')', '\'', '1', '2', '3', '4', '5', '6', '7', '8', '9', '0' }, StringSplitOptions.RemoveEmptyEntries);

		List<string> wordsArray = new List<string> ();
		string alphabet = "ABCDEFGHIJKLMNOPQRSTUVWXYZ";

		string collectedWord = "";

		foreach (char letter in textData.text) {
			if (alphabet.Contains(letter.ToString().ToUpper())) {
				collectedWord += letter;
			} else {
				if (collectedWord.Length > 0) {
					wordsArray.Add (collectedWord);
					collectedWord = "";
				}
			}
		}

		foreach (string word in wordsArray) {
			if (word.Length <= gameSettings.minWordLength || word.Length > cubes.Length)
				continue;

			string normWord = word.ToUpper();

			bool newWorld = true;
			foreach (Word w in wordsList) {
				if (w.key == normWord) {
					w.count++;
					newWorld = false;
				}
			}

			if (newWorld) {
				wordsList.Add (new Word (normWord));
			}
		}

	}

	private void StartGame(){
		GetNextWord();
		InitCubes();
		gamePanel.SetActive(false);
		isReady = true;
	}

	private void ResetGame(){
		wordCounter = 0;

		healthField.text = Player._instance.ResetHealth().ToString();
		scoreField.text = Player._instance.ResetScore().ToString();

		if (gameSettings.sortWords && gameSettings.OrderBy == GameSettings.OPTIONS.Random) SortWords();
		
		StartGame();
	}

	private void InitCubes(){

		Debug.Log (curWord);

		int startPoint = cubes.Length / 2  - curWord.Length / 2;

		for(int i = 0; i < cubes.Length; i++) {
			
			if (i >= startPoint && i < (curWord.Length + startPoint)) {
				cubes[i].gameObject.SetActive(true);
				cubes[i].SetLetter (curWord [i - startPoint]);
				cubes[i].transform.rotation = Quaternion.Euler(new Vector3(0,180,0));
			} else {
				cubes[i].Hide();
			}

		}
	}

	public void KeyPress(char key){

		if (!isReady) return;
		if (Key.HideBtn(key)) return;

		bool mistake = true;

		foreach (CubeLetter cube in cubes) {
			if (cube.gameObject.activeSelf && cube.GetLetter () == key) {
				letterCounter--;
				cube.StartCoroutine("RotationCoroutine");
				mistake = false;
			}
		}

		if (mistake) {
			if (!Player._instance.SubHealth ()) {
				gamePanel.SetActive(true);
				infoField.text = "GAME OVER";
				isReady = false;
				Invoke("ResetGame", 3.0f);
			}

			healthField.text = Player._instance.Health.ToString ();
		} else {
			if (letterCounter == 0) {
				scoreField.text = Player._instance.AddScore().ToString();
				healthField.text = Player._instance.ResetHealth().ToString();
				isReady = false;
				Invoke("StartGame", 1.0f);
			}
		}
	}

	private void GetNextWord(){
		if (wordCounter >= wordsList.Count) {
			gamePanel.SetActive(true);
			infoField.text = "YOU ARE WIN!";
			isReady = false;
			Invoke("ResetGame", 3.0f);
			return;
		}

		healthField.text = Player._instance.Health.ToString();
		Key.ShowAllBtns();
		curWord = wordsList[wordCounter].key;
		letterCounter = curWord.Length;
		wordCounter++;
	}

	public int GetMaxHealth(){
		return gameSettings.maxHealth;
	}

	private void SortWords(){
		if (gameSettings.OrderBy != GameSettings.OPTIONS.Random) {
			wordsList.Sort(
				delegate (Word x, Word y){
					if (gameSettings.OrderBy == GameSettings.OPTIONS.Up){
						if (x.count > y.count) return -1;
						else return 1;
					}
					else{
						if (x.count > y.count) return 1;
						else return -1;
					}
				}
			);
		}
		else{
			for(int i = 0; i < wordsList.Count; i++) {
				int rndIndex = Mathf.FloorToInt(UnityEngine.Random.value * wordsList.Count);
				Word firstWord = wordsList[i];
				Word secondWord = wordsList[rndIndex];

				wordsList[i] = secondWord;
				wordsList[rndIndex] = firstWord;
			}
		}
	}

	class Word{
		public string key = "";
		public int count = 0;

		public Word(string key){
			this.key = key;
		}
	}

}
