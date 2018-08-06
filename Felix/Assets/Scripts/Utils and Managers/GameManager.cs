using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System;
using System.Collections;
using System.Collections.Generic;

public class GameManager : MonoBehaviour {

    private ProtectedUint MAX_MILK = new ProtectedUint("6");
    private ProtectedUint FILL_TIME = new ProtectedUint("30000"); //время необходимое для появления 1 молока в миллисекундах
    [HideInInspector]
    public uint[] FISH_COST = { 1, 2, 5, 10 };
    [HideInInspector]
    public uint[] ITEM_COST = { 2, 3, 4, 5, 6, 1, 8, 9 };
    [HideInInspector]
    public uint[] MILK_COST = { 10, 18, 26, 30, 35, 40 };

    [HideInInspector]
    public static GameManager instance = null;

    [HideInInspector]
    public static bool LOADED_FROM_MENU = true;

    private Text LoadingTextComponent;
    private string loadingText = "LOADING";

    [HideInInspector]
    public LoadingImg loading_img;

    public Sprite[] star_sprites;

    [HideInInspector]
    public uint star_count;
    [HideInInspector]
    public uint score;
    [HideInInspector]
    public uint[] milk_pos = new uint[6];

    [HideInInspector]
    public List<List<uint[]>> levels;
    [HideInInspector]
    public ProtectedUint fish = new ProtectedUint();
    [HideInInspector]
    public ProtectedUint milk = new ProtectedUint();
    [HideInInspector]
    public ProtectedUint new_milk = new ProtectedUint();
    [HideInInspector]
    public uint[] items;
    [HideInInspector]
    public int[] bag;
    [HideInInspector]
    public int[] level_params;
    public long all_score;

    private double saved_time;

    [HideInInspector]
    public bool sound_on;
    [HideInInspector]
    public bool music_on;

    [HideInInspector]
    public int cur_stage;
    [HideInInspector]
    public int cur_level;
    [HideInInspector]
    public int cur_mode;

    [HideInInspector]
    public bool new_record;
    [HideInInspector]
    public uint add_level_fish = 0;

    [HideInInspector]
    public MilkPanel milk_panel;

    [HideInInspector]
    public bool pause = false;

    void Awake () {
        if (instance == null)
        {
            instance = this;
        }
        else if (instance != this) {
            Destroy(instance);
        }

        Application.targetFrameRate = 60;

        DontDestroyOnLoad(gameObject);
	}

    void Start() {
        LoadingTextComponent = GameObject.Find("LoadingText").GetComponent<Text>();

        if (!Application.genuine)
        {
            LoadingTextComponent.text = "Hack was detected.\nPlease, reinstall the app.";
            return;
        }

        InternetChecker.CheckInternetConnect();

        StartCoroutine("FirstLoading");
        LoadState();
    }

    private AsyncOperation async;
    IEnumerator FirstLoading()
    {
        async = SceneManager.LoadSceneAsync("MainMenu");
        async.allowSceneActivation = false;

        while (async.isDone == false)
        {
            float p = async.progress * 100f;
            float pRounded = Mathf.RoundToInt(p);

            LoadingTextComponent.text = loadingText + " " + pRounded.ToString() + "%";

            yield return true;
        }
    }

    public IEnumerator LoadLevel(string level_name, float waite = 0)
    {
        loading_img.Activate();
        pause = true;
        AsyncOperation async = SceneManager.LoadSceneAsync(level_name);
        async.allowSceneActivation = false;
        yield return new WaitForSeconds(waite);
        async.allowSceneActivation = true;
        loading_img.DeActivate();
        yield return true;
    }

    public IEnumerator waitThenCallback(float time, Action callback)
    {
        yield return new WaitForSeconds(time);
        callback();
    }

    private void LoadState() {

        DataToSave obj = (DataToSave)SaveLoad.LoadData("save.dat");

        if (obj == null)
        {
            LoadLevels();
            all_score = 0;
            cur_stage = 0;
            cur_level = -1;
            items = new uint[8];
            bag = new int[] { -1, -1, -1 };
            fish.Value = "0";
            milk.Copy(MAX_MILK.Value);
            new_milk.Value = "0";

            PlayerPrefs.DeleteAll();

            LanguageManager.AutoDetectLanguage();

            LoadingTextComponent.text = loadingText = LanguageManager.GetText("LOADING");

            music_on = true;
            sound_on = true;

            PlayerPrefs.SetInt("rate", 0);
            RatePanel.LaunchApp();

            SaveData();
            async.allowSceneActivation = true;

            return;
        }

        levels = obj.levels;
        all_score = obj.all_score;
        cur_stage = obj.cur_stage;
        cur_level = obj.cur_level;
        fish.Value = obj.fish.ToString();
        milk.Value = obj.milk.ToString();
        new_milk.Value = obj.new_milk.ToString();
        items = obj.items;
        bag = obj.bag;
        saved_time = obj.time;

        LanguageManager.LoadLanguage(PlayerPrefs.GetInt("language"));
        music_on = PlayerPrefs.GetInt("music") == 1 ? true : false;
        sound_on = PlayerPrefs.GetInt("sound") == 1 ? true : false;

        if (PlayerPrefs.HasKey("rate")) RatePanel.LaunchApp();

        if (milk + new_milk < MAX_MILK) {

            uint add_new_milk = (uint)Mathf.Min( Mathf.Floor( (int)((DateTime.Now - new DateTime(1970, 1, 1)).TotalMilliseconds - saved_time) / FILL_TIME.GetConverted<int>()), (MAX_MILK - (milk + new_milk)).GetConverted<uint>() );

            new_milk += add_new_milk;

            //включаем таймер
            if (milk + new_milk < MAX_MILK)
            {
                double start_time = FILL_TIME.GetConverted<int>() - (((DateTime.Now - new DateTime(1970, 1, 1)).TotalMilliseconds - saved_time) - (new_milk * FILL_TIME).GetConverted<int>() );
                Timers.AddTimer("milk", (int)start_time, 0, 1000, MilkTick, NewMilkAppear);
            }
        }

        async.allowSceneActivation = true;
    }
    
    private void MilkTick()
    {
        if (milk_panel!=null)
        {
            milk_panel.SetMilkTimer(Timers.GetTime("milk"));
        }
    }

    private void NewMilkAppear()
    {
        new_milk++;
        if (milk_panel != null)
        {
            milk_panel.SetMilkCount(milk.GetConverted<uint>(), new_milk.GetConverted<uint>());
            //добавляем бутылку молока на сцену
            if (SceneManager.GetActiveScene().name == "ChooseLevel") Camera.main.GetComponent<LevelManager>().AddNewMilk(new_milk.GetConverted<uint>()-1);
        }

        CheckNewMilk();
    }

    private void CheckNewMilk()
    {
        if (milk + new_milk < MAX_MILK)
        {
            if (!Timers.HasTimer("milk"))
            {
                saved_time = (DateTime.Now - new DateTime(1970, 1, 1)).TotalMilliseconds;
                Timers.AddTimer("milk", FILL_TIME.GetConverted<int>(), 0, 1000, MilkTick, NewMilkAppear);
            }
        }
        else
        {
            if (Timers.HasTimer("milk")) Timers.RemoveTimer("milk");
            //milk_panel.SetMilkTimer(0);
        }

        SaveData();
    }

    public void BuyMilk( uint count )
    {
        milk += count;
        milk_panel.SetMilkCount( milk.GetConverted<uint>(), new_milk.GetConverted<uint>() );

        if (milk + new_milk == MAX_MILK)
        {
            Timers.RemoveTimer("milk");
            milk_panel.SetMilkTimer(0);
        }

    }

    public bool UseMilk()
    {
        if (milk > 0)
        {
            milk--;

            if (MAX_MILK - milk == 1)
                saved_time = (DateTime.Now - new DateTime(1970, 1, 1)).TotalMilliseconds;

            SaveData();

            if (!Timers.HasTimer("milk"))
                Timers.AddTimer("milk", FILL_TIME.GetConverted<int>(), 0, 1000, MilkTick, NewMilkAppear);

            if (milk_panel != null)
            {
                milk_panel.UseMilk();
            }

            return true;
        }
        else
        {
            if (milk == 0)
            {
                if (milk_panel != null) {
                    if (new_milk > 0) milk_panel.FlashNewMilk();
                    else milk_panel.BuyMilkCLICK();
                }
            }

            return false;
        }
    }

    public void ReturnMilk()
    {
        if (new_milk > 0)
        {
            new_milk--;
            milk_pos[new_milk.GetConverted<uint>()] = 0;
        }

        milk++;

        CheckNewMilk();
    }

    public void TakeNewMilk( )
    {
        new_milk--;
        milk_pos[new_milk.GetConverted<uint>()] = 0;
        milk++;
        milk_panel.SetMilkCount(milk.GetConverted<uint>(), new_milk.GetConverted<uint>());
    }

    public int MissMilk()
    {
        return MAX_MILK.GetConverted<int>() - ( milk.GetConverted<int>() + new_milk.GetConverted<int>() );
    }

    public uint GetCurScore()
    {
        return levels[cur_stage][cur_level][cur_mode+3];
    }

    public uint GetCurStars()
    {
        return levels[cur_stage][cur_level][cur_mode];
    }

    public void LevelComplete(uint star_count, uint score)
    {
        this.star_count = star_count;
        this.score = score + star_count*50;

        add_level_fish = 0;

        if (star_count > levels[cur_stage][cur_level][cur_mode])
        {
            if (star_count >= 1 && levels[cur_stage][cur_level][cur_mode] < 1) add_level_fish += 3;
            if (star_count >= 2 && levels[cur_stage][cur_level][cur_mode] < 2) add_level_fish += 5;
            if (star_count == 3) {
                add_level_fish += 12;
#if UNITY_ANDROID
                if (cur_stage == 0) CheckFloorAch();
#endif
            }

            levels[cur_stage][cur_level][cur_mode] = star_count;
            PlayerPrefs.SetInt("level_complete", (int)star_count);

            fish += add_level_fish;
        }

        if (this.score > levels[cur_stage][cur_level][cur_mode + 3])
        {
            levels[cur_stage][cur_level][cur_mode + 3] = this.score;
            new_record = true;

            all_score += this.score;
            SocialsPlatform.ReportScore(all_score);
        }
        else
            new_record = false;
    }

    private void CheckFloorAch()
    {
        if (levels[cur_stage][cur_level][0] == 3 && levels[cur_stage][cur_level][1] == 3 && levels[cur_stage][cur_level][0] == 3)
        {
            SocialsPlatform.AddAchievement((string)typeof (GameServicesClass).GetField("ACH_"+(15-cur_level)+"ST_FLOOR_KING").GetValue(null));
        }
    }

    /*
    public bool NextLevel()
    {
        if (cur_mode >= 2)
        {
            cur_mode = 0;

            if (cur_level + 1 >= levels[cur_stage].Count)
            {
                //переходим на новый stage
                cur_level = 0;
                cur_stage += cur_stage + 1 >= levels.Count ? 0 : 1;

                return false;
            }
            else
                cur_level++;
        }
        else
            cur_mode++;

        return true;
    }
    */

    private void LoadLevels() {
        TextAsset info = (TextAsset) Resources.Load("Levels/info", typeof(TextAsset));
        JSONObject json = new JSONObject(info.text);

        //каждый параметр указывается на количество уровней в стейдже.

        levels = new List<List<uint[]>>();

        foreach (JSONObject j in json.list[0].list)
        {
            levels.Add(new List<uint[]>());
            for (int i = 0; i < (int)j.n; i++)
            {
                levels[levels.Count - 1].Add( new uint[6] {0, 0, 0, 0, 0, 0} );
            }
        }
    }
    
    public void SaveSettings()
    {
        PlayerPrefs.SetInt("language", (int)LanguageManager.curLanguage);
        PlayerPrefs.SetInt("music",music_on ? 1 : 0);
        PlayerPrefs.SetInt("sound",sound_on ? 1 : 0);
        PlayerPrefs.Save();
    }

    public void SaveData()
    {
        DataToSave data = new DataToSave();
        data.levels = levels;
        data.all_score = all_score;
        data.cur_stage = cur_stage;
        data.cur_level = cur_level;
        data.fish = fish.GetConverted<uint>();
        data.milk = milk.GetConverted<uint>();
        data.new_milk = new_milk.GetConverted<uint>();
        data.items = items;
        data.bag = bag;
        data.time = saved_time;

        SaveLoad.SaveData(data, "save.dat");
    }

#if UNITY_EDITOR
    private void OnApplicationQuit()
    {
        PlayerPrefs.DeleteKey("level_complete");
        SaveData();
        SaveSettings();
    }
#endif

    void OnApplicationPause(bool pauseStatus)
    {
        pause = pauseStatus;

        if (pauseStatus)
        {
            PlayerPrefs.DeleteKey("level_complete");
            SaveData();
            SaveSettings();
        }
        else {
            InternetChecker.CheckInternetConnect();

            if (Facebook.Unity.FB.IsInitialized)
            {
                Facebook.Unity.FB.ActivateApp();
            }
            else
            {
                Facebook.Unity.FB.Init(() => {
                    Facebook.Unity.FB.ActivateApp();
                });
            }
        }
    }

}

[Serializable]
class DataToSave
{
    public List<List<uint[]>> levels;
    public long all_score;
    public int cur_stage;
    public int cur_level;
    public uint fish;
    public uint milk;
    public uint new_milk;
    public uint[] items;
    public int[] bag;
    public double time;
}