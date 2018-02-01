using System;
using UnityEngine;

public class GameManager:MonoBehaviour {
	
	public delegate void ChangePoint();
	public static event ChangePoint OnChangePoint;
	
	public byte PlayerPlane = 1;
	public byte UnlockPlane = 1<<3;
	private int[] PlanesPrices = new int[4]{0,200,1000,5000};
	private byte[,] PlanesParams = new byte[4,3]{
		{1,2,10},
		{2,3,13},
		{3,4,18},
		{4,5,25}
	};
	
	public int Score = 0;
	public int Points = 0;
	private int[,] UpgradeCosts = new int[3,5]{
		{140,260,380,500,600},
		{120,220,330,440,550},
		{100,200,300,400,500}
	};
	private byte[,] UpgradeParams = new byte[3,6]{
		{2,3,5,7,8,10}, //щит
		{1,2,4,5,7,8}, //магнит
		{2,4,5,6,7,9} //slow
	};
	private byte[] UpgradeLevels = new byte[]{0,0,0};
	
	static public bool isActive {
		get {
			return _instance != null;
		}
	}
	
	static GameManager _instance;
	static public GameManager instance
	{
		get
		{
			if (_instance == null)
			{
				_instance = UnityEngine.Object.FindObjectOfType(typeof(GameManager)) as GameManager;
				
				if (_instance == null)
				{
					GameObject go = new GameObject("_gamemanager");
					DontDestroyOnLoad(go);
					_instance = go.AddComponent<GameManager>();
				}
			}
			return _instance;
		}
	}
	
	public bool MUSIC;
	public bool SOUND;
	public bool SFX;

	public bool FIRST_START;
	
	private AudioSource MusicSource;
	private AudioSource SoundSource;
	
	void Awake(){

        Application.targetFrameRate = 30;
		
#if UNITY_ANDROID
		//if ( !(Application.genuine) ) return;
#endif
		MusicSource = gameObject.AddComponent<AudioSource>();
		MusicSource.playOnAwake = false;
		MusicSource.ignoreListenerVolume = true;
		MusicSource.loop = true;
		
		SoundSource = gameObject.AddComponent<AudioSource>();
		SoundSource.playOnAwake = false;
		SoundSource.loop = false;
		
		if ( PlayerPrefs.HasKey("Music") ){
			MUSIC = PlayerPrefs.GetInt( "Music" ) == 1 ? true : false;
			SOUND = PlayerPrefs.GetInt( "Sound" ) == 1 ? true : false;
			SFX = PlayerPrefs.GetInt( "Clouds" ) == 1 ? true : false;

			FIRST_START = false;
		}
		else{
			MUSIC = true;
			SOUND = true;
			SFX = true;

			FIRST_START = true;
		}
		
		LoadData();
		
		MusicSource.mute = !MUSIC;
		AudioListener.volume = SOUND ? 1 : 0;
	}
	
	public void MusicPitch( float value ){
		MusicSource.pitch = value;
	}
	
	public void PlayMusic( AudioClip clip ){
		MusicSource.Stop();
		
		if ( clip == null ) return;
		
		MusicSource.clip = clip;
		MusicSource.Play();
	}
	
	public void PlaySound( AudioClip sound ){
		if ( AudioListener.volume > 0 ){
			SoundSource.PlayOneShot( sound );
		}
	}
	
	public void SetMusic( bool toggle ){
		MUSIC = toggle;
		MusicSource.mute = !toggle;
	}
	
	public void SetSound( bool toggle ){
		SOUND = toggle;
		AudioListener.volume = toggle ? 1 : 0;
	}
	
	private void SavePrefs(){
		PlayerPrefs.SetInt( "Music", MUSIC ? 1 : 0 );
		PlayerPrefs.SetInt( "Sound", SOUND ? 1 : 0 );
		PlayerPrefs.SetInt( "Clouds", SFX ? 1 : 0 );
		
		PlayerPrefs.Save();
	}
	
	void OnApplicationQuit() {
		SavePrefs();
	}
	
	public byte GetUpgradeParam( int index ){
		if ( index > UpgradeParams.Length || index < 0 ) Debug.LogError("выход за границы UpgradeParams " + index.ToString() );
		return UpgradeParams[index,GetUpgradeLevel(index)];
	}
	
	public int GetUpgradeCost( int index ){
		if ( index > UpgradeCosts.Length || index < 0 ) Debug.LogError("выход за границы UpgradeCosts " + index.ToString() );
		return UpgradeCosts[index,GetUpgradeLevel(index)];
	}
	
	public byte GetPlaneControl( int index ){
		return PlanesParams[ index-1, 0 ];
	}
	
	public byte GetPlaneGlide( int index ){
		return PlanesParams[ index-1, 1];
	}
	
	public byte GetPlaneAcceleration( int index ){
		return PlanesParams[ index-1, 2 ];
	}
	
	public int GetPlanePrice( int index ){
		if ( index > PlanesPrices.Length || index < 0 ) Debug.LogError("выход за границы PlanesPrices " + index.ToString() );
		return PlanesPrices[index];
	}
	
	public byte GetUpgradeLevel( int index ){
		if ( index > UpgradeLevels.Length || index < 0 ) Debug.LogError("выход за границы UpgradeLevels " + index.ToString() );
		return UpgradeLevels[index];
	}
	
	public void BuyUpgradeLevel( int index ){
		if ( index > UpgradeLevels.Length || index < 0 ) Debug.LogError("выход за границы UpgradeLevels " + index.ToString() );
		Points -= GetUpgradeCost( index );
		UpgradeLevels[index]++;
		OnChangePoint();
		SaveData();
	}
	
	public void BuyPlane( ){
		Points -= GetPlanePrice(Swipe.Index());
		UnlockPlane |= (byte)(1 << (3 - Swipe.Index()) );
		OnChangePoint();
		SaveData();
	}
	
	public void ChoosePlane( ){
		GameManager.instance.PlayerPlane = (byte)(Swipe.Index()+1);
		SaveData();
	}
	
	public void BuyPoints(){
		Points += 1000;
		OnChangePoint();
		SaveData();
	}
	
	private void LoadData(){
		DataToSave obj = (DataToSave)SaveLoad.LoadData( "save.dat" );
		
		if ( obj == null ) return;
		
		PlayerPlane = obj.PlayerPlane;
		UnlockPlane = obj.UnlockPlane;
		Score = obj.Score;
		Points = obj.Points;
		UpgradeLevels = obj.UpgradeLevels;
		
	}
	
	public void SaveData(){
		DataToSave data = new DataToSave();
		data.PlayerPlane = PlayerPlane;
		data.UnlockPlane = UnlockPlane;
		data.Score = Score;
		data.Points = Points;
		data.UpgradeLevels = UpgradeLevels;
		
		SaveLoad.SaveData( data, "save.dat" );
		
		SavePrefs();
	}

	public void ResetData(){

		PlayerPlane = 1;
		UnlockPlane = 1<<3;
		Score = 0;
		Points = 0;
		UpgradeLevels = new byte[]{0,0,0};

		FIRST_START = true;

		OnChangePoint();

		DataToSave data = new DataToSave();
		data.PlayerPlane = PlayerPlane;
		data.UnlockPlane = UnlockPlane;
		data.Score = Score;
		data.Points = Points;
		data.UpgradeLevels = UpgradeLevels;
		
		SaveLoad.SaveData( data, "save.dat" );

		PlayerPrefs.DeleteAll();
	}
	
}

[Serializable]
class DataToSave{
	public byte PlayerPlane;
	public byte UnlockPlane;
	public int Score;
	public int Points;
	public byte[] UpgradeLevels;
}