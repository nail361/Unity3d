using UnityEngine;
#if UNITY_IOS || UNITY_ANDROID
using UnityEngine.Advertisements;
#endif
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System;
using System.Text;

public class GameManager : MonoBehaviour {

    public static int FinishPoints = 0;
    public static bool RandomMap = false;
    public static bool ProfiMode = false;

    [SerializeField]
    private GameObject questionPref;

    [HideInInspector]
    public LoadingPanel loading_panel;

    public static GameManager instance;

    public static List<string> teams;
    public static List<string[]> players;

    public static int cur_team = 0;

    public static bool multi_game = false;
    public static int difficult = 0;

    public static List<string> easy_words_tell;
    public static List<string> easy_words_draw;
    public static List<string> easy_words_show;

    public static List<string> medium_words_tell;
    public static List<string> medium_words_draw;
    public static List<string> medium_words_show;

    public static List<string> hard_words_tell;
    public static List<string> hard_words_draw;
    public static List<string> hard_words_show;

    private bool firstStart = false;

#if UNITY_IOS || UNITY_ANDROID
    private string gameId = "1052054";
#endif

    void Awake()
    {
#if UNITY_IOS
        Environment.SetEnvironmentVariable("MONO_REFLECTION_SERIALIZER", "yes");
#endif

        if (instance != null)
        {
            Destroy(gameObject);
            return;
        }

        instance = this;
        DontDestroyOnLoad(gameObject);

        Screen.sleepTimeout = SleepTimeout.NeverSleep;

#if UNITY_IOS
        gameId = "1052053";
        InitAds();
#elif UNITY_ANDROID
        gameId = "1052054";
        InitAds();
#endif

        teams = new List<string>();
        players = new List<string[]>();

        Application.targetFrameRate = 60;
    }

    void Start()
    {
        CheckFirstStart();
    }

    private void CheckFirstStart()
    {
        FileInfo info = new FileInfo(Application.persistentDataPath + "/version.dat");

        if (info == null || info.Exists == false)
        {
            if (Application.internetReachability == NetworkReachability.NotReachable)
            {
                firstStart = true;
                loading_panel.ShowLoading("Внимание!", "для первого запуска приложения необходим доступ к интернету");
            }
            else
            {
                firstStart = false;
                StartCoroutine("UpdateWords");
            }
        }
        else
        {
            loading_panel.HideLoading();
            LoadWords();
        }
    }

    public void StartCheckUpdate()
    {
        StartCoroutine("CheckUpdate");
    }

    public void UpdateWords()
    {
        StartCoroutine("DownloadWords");
    }

    private static void LoadWords(List<string> array = null, int index = 0, Game.GAME_TYPE type = Game.GAME_TYPE.DRAW)
    {
        string words = "";

        using (StreamReader reader = new StreamReader(Application.persistentDataPath + "/words.dat")) {

            byte[] decodedBytes = Convert.FromBase64String(reader.ReadToEnd());
            words = Encoding.UTF8.GetString(decodedBytes);

            reader.Close();
        }

        string[] difficults = words.Split('#');

        if (array == null)
        {
            easy_words_tell = new List<string>(difficults[0].Split(','));
            easy_words_draw = new List<string>(difficults[1].Split(','));
            easy_words_show = new List<string>(difficults[2].Split(','));

            medium_words_tell = new List<string>(difficults[3].Split(','));
            medium_words_draw = new List<string>(difficults[4].Split(','));
            medium_words_show = new List<string>(difficults[5].Split(','));

            hard_words_tell = new List<string>(difficults[6].Split(','));
            hard_words_draw = new List<string>(difficults[7].Split(','));
            hard_words_show = new List<string>(difficults[8].Split(','));
        }
        else
            array = new List<string>(difficults[index + (int)type].Split(','));
    }

    private IEnumerator CheckUpdate()
    {
        WWW www;

        int cur_ver = 0;

        loading_panel.ShowLoading("Проверка наличия обновлений...", "пожалуйста подождите");

        FileInfo info = new FileInfo(Application.persistentDataPath + "/version.dat");
        
        if (info != null && info.Exists)
        {
            using (StreamReader reader = new StreamReader(info.FullName))
            {
                cur_ver = Convert.ToInt32(reader.ReadLine());
                reader.Close();
            }
        }

        www = new WWW("https://nail361.github.io/nail361/Activity/version.txt");

        while (!www.isDone)
        {
            yield return null;
        }

        if (Convert.ToInt32(www.text) > cur_ver)
        {
            loading_panel.HideLoading();
            Instantiate(questionPref);
        }
        else
        {
            loading_panel.ShowLoading("В настоящий момент обновлений нет", "попробуйте проверить позже");
            Invoke("HideLoading", 1.5f);
        }
    }

    private void HideLoading()
    {
        loading_panel.HideLoading();
    }

    private IEnumerator DownloadWords()
    {
        WWW www;
        int i = 0;

        loading_panel.ShowLoading("Обновление слов...","пожалуйста подождите");

        while (i < 2)
        {
            string path = "";

            switch (i)
            {
                case 0: path = "words"; break;
                case 1: path = "version"; break;
            }

            FileInfo info = new FileInfo(Application.persistentDataPath + "/" + path + ".dat");
            
            if (info != null && info.Exists)
                info.Delete();

            www = new WWW("https://nail361.github.io/nail361/Activity/" + path + ".txt");

            while (!www.isDone)
            {
                yield return null;
            }

            string www_text = www.text;

            if (i == 0) {
                byte[] bytesToEncode = Encoding.UTF8.GetBytes(www_text);
                www_text = Convert.ToBase64String(bytesToEncode);
            }

            File.WriteAllText(Application.persistentDataPath + "/" + path + ".dat", www_text, Encoding.UTF8);

            i++;
        }

        LoadWords();

        loading_panel.HideLoading();
    }

#if UNITY_IOS || UNITY_ANDROID
    private void InitAds()
    {
        if (Advertisement.isSupported && !Advertisement.isInitialized)
        {
            Advertisement.Initialize(gameId, false);
        }
    }
#endif

    public static void FillTeam(string team_name, string[] _players)
    {
        teams.Add(team_name);
        players.Add(_players);
    }

    public static void ResetTeams()
    {
        teams.Clear();
        players.Clear();
    }

    public static void NextTurn(int winner)
    {
        if (winner >= 0)
        {
            Game.teams[winner].GoToPos(difficult);
            if (winner != cur_team)
                Game.teams[cur_team].GoToPos((int)Mathf.Ceil((4-difficult)/2) * -1);
        }
        else
        {
            Game.teams[cur_team].GoToPos((4-difficult) *-1);
        }

        cur_team++;

        if (cur_team >= teams.Count) cur_team = 0;
    }

    public static string GetWord(int diff, Game.GAME_TYPE type)
    {
        difficult = diff;

        string word = "";

        switch (diff)
        {
            case 1:
                switch (type)
                {
                    case Game.GAME_TYPE.TELL:
                        word = easy_words_tell[UnityEngine.Random.Range(0, easy_words_tell.Count)];
                        easy_words_tell.Remove(word);
                        if (easy_words_tell.Count == 0) LoadWords(easy_words_tell, diff - 1, type);
                        break;
                    case Game.GAME_TYPE.DRAW:
                        word = easy_words_draw[UnityEngine.Random.Range(0, easy_words_draw.Count)];
                        easy_words_draw.Remove(word);
                        if (easy_words_draw.Count == 0) LoadWords(easy_words_draw, diff - 1, type);
                        break;
                    case Game.GAME_TYPE.SHOW:
                        word = easy_words_show[UnityEngine.Random.Range(0, easy_words_show.Count)];
                        easy_words_show.Remove(word);
                        if (easy_words_show.Count == 0) LoadWords(easy_words_show, diff - 1, type);
                        break;
                }

                break;

            case 2:
                switch (type)
                {
                    case Game.GAME_TYPE.TELL:
                        word = medium_words_tell[UnityEngine.Random.Range(0, medium_words_tell.Count)];
                        medium_words_tell.Remove(word);
                        if (medium_words_tell.Count == 0) LoadWords(medium_words_tell, diff - 1, type);
                        break;
                    case Game.GAME_TYPE.DRAW:
                        word = medium_words_draw[UnityEngine.Random.Range(0, medium_words_draw.Count)];
                        medium_words_draw.Remove(word);
                        if (medium_words_draw.Count == 0) LoadWords(medium_words_draw, diff - 1, type);
                        break;
                    case Game.GAME_TYPE.SHOW:
                        word = medium_words_show[UnityEngine.Random.Range(0, medium_words_show.Count)];
                        medium_words_show.Remove(word);
                        if (medium_words_show.Count == 0) LoadWords(medium_words_show, diff - 1, type);
                        break;
                }

                break;
            case 3:
                switch (type)
                {
                    case Game.GAME_TYPE.TELL:
                        word = hard_words_tell[UnityEngine.Random.Range(0, hard_words_tell.Count)];
                        hard_words_tell.Remove(word);
                        if (hard_words_tell.Count == 0) LoadWords(hard_words_tell, diff - 1, type);
                        break;
                    case Game.GAME_TYPE.DRAW:
                        word = hard_words_draw[UnityEngine.Random.Range(0, hard_words_draw.Count)];
                        hard_words_draw.Remove(word);
                        if (hard_words_draw.Count == 0) LoadWords(hard_words_draw, diff - 1, type);
                        break;
                    case Game.GAME_TYPE.SHOW:
                        word = hard_words_show[UnityEngine.Random.Range(0, hard_words_show.Count)];
                        hard_words_show.Remove(word);
                        if (hard_words_show.Count == 0) LoadWords(hard_words_show, diff - 1, type);
                        break;
                }

                break;
        }

        return word;
    }

    void OnApplicationPause(bool pauseStatus)
    {
        if (!pauseStatus && firstStart)
        {
            CheckFirstStart();
        }
    }

}
