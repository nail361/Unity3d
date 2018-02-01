using UnityEngine;
using UnityEngine.UI;
using Facebook.Unity;

public class MainMenu : MonoBehaviour
{
    [SerializeField]
    private GameObject SettingsScreen;

    [SerializeField]
    private Toggle SoundToggle;
    [SerializeField]
    private Toggle MusicToggle;
    [SerializeField]
    private Button SettingBTN;
    [SerializeField]
    private Button PlayBTN;
    [SerializeField]
    private Dropdown LanguageSelector;

    private Animation l_selector_animation;

    private bool playing = false;

    void Start()
    {
        SetLanguage();

        SoundToggle.isOn = GameManager.instance.sound_on;
        MusicToggle.isOn = GameManager.instance.music_on;
        LanguageSelector.value = LanguageManager.GetLanguageIndex();

        l_selector_animation = LanguageSelector.GetComponent<Animation>();
        
        SocialsPlatform.Init();

        if (FB.IsInitialized)
        {
            FB.ActivateApp();
        }
        else
        {
            FB.Init(() => {
                FB.ActivateApp();
            });
        }

        GameManager.LOADED_FROM_MENU = true;
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            PlayerPrefs.DeleteKey("level_complete");
            GameManager.instance.SaveData();
            GameManager.instance.SaveSettings();

            Application.Quit();
        }
    }

    private void SetLanguage()
    {
        PlayBTN.gameObject.GetComponentInChildren<Text>().text = LanguageManager.GetText("PlayBTN");
    }

    public void PlayCLICK()
    {
        Destroy(PlayBTN.gameObject);
        StartCoroutine( GameManager.instance.LoadLevel("ChooseLevel", 1.5f) );
    }

    public void SettingsCLICK()
    {
        if (playing) return;

        playing = true;

        if (!SettingBTN.animator.GetBool("settings_on"))
        {
            SettingBTN.animator.SetBool("settings_on", true);
            SettingsScreen.SetActive(true);
            SoundToggle.animator.SetBool("on", true);
            MusicToggle.animator.SetBool("on", true);
            l_selector_animation["language_dropdown"].speed = 1;
            l_selector_animation["language_dropdown"].time = 0;
            l_selector_animation.Play();
            StartCoroutine(GameManager.instance.waitThenCallback(0.5f, () => { playing = false; }));
        }
        else {
            SettingBTN.animator.SetBool("settings_on", false);
            SoundToggle.animator.SetBool("on", false);
            MusicToggle.animator.SetBool("on", false);
            l_selector_animation["language_dropdown"].speed = -2;
            l_selector_animation["language_dropdown"].time = l_selector_animation["language_dropdown"].length;
            l_selector_animation.Play();

            StartCoroutine(GameManager.instance.waitThenCallback(0.5f, () => { playing = false; SettingsScreen.SetActive(false); }));
        }
    }

    public void ShowLiderboard()
    {
        SocialsPlatform.ShowLeaderboard();
    }

    public void ShowAchoevements()
    {
        SocialsPlatform.ShowAchievements();
    }

    public void OnSoundToggle( )
    {
        GameManager.instance.sound_on = SoundToggle.isOn;
        GameManager.instance.SaveSettings();
    }

    public void OnMusicToggle()
    {
        GameManager.instance.music_on = MusicToggle.isOn;
        GameManager.instance.SaveSettings();
    }

    public void OnLanguageChange()
    {
        LanguageManager.ChangeLanguage( LanguageSelector.value );

        SetLanguage();
    }

    public void InviteFriends()
    {
        if (!FB.IsInitialized) AndroidNativeUtils.ShowMsg(LanguageManager.GetText("NoWeb"));

        FB.ShareLink(
            new System.Uri("https://play.google.com/store/apps/details?id=com.SmileApps.Felix"),
            "My score in Felix",
            GameManager.instance.all_score.ToString() + ", try to play!"
        );
        /*
        FB.Mobile.AppInvite(
            new System.Uri("https://fb/apps/501941880014084")
        );
        */
    }

}