using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using UnityEngine.Advertisements;
using UnityEngine.SceneManagement;
#if UNITY_EDITOR
using UnityEditor;
#endif
using GooglePlayGames;
using UnityEngine.SocialPlatforms;

public class MainMenuScript : MonoBehaviour
{
    [SerializeField]
    private GameObject requestBuyPref;

    public GameObject MenuPanel;
    public GameObject LoadingPanel;

    public Image LoadingPlane;

    public Text LoadingText;
    public Text PointsCount;

    public AudioClip MainSound;
    public AudioClip BuySound;

    public Sprite BuyIMG;
    public Sprite SelectIMG;
    public Sprite SelectedIMG;
    public Sprite upON;

    public GameObject Shield;
    public GameObject Magnet;
    public GameObject Slow;

    public GameObject[] Planes;

    private string msg = "test mode";

    void Start()
    {

        if (!Social.localUser.authenticated)
        {
            ConnectToGoogleService();
        }

        if (Advertisement.isSupported)
        {
            if (!Advertisement.isInitialized)
            {
                Advertisement.Initialize("131624889", false);
            }
        }

        Time.timeScale = 1;
        GameManager.instance.MusicPitch(1f);

        GameManager.instance.PlayMusic(MainSound);

        GameManager.OnChangePoint += UpdatePointsText;

        FillPlanes();
        FillUpgrades();
        UpdatePointsText();
    }

    void OnDestroy()
    {
        GameManager.OnChangePoint -= UpdatePointsText;
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (MenuPanel.GetComponent<Animator>().GetInteger("State") == 0)
                Application.Quit();
            else HideSettings();
        }

    }

#if DEBUG_MODE
    void OnGUI()
    {
        if (GUI.Button(new Rect(0, 700, 50, 50), "reset"))
        {
            GameManager.instance.ResetData();
            FillPlanes();
            FillUpgrades();
        }

        if (GUI.Button(new Rect(60, 700, 50, 50), "points"))
        {
            GameManager.instance.BuyPoints();
        }

        GUI.Label(new Rect(150, 600, 330, 300), msg);
    }
#endif

    public void StartGame()
    {
        MenuPanel.SetActive(false);
        LoadingPanel.SetActive(true);

        StartCoroutine("launchLevel");
    }

    IEnumerator launchLevel()
    {
        AsyncOperation async = SceneManager.LoadSceneAsync(1);

        while (async.isDone == false)
        {
            float p = async.progress * 100f;
            float pRounded = Mathf.RoundToInt(p);

            LoadingText.text = "LOADING\n" + pRounded.ToString() + "%";
            //float newX = -160 + 3.2f * pRounded;

            float lerpX = Mathf.Lerp(LoadingPlane.rectTransform.anchoredPosition.x, 160, Time.deltaTime);

            LoadingPlane.rectTransform.anchoredPosition = new Vector2(lerpX, LoadingPlane.rectTransform.anchoredPosition.y);

            yield return true;
        }
    }

    public void ShowRating()
    {
        if (Social.localUser.authenticated)
            PlayGamesPlatform.Instance.ShowLeaderboardUI(GPGConfigs.leaderboard_rating);
        else
        {
#if UNITY_ANDROID
            AndroidNativeUtils.ShowMsg("Connect the internet!");
#endif
        }
    }

    public void ShowAchievements()
    {
        if (Social.localUser.authenticated)
            Social.Active.ShowAchievementsUI();
        else
        {
#if UNITY_ANDROID
            AndroidNativeUtils.ShowMsg("Connect the internet!");
#endif
        }
    }

    public void ShowSettings()
    {
        MenuPanel.GetComponent<Animator>().SetInteger("State", 1);
    }

    public void HideSettings()
    {
        MenuPanel.GetComponent<Animator>().SetInteger("State", 0);
    }

    private void FillPlanes()
    {

        for (byte i = 0; i < Planes.Length; i++)
            foreach (Transform child in Planes[i].transform)
            {
                switch (child.name)
                {
                    case "Lock":
                        if ((GameManager.instance.UnlockPlane >> (3 - i) & 1) == 1)
                            child.gameObject.SetActive(false);
                        else
                            child.gameObject.SetActive(true);
                        break;
                    case "params":
                        child.FindChild("CONTROL").gameObject.GetComponent<Slider>().value = GameManager.instance.GetPlaneControl(i + 1);
                        child.FindChild("GLIDE").gameObject.GetComponent<Slider>().value = GameManager.instance.GetPlaneGlide(i + 1);
                        child.FindChild("ACCELARATION").gameObject.GetComponent<Slider>().value = GameManager.instance.GetPlaneAcceleration(i + 1);
                        break;
                    case "ActionBTN":
                        if ((GameManager.instance.UnlockPlane >> (3 - i) & 1) == 1)
                        {
                            if (i + 1 != GameManager.instance.PlayerPlane)
                            {
                                child.gameObject.GetComponent<RectTransform>().sizeDelta = new Vector2(128, 64);
                                child.gameObject.GetComponent<Button>().image.sprite = SelectIMG;
                            }
                            else
                            {
                                child.gameObject.GetComponent<Button>().image.sprite = SelectedIMG;
                                child.gameObject.GetComponent<RectTransform>().sizeDelta = new Vector2(128, 64);
                            }
                        }
                        else
                        {
                            child.gameObject.GetComponent<RectTransform>().sizeDelta = new Vector2(64, 64);
                            child.gameObject.GetComponent<Button>().image.sprite = BuyIMG;
                        }
                        break;
                    case "Price":
                        if ((GameManager.instance.UnlockPlane >> (3 - i) & 1) == 0)
                        {
                            child.gameObject.SetActive(true);
                            child.FindChild("PriceText").gameObject.GetComponent<Text>().text = GameManager.instance.GetPlanePrice(i).ToString();
                        }
                        else
                            child.gameObject.SetActive(false);
                        break;
                }
            }
    }

    private void FillUpgrades()
    {
        foreach (Transform child in Shield.transform)
        {
            FillUpgrade(child, 0);
        }
        foreach (Transform child in Magnet.transform)
        {
            FillUpgrade(child, 1);
        }
        foreach (Transform child in Slow.transform)
        {
            FillUpgrade(child, 2);
        }
    }

    private void FillUpgrade(Transform child, int index)
    {
        switch (child.name)
        {
            case "Title":
                child.gameObject.GetComponent<Text>().text = GameManager.instance.GetUpgradeParam(index).ToString() + " sec.";
                break;
            case "upgrade_1":
                if (GameManager.instance.GetUpgradeLevel(index) > 0) child.gameObject.GetComponent<Image>().sprite = upON;
                break;
            case "upgrade_2":
                if (GameManager.instance.GetUpgradeLevel(index) > 1) child.gameObject.GetComponent<Image>().sprite = upON;
                break;
            case "upgrade_3":
                if (GameManager.instance.GetUpgradeLevel(index) > 2) child.gameObject.GetComponent<Image>().sprite = upON;
                break;
            case "upgrade_4":
                if (GameManager.instance.GetUpgradeLevel(index) > 3) child.gameObject.GetComponent<Image>().sprite = upON;
                break;
            case "upgrade_5":
                if (GameManager.instance.GetUpgradeLevel(index) > 4) child.gameObject.GetComponent<Image>().sprite = upON;
                break;
            case "UpgradeBTN":
                if (GameManager.instance.GetUpgradeLevel(index) == 5) child.gameObject.SetActive(false);
                else
                    child.Find("CostText").gameObject.GetComponent<Text>().text = GameManager.instance.GetUpgradeCost(index).ToString();
                break;
        }
    }

    private void UpdatePointsText()
    {
        PointsCount.text = GameManager.instance.Points.ToString();
    }

    public void onActionBTN()
    {

        if ((GameManager.instance.UnlockPlane >> (3 - Swipe.Index()) & 1) == 1)
        {
            if (Swipe.Index() + 1 != GameManager.instance.PlayerPlane)
            {
                GameManager.instance.ChoosePlane();
            }
        }
        else
        {
            if (GameManager.instance.Points >= GameManager.instance.GetPlanePrice(Swipe.Index()))
            {
                GameManager.instance.BuyPlane();
                GameManager.instance.PlaySound(BuySound);

                if (Swipe.Index() == 3)
                {
                    Social.ReportProgress(GPGConfigs.achievement_eagle, 100f, null);
                }
            }
            else
            {
#if UNITY_ANDROID
                AndroidNativeUtils.ShowMsg("Not enough points!");
#endif
                Instantiate(requestBuyPref);
            }
        }
        FillPlanes();
    }

    public void BuyUpgrade(int index)
    {
        if (GameManager.instance.Points >= GameManager.instance.GetUpgradeCost(index))
        {
            GameManager.instance.BuyUpgradeLevel(index);
            FillUpgrades();
            GameManager.instance.PlaySound(BuySound);

            if (Social.localUser.authenticated)
            {
                switch (index)
                {
                    case 0: PlayGamesPlatform.Instance.IncrementAchievement(GPGConfigs.achievement_shield_upgrader, 1, null); break;
                    case 1: PlayGamesPlatform.Instance.IncrementAchievement(GPGConfigs.achievement_magnet_upgrader, 1, null); break;
                    case 2: PlayGamesPlatform.Instance.IncrementAchievement(GPGConfigs.achievement_slow_upgrader, 1, null); break;
                }
            }
        }
        else
        {
#if UNITY_ANDROID
            AndroidNativeUtils.ShowMsg("Not enough points!");
#endif
            Instantiate(requestBuyPref);
        }
    }

    private void ConnectToGoogleService()
    {

        if (!Connected())
        {
            PlayGamesPlatform.Instance.SignOut();
            return;
        }

        //PlayGamesPlatform.DebugLogEnabled = true;
        PlayGamesPlatform.Activate();

        Social.localUser.Authenticate((bool success) =>
        {
            if (success)
            {
                Social.LoadAchievements((IAchievement[] achievements) =>
                {
                    foreach (var ach in achievements)
                    {
                        msg += "\n" + ach.ToString();
                    };
                });
            }
        });
    }

    void OnApplicationPause(bool pauseStatus)
    {
        if (!pauseStatus && !Social.localUser.authenticated)
        {
            ConnectToGoogleService();
        }
    }

    private bool Connected()
    {
        /*
		bool flag = false;
		WebClient client;
		Stream stream;

		try
		{
			client = new WebClient();
			stream = client.OpenRead("http://www.google.com");
			flag = true;
			
			client.Dispose();
			stream.Dispose();
		}
		catch
		{

		}
		
		return flag;
		*/

        if (Application.internetReachability == NetworkReachability.NotReachable) return false;

        return true;
    }

}