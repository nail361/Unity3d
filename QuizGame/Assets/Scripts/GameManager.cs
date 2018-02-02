using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System;

public class GameManager : MonoBehaviour {

	public GameSettings gameSettings;

	private List<Word> wordsList = new List<Word>();
	private TextAsset textData;

	void Start () {
		StartCoroutine("DownloadAssetBundle");
	}

	public IEnumerator DownloadAssetBundle() {

//#if UNITY_EDITOR
		//textData = (TextAsset)UnityEditor.AssetDatabase.LoadAssetAtPath("Assets/AssetBundles/wordsBank", typeof(TextAsset));
		//yield return null;

//#else
		while (!Caching.ready){
			OnBundleLoaded();
			yield return null;
		}

		using(WWW www = WWW.LoadFromCacheOrDownload ("file://C:/Users/balyasnikov.ds/Documents/UnityProjects/QuizGame/Assets/AssetBundles/wordsBank", 0)){
			yield return www;
			if (www.error != null) throw new Exception("WWW download:" + www.error);

			AssetBundle assetBundle = www.assetBundle;
			
			AssetBundleRequest request = assetBundle.LoadAssetAsync("alice30.txt", typeof(TextAsset));
			yield return request;
			textData = request.asset as TextAsset;
			OnBundleLoaded();
			assetBundle.Unload(false);
		}

//#endif
	}

	private void OnBundleLoaded(){
		FillWords ();
		SortWords ();

		//StartGame ();
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
