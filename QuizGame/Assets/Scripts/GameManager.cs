using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class GameManager : MonoBehaviour {

	public static GameManager _instance;

	public GameSettings gameSettings;

	[SerializeField]
	private GameObject cubePref;
	[SerializeField]
	private GameObject cubeBorder;

	private List<GameObject> cubePool = new List<GameObject>();

	private List<Word> wordsList = new List<Word>();
	private TextAsset textData;

	private string curWord = "";
	private int wordCounter = 0;
	private float dx = 1.15f;

	void Start () {

		if (!_instance)
			_instance = this;
		else
			Destroy (gameObject);

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
		FillWords ();
		SortWords ();

		StartGame ();
	}

	private void FillWords(){

		string[] wordsArray = textData.text.Split(new[] { ' ', ';' ,'.', ',', ':', '?', '!', '(', ')', '\'', '1', '2', '3', '4', '5', '6', '7', '8', '9', '0' }, StringSplitOptions.RemoveEmptyEntries);

		foreach (string word in wordsArray) {

			if (word.Length <= 3)
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
	}

	private void InitCubes(){

		int i;
		CubeLetter cl = null;

		Debug.Log (curWord);

		for( i = 0; i < curWord.Length; i++) {
			if (cubePool.Count <= i) {
				cubePool.Add( Instantiate(cubePref) );
				cubePool[i].transform.SetParent(cubeBorder.transform);
				cubePool[i].transform.position = new Vector3( dx * i, 0 ,0);
			}

			cl = cubePool[i].GetComponent<CubeLetter>();
			cl.SetLetter(curWord[i]);
			//cubePool[i].transform.rotation.eulerAngles = new Vector3(0,180,0);
		}

		for (; i < cubePool.Count; i++) {
			cubePool [i].SendMessage("Hide");
		}
	}

	private void GetNextWord(){
		if (wordCounter >= wordsList.Count) {
			//You are win
			Debug.Log("You are win");
			return;
		}

		curWord = wordsList[wordCounter].key;
		wordCounter++;
	}

	private void SortWords(){
		wordsList.Sort((x, y) => x.count.CompareTo(y.count));
	}

	class Word{
		public string key = "";
		public int count = 0;

		public Word(string key){
			this.key = key;
		}
	}

}
