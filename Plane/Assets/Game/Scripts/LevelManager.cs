using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine.Advertisements;
#if UNITY_EDITOR
using UnityEditor;
#endif
using GooglePlayGames;
using UnityEngine.SceneManagement;

delegate void OnOFFDel( bool on );

[RequireComponent (typeof (AudioSource))]

public class LevelManager : MonoBehaviour {

	static LevelManager _instance;
	static public LevelManager instance
	{
		get
		{
			if (_instance == null)
			{
				_instance = UnityEngine.Object.FindObjectOfType(typeof(LevelManager)) as LevelManager;
				
				if (_instance == null)
				{
					GameObject go = new GameObject("LevelManager");
					_instance = go.AddComponent<LevelManager>();
				}
			}
			return _instance;
		}
	}

	//Sounds
	public AudioClip GameOverSound;

	public AudioClip MusicSound;
	public AudioClip PointSound;

	public AudioClip GoodStuffSound;
	public AudioClip BadStuffSound;

	public AudioClip FuelSound;
	public AudioClip ShieldBreakSound;

	//GUI

	public GameObject GameOverPanel;

	public GameObject LowSpeed;

	public Sprite PauseSprite;
	public Sprite ResumeSprite;
	public Button PauseBtn;

	public Image MagnetIMG;
	public Text MagnetText;

	public Image ShieldIMG;
	public Text ShieldText;

	public Image SlowIMG;
	public Text SlowText;

	public Image FastIMG;
	public Text FastText;

	public Image HideSetkaIMG;
	public Text HideText;

	public Image InvertIMG;
	public Text InvertText;

	public Color32 PointsSelect;
	public Text PointsGUI;
	public Text ScoreGUI;
	public Text OldScoreGUI;
	public Text WaiteGUI;
	public Toggle MusicToggleGUI;
	public Toggle SoundToggleGUI;
	public Toggle SFXToggleGUI;
	public GameObject PausePanel;
	//------

	public CloudGenerator clouds;

	public PlayerManager playerManager;
	public ParticleSystem SpeedUpSFX;
	public Slider SpeedIndicator;

	public bool Paused = false;

	public GameObject[] goodStuff;
	public GameObject[] badStuff;
	public GameObject[] badStuffEffect;
	public GameObject[] fastStuff;
	public GameObject pointPref;
	public GameObject fuelPref;
	public GameObject warningPref;
	public GameObject setkaGO;
	public GameObject diePref;

	enum StuffType { None = 0, Good = 1, Bad = 2};

	//Plane params
	private int plane_glide;
	private int plane_accelerate;
	//-----------------
	public int Step = 5;
	private float spawnDistance = 75;
	private float Score = 0;
	private int Points = 0;
	private float speed = 0;
	private float minSpeed = 15;
	private float maxSpeed = 60;
	private float scoreSpeed = 20;
	private float speedMutant = 0;
	private float gameTime = 0;

	private float fixCount = 3;

	private float m_newSpeed;
	private float m_waitSFX = 0;
	private float m_fuelTimer = 1.5f;

	//private bool isWaitWarning = false;
	/*
	private byte[,] setka = new byte[3,3]{
		{0,0,0},
		{0,0,0},
		{0,0,0}
	};
	*/
	private StuffType[,] setka = new StuffType[3,3]{
		{StuffType.None,StuffType.None,StuffType.None},
		{StuffType.None,StuffType.None,StuffType.None},
		{StuffType.None,StuffType.None,StuffType.None}
	};

	private int goodStuffCount = 0;
	private int canGoodStuff = 0;
	private int MultipleStuff = 0;

	private List<GameObject> AllStuff = new List<GameObject>();

	private List<StuffItem> GoodStuffUse = new List<StuffItem>();
	private List<StuffItem> BadStuffUse = new List<StuffItem>();

	void Start(){
		GameManager.instance.PlayMusic( MusicSound );

		MusicToggleGUI.isOn = GameManager.instance.MUSIC;
		SoundToggleGUI.isOn = GameManager.instance.SOUND;
		SFXToggleGUI.isOn = GameManager.instance.SFX;

		clouds.enabled = SFXToggleGUI.isOn;

		OldScoreGUI.text = GameManager.instance.Score.ToString();

		Points = GameManager.instance.Points;
		PointsGUI.text = Points.ToString();

		SpeedIndicator.minValue = minSpeed;
		SpeedIndicator.maxValue = maxSpeed;
		SpeedIndicator.value = minSpeed;

		plane_glide = GameManager.instance.GetPlaneGlide( GameManager.instance.PlayerPlane );
		plane_accelerate = GameManager.instance.GetPlaneAcceleration( GameManager.instance.PlayerPlane );

		speed = (minSpeed + maxSpeed) / 2;
		
		Spawn();

		if ( GameManager.instance.FIRST_START ){
			Pause();
		}
	}

	public void SetkaZero( Vector2 index ){
		setka[(byte)index.x,(byte)index.y] = StuffType.None;

		Spawn();
	}

	public void DieStuff( GameObject stuff, bool breakFromShield = false ){

		if ( (int)stuff.GetComponent<Stuff>().type > 0 && (int)stuff.GetComponent<Stuff>().type < 10 ) goodStuffCount--;

		if ( breakFromShield ){
			GameManager.instance.PlaySound( ShieldBreakSound );
			Instantiate( diePref, stuff.transform.position, Quaternion.identity );
		}

		AllStuff.Remove( stuff );
		Destroy( stuff );
	}

	public void PickPoint(){
		GameManager.instance.PlaySound( PointSound );
		Points += 1;
		PointsGUI.text = Points.ToString();
		StopCoroutine( "ChangePointColor" );
		StartCoroutine( "ChangePointColor" );
	}

	IEnumerator ChangePointColor(){
		PointsGUI.color = PointsSelect;
		yield return new WaitForSeconds( 0.5f );
		PointsGUI.color = Color.white;
	}

	public void PickHideSetka( int index ){
		if (Social.localUser.authenticated){
			PlayGamesPlatform.Instance.IncrementAchievement(GPGConfigs.achievement_hide_picker, 1, null);
			PlayGamesPlatform.Instance.IncrementAchievement(GPGConfigs.achievement_hide_mania, 1, null);
		}

		StuffItem si = BadStuffUse.Find( x => x.type == index );
		
		if ( si == null )
			BadStuffUse.Add( new StuffItem( index, Random.Range(2,9), HideSetkaIMG, HideText, (byte)BadStuffUse.Count, new OnOFFDel( OnOffSetka ) ) );
		else
			si._Time += Random.Range(2,9);
	}
	public void OnOffSetka( bool trigger ){
		setkaGO.SetActive( !trigger );
	}

	public void PickInvert( int index ){
		if (Social.localUser.authenticated){
			PlayGamesPlatform.Instance.IncrementAchievement(GPGConfigs.achievement_invert_picker, 1, null);
			PlayGamesPlatform.Instance.IncrementAchievement(GPGConfigs.achievement_invert_mania, 1, null);
		}

		StuffItem si = BadStuffUse.Find( x => x.type == index );

		if ( si == null )
			BadStuffUse.Add( new StuffItem( index, Random.Range(2,9), InvertIMG, InvertText, (byte)BadStuffUse.Count, new OnOFFDel( playerManager.InvertOnOff ) ) );
		else
			si._Time += Random.Range(2,9);
	}

	public void PickFast( int index ){
		if (Social.localUser.authenticated){
			PlayGamesPlatform.Instance.IncrementAchievement(GPGConfigs.achievement_fast_picker, 1, null);
			PlayGamesPlatform.Instance.IncrementAchievement(GPGConfigs.achievement_fast_mania, 1, null);
		}

		StuffItem si = GoodStuffUse.Find( x => x.type == (int)Stuff.GoodBadTypes.Slow );

		if ( si != null ){
			si._Time = 0;
			return;
		}

		si = BadStuffUse.Find( x => x.type == index );

		if ( si == null )
			BadStuffUse.Add( new StuffItem( index, Random.Range(2,9), FastIMG, FastText, (byte)BadStuffUse.Count, new OnOFFDel(FastOnOff) ) );
		else
			si._Time += Random.Range(2,9);
	}
	public void FastOnOff( bool trigger ){
		if ( trigger ){
			speedMutant = speed / 2;
			Camera.main.GetComponent<ColorCorrectionCurves>().saturation = 1.4f;
		}
		else{
			speedMutant = 0;
			Camera.main.GetComponent<ColorCorrectionCurves>().saturation = 1.0f;
		}
	}

	public void PickShield( int index ){
		if (Social.localUser.authenticated){
			PlayGamesPlatform.Instance.IncrementAchievement(GPGConfigs.achievement_shield_picker, 1, null);
			PlayGamesPlatform.Instance.IncrementAchievement(GPGConfigs.achievement_shield_mania, 1, null);
		}

		StuffItem si = GoodStuffUse.Find( x => x.type == index );

		if ( si == null )
			GoodStuffUse.Add( new StuffItem( index, GameManager.instance.GetUpgradeParam(index-1), ShieldIMG, ShieldText, (byte)GoodStuffUse.Count, new OnOFFDel(playerManager.ShieldOnOff) ) );
		else
			si._Time += GameManager.instance.GetUpgradeParam(index-1);
	}

	public void PickMagnet( int index ){
		if (Social.localUser.authenticated){
			PlayGamesPlatform.Instance.IncrementAchievement(GPGConfigs.achievement_magnet_picker, 1, null);
			PlayGamesPlatform.Instance.IncrementAchievement(GPGConfigs.achievement_magnet_mania, 1, null);
		}

		StuffItem si = GoodStuffUse.Find( x => x.type == index );

		if ( si == null )
			GoodStuffUse.Add( new StuffItem( index, GameManager.instance.GetUpgradeParam(index-1), MagnetIMG, MagnetText, (byte)GoodStuffUse.Count, new OnOFFDel(playerManager.MagnetOnOff) ) );
		else
			si._Time += GameManager.instance.GetUpgradeParam(index-1);
	}

	public void PickSlow( int index ){
		if (Social.localUser.authenticated){
			PlayGamesPlatform.Instance.IncrementAchievement(GPGConfigs.achievement_slow_picker, 1, null);
			PlayGamesPlatform.Instance.IncrementAchievement(GPGConfigs.achievement_slow_mania, 1, null);
		}

		StuffItem si = BadStuffUse.Find( x => x.type == (int)Stuff.GoodBadTypes.Fast );
		
		if ( si != null ){
			si._Time = 0;
			return;
		}
		
		si = GoodStuffUse.Find( x => x.type == index );
	

		if ( si == null )
			GoodStuffUse.Add( new StuffItem( index, GameManager.instance.GetUpgradeParam(index-1), SlowIMG, SlowText, (byte)GoodStuffUse.Count, new OnOFFDel(SlowOnOff) ) );
		else
			si._Time += GameManager.instance.GetUpgradeParam(index-1);
	}
	public void SlowOnOff( bool trigger ){
		if ( trigger ){
			speedMutant = -speed / 2;
			Camera.main.GetComponent<ColorCorrectionCurves>().saturation = 0.6f;

		}
		else{
			speedMutant = 0;
			Camera.main.GetComponent<ColorCorrectionCurves>().saturation = 1f;
		}
	}

	public void PlayerDie(){

		if ( GameOverPanel.activeSelf || playerManager.ShieldON ) return;

		Handheld.Vibrate();

		GameManager.instance.Points = Points;
		GameManager.instance.SaveData();

		GameManager.instance.PlayMusic( null );
		GameManager.instance.PlaySound( GameOverSound );

		GameOverPanel.SetActive( true );
		GameOverPanel.transform.FindChild("Score").GetComponent<Text>().text = "SCORE: " + Mathf.Round( Score ).ToString();

		if ( (int)Mathf.Round( Score ) > GameManager.instance.Score ){
			GameOverPanel.transform.FindChild("NewScore").gameObject.SetActive( true );
			GameManager.instance.Score = (int)Mathf.Round( Score );
			if (Social.localUser.authenticated)
				Social.ReportScore( (long)Score, GPGConfigs.leaderboard_rating, (bool success) => {});
		}

		Time.timeScale = 0;

	}

	public void PickFuel(){

		m_newSpeed = speed;

		if ( m_newSpeed + plane_accelerate < maxSpeed )
			m_newSpeed += plane_accelerate;
		else m_newSpeed = maxSpeed;

		if ( SpeedUpSFX.isStopped ) SpeedUpSFX.Play();

		m_waitSFX = 1f;

		GameManager.instance.PlaySound( FuelSound );
	}

	public void Swipe(){

		if ( m_waitSFX > 0 ) return;

		m_newSpeed = speed;

		m_newSpeed -= ( 5 - plane_glide/2 );

		if ( m_newSpeed < minSpeed ) m_newSpeed = minSpeed;

		if ( SpeedUpSFX.isStopped ) SpeedUpSFX.Play();

		m_waitSFX = 0.3f;
	}

	#if DEBUG_MODE
	void OnGUI(){
		GUI.Label( new Rect(0,750,475,50), ((int)gameTime).ToString() );
	}
	#endif

	void Update () {
        if (Paused) return;

		if ( m_waitSFX > 0 ){
			speed = Mathf.Lerp( speed, m_newSpeed, Time.deltaTime );
			m_waitSFX -= Time.deltaTime;
		}
		else{
			if ( SpeedUpSFX.isPlaying ) SpeedUpSFX.Stop();

			if ( speed > minSpeed )
				speed -= Time.deltaTime * ( (10 - plane_glide) / 5 );
			else{ 
				speed = minSpeed;
				PlayerDie();
			}

			if ( m_fuelTimer > 0 ){
				m_fuelTimer -= Time.deltaTime;
			}

		}
		
		SpeedIndicator.value = speed;

		gameTime += Time.deltaTime;

		if ( speed > scoreSpeed ){

			if ( LowSpeed.activeSelf ){
				LowSpeed.SetActive( false );
			}

			Score += Time.deltaTime * ( (speed - scoreSpeed) / 10);

			if (Social.localUser.authenticated){
				if ( Score >= 500 ) Social.ReportProgress(GPGConfigs.achievement_master, 100f, null);
				else if ( Score >= 250 ) Social.ReportProgress(GPGConfigs.achievement_gamer, 100f, null);
				else if ( Score > 100 ) Social.ReportProgress(GPGConfigs.achievement_beginner, 100f, null);
			}
		}
		else{
			if ( !LowSpeed.activeSelf ){
				LowSpeed.SetActive( true );
			}
		}

		ScoreGUI.text = "SCORE: " + Mathf.Round( Score ).ToString();

		if ( Score > GameManager.instance.Score && OldScoreGUI.color != Color.gray ){
			ScoreGUI.color = OldScoreGUI.color;
			OldScoreGUI.color = Color.gray;
		}

		//speed+= 0.2f * Time.deltaTime;

		List<StuffItem> stuffForRemove = new List<StuffItem>();

		foreach( StuffItem si in GoodStuffUse ){
			si._Time -= Time.deltaTime;
			
			if ( si._Time <= 0 ) stuffForRemove.Add( si.Remove() );

		}

		foreach( StuffItem si in stuffForRemove ) GoodStuffUse.Remove(si);
		for( byte i = 0; i < GoodStuffUse.Count; i++ ) GoodStuffUse[i].Reshift( i );

		stuffForRemove = new List<StuffItem>();
		
		foreach( StuffItem si in BadStuffUse ){
			si._Time -= Time.deltaTime;
			
			if (si._Time <= 0) stuffForRemove.Add( si.Remove() );
		}
		
		foreach( StuffItem si in stuffForRemove ) BadStuffUse.Remove(si);
		for( byte i = 0; i < BadStuffUse.Count; i++ ) BadStuffUse[i].Reshift( i );

		fixCount -= Time.deltaTime;
		
		if  ( fixCount <= 0 ) Spawn ();
	}

	void Spawn(){

		fixCount = 3;

		if ( MultipleStuff > 0 ){
			MultipleStuff--;
			return;
		}
		/*
		Vector2 GoodBadStuff; //X - хороший 1 / Y - плохой 2
		GoodBadFill( out GoodBadStuff );
		*/

		List<Vector2> freeSlots;
		FreeSlots( out freeSlots );
		if (freeSlots.Count == 0) return;

		Vector2 index = RandomIndex( freeSlots );

		if ( m_fuelTimer <= 0 ){
			AddFuel( index );
			m_fuelTimer = 1.5f;
		}

		if ( Random.value > 0.1f ){ //ВЫСТАВИТЬ в 0.1

			int count;

			if ( Random.value < 0.1f && gameTime > 150 ){ //ВЫСТАВИТЬ!!! 150

				if ( gameTime > 400 ){ //ВЫСТАВИТЬ!!! 400
					count = Random.Range( 0, 20 );
					if ( count < 17 ) count = 17;
				}
				else
					count = 17;

				GameObject[] fasts = new GameObject[count - 16];
				for( int i = 17; i <= count; i++ ){
					if ( i > 17 ){ MultipleStuff++;
						FreeSlots( out freeSlots );
						index = RandomIndex( freeSlots );
					}

					Destroy( GameObject.Instantiate( warningPref, new Vector3((index.x-1)*Step,(index.y-1)*Step, 12 ), Quaternion.Euler( new Vector3(90,180,0) ) ), 1.5f );
					fasts[count - i] = CreateStuff( StuffType.Bad, index, fastStuff, true );
				}

                fixCount = 1.5f;
                StartCoroutine( FastCreateWait( fasts ) );

				return;
			}
			
			CreateStuff( StuffType.Bad, index, badStuff );

			if ( canGoodStuff > 0 ) canGoodStuff--;
			AddPoints(index);

			count = (int)Random.Range( 0f, Mathf.Min( Mathf.Ceil((gameTime+50) / 75f), 6f ) );

			for( int i = 0; i < count; i++ ){
				FreeSlots( out freeSlots );
				if (freeSlots.Count == 0) return;

				MultipleStuff++;
				index = RandomIndex( freeSlots );
				CreateStuff( StuffType.Bad, index, badStuff);
			}
		}
		else
		if ( goodStuffCount < 2 && canGoodStuff == 0 ){
			goodStuffCount++;
			CreateStuff( StuffType.Good, index, goodStuff );
			canGoodStuff = 5;
		}
		else{
			CreateStuff( StuffType.Bad, index, badStuffEffect, false, true );
			if ( canGoodStuff > 0 ) canGoodStuff--;
			AddPoints(index);
		}

	}

	IEnumerator FastCreateWait( GameObject[] fasts ){
		yield return new WaitForSeconds(1.5f);

		foreach( GameObject fast in fasts ){
			fast.SetActive( true );
		}

        yield return null;
	}

	private GameObject CreateStuff( StuffType type, Vector2 index, GameObject[] stuffs, bool isFast = false, bool badEffect = false ){
		setka[(byte)index.x,(byte)index.y] = type;
		int stuffIndex = Random.Range(0,stuffs.Length);

		GameObject stuff = (GameObject)Instantiate( stuffs[stuffIndex], new Vector3((index.x-1)*Step,(index.y-1)*Step,spawnDistance), Quaternion.LookRotation(Vector3.back) );
		stuff.transform.parent = transform;
		stuff.name = "stuff_"+index.x.ToString()+"_"+index.y.ToString();
		Stuff stuffComp = (Stuff)stuff.AddComponent(typeof(Stuff));
		stuffComp.location = index;
		stuffComp.isFast = isFast;
		stuffComp.isGood = type == StuffType.Good ? true : false;
		stuffComp.badEffect = badEffect;
		stuffComp.setType( ++stuffIndex );
		AllStuff.Add( stuff );

		return stuff;
	}

	private void AddFuel( Vector2 index ){
		GameObject fuel = (GameObject)Instantiate( fuelPref, new Vector3((index.x-1)*Step,(index.y-1)*Step,spawnDistance + Random.Range(-10, -20)), Quaternion.LookRotation(Vector3.back) );
		fuel.transform.parent = transform;
		Stuff fuelComp = (Stuff)fuel.AddComponent(typeof(Stuff));
		fuelComp.location = index;
		fuelComp.isGood = true;
		fuelComp.setType( 10 );
		AllStuff.Add( fuel );
	}
	
	private byte counter = 0;
	void AddPoints( Vector2 index ){

		counter++;

		if ( counter < 5 ) return;

		counter = 0;

		for( int i = 1; i <= 5; i++){
			GameObject point = (GameObject)Instantiate( pointPref, new Vector3((index.x-1)*Step,(index.y-1)*Step,spawnDistance - i * 2 - 10 ), Quaternion.LookRotation(Vector3.back) );
			point.transform.FindChild("mesh").GetComponent<Animation>()["rotationY"].time = Random.Range ( 0, 60f );
			point.transform.parent = transform;
			point.name = "point_"+i.ToString();
			Stuff pointComp = (Stuff)point.AddComponent(typeof(Stuff));
			pointComp.location = index;
			pointComp.isGood = true;
			pointComp.setType( 0 );
			pointComp.PlayerTransform = playerManager.transform;
			AllStuff.Add( point );
		}
	}
	/*
	void GoodBadFill( out Vector2 goodBad ){
		goodBad = new Vector2();

		foreach( GameObject stuff in AllStuff ){
			if (stuff.GetComponent<Stuff>().isGood) goodBad.x++;
			else goodBad.y++;
		}
	}
	*/

	public float GetSpeed( bool fast = false ){
		if ( fast )
			return maxSpeed * 2;

		return speed + speedMutant;
	}

	void FreeSlots( out List<Vector2> freeSlots ){

		freeSlots = new List<Vector2>();

		for(int i = 0; i < 3; i++)
		for(int j = 0; j < 3; j++){
			if( setka[i,j] == StuffType.None )
				freeSlots.Add( new Vector2(i,j) );
		}
	}

	Vector2 RandomIndex( List<Vector2> slots ){
		return slots[ Random.Range( 0, slots.Count ) ];
	}

	//GUI Functions

	public void OnMusicToggle(){
		GameManager.instance.SetMusic( MusicToggleGUI.isOn );
	}

	public void OnSoundToggle(){
		GameManager.instance.SetSound( SoundToggleGUI.isOn );
	}

	public void OnCloudToggle(){
		GameManager.instance.SFX = SFXToggleGUI.isOn;
		clouds.enabled = SFXToggleGUI.isOn;
	}

	public void Pause(){

		Paused = !Paused;

		PausePanel.SetActive(Paused);

		if (Paused){
			PauseBtn.image.sprite = ResumeSprite;
			StopCoroutine("ExitPause");
			WaiteGUI.enabled = false;

			GameManager.instance.MusicPitch( 0.2f );

			Time.timeScale = 0;
		}
		else{
			PauseBtn.image.sprite = PauseSprite;
			StartCoroutine("ExitPause");
		}

	}

	void OnApplicationPause(bool pauseStatus){
		if (pauseStatus && !Paused) Pause();
	}

	IEnumerator ExitPause(){
		WaiteGUI.enabled = true;
		WaiteGUI.text = "3";
		Time.timeScale = 0.05f;
		GameManager.instance.MusicPitch( 0.4f );
		yield return new WaitForSeconds(0.05f);
		//Time.timeScale = 0.4f;
		WaiteGUI.text = "2";
		GameManager.instance.MusicPitch( 0.6f );
		yield return new WaitForSeconds(0.05f);
		//Time.timeScale = 0.6f;
		WaiteGUI.text = "1";
		GameManager.instance.MusicPitch( 0.8f );
		yield return new WaitForSeconds(0.05f);
		WaiteGUI.enabled = false;
		Time.timeScale = 1;
		GameManager.instance.MusicPitch( 1 );
		yield break;
	}

	public void ShowAds(){
		if (Advertisement.IsReady(null)){
			Advertisement.Show(null, new ShowOptions {
                resultCallback = OnAdsResult
            });
		}
		else{
            AndroidNativeUtils.ShowMsg("sorry, video not available");
		}
	}

	void OnAdsResult( ShowResult result ){
		switch( result ){
		    case ShowResult.Failed: AndroidNativeUtils.ShowMsg("sorry, video not available, try later"); break;
		    case ShowResult.Skipped: AndroidNativeUtils.ShowMsg("you skipped video!"); break;
		    case ShowResult.Finished: ExtraLife(); break;
		}
	}

	void ExtraLife(){
		GameOverPanel.transform.FindChild("AdsBTN").gameObject.SetActive(false);
		GameManager.instance.PlayMusic( MusicSound );
		GameOverPanel.SetActive( false );
		speed = (minSpeed + maxSpeed) / 2;
		Pause();
	}

	public void Retry(){
		Time.timeScale = 1;
		LevelToLoad = 1;
		StartCoroutine("LoadLevel");
	}

	public void ExitToMainMenu(){
		LevelToLoad = 0;
		StartCoroutine("LoadLevel");
	}

	private int pRounded;
	private byte LevelToLoad = 0;
	IEnumerator LoadLevel(){
		AsyncOperation async = SceneManager.LoadSceneAsync(LevelToLoad);
		
		while(async.isDone == false) {
			float p = async.progress * 100f;
			pRounded = Mathf.RoundToInt(p);
			string perc = pRounded.ToString();  
			
			yield return true;
		}
	}

	private class StuffItem{

		public int type;

		private float time;
		public float _Time{
			get{
				return time;
			}
			set{
				time = value;
				text.text = Mathf.Ceil( time ).ToString();
			}
		}
		public Image img;
		public Text text;
		public OnOFFDel callback;
		
		public StuffItem( int _type, float _time, Image _img, Text _text, byte shift, OnOFFDel _callback = null ){
			type = _type;
			time = _time;
			img = _img;
			text = _text;
			callback = _callback;

			if ( type >= 0 ){
				GameManager.instance.PlaySound( LevelManager.instance.GoodStuffSound );
				img.rectTransform.anchoredPosition = new Vector3( shift * -32 - 10, img.rectTransform.anchoredPosition.y, 0);
			}
			else{
				GameManager.instance.PlaySound( LevelManager.instance.BadStuffSound );
				img.rectTransform.anchoredPosition = new Vector3( shift * 32 + 10, img.rectTransform.anchoredPosition.y, 0);
			}
			img.enabled = true;
			text.text = time.ToString();
			text.enabled = true;

            if ( callback != null ) callback( true );
		}

		public StuffItem Remove( ){
			img.enabled = false;
			text.enabled = false;
			if ( callback != null ) callback( false );

			return this;
		}

		public void Reshift( byte shift ){
			if ( type >= 0 )
				img.rectTransform.anchoredPosition = new Vector3( shift * -32 - 10, img.rectTransform.anchoredPosition.y, 0);
			else
                img.rectTransform.anchoredPosition = new Vector3( shift * 32 + 10, img.rectTransform.anchoredPosition.y, 0);
		}
	}

}
