using UnityEngine;
using UnityEngine.UI;

public class RatePanel : BaseWindow {

    //Window1
    [SerializeField]
    private GameObject window_1;
    [SerializeField]
    private Text question_title;
    [SerializeField]
    private Text yes_label;
    [SerializeField]
    private Text no_label;
    //----------

    //Window2
    [SerializeField]
    private GameObject window_2;
    [SerializeField]
    private Text rate_title;
    [SerializeField]
    private Text rate_label;
    [SerializeField]
    private Text remind_label;
    [SerializeField]
    private Text exit_label;
    //---------------

    public static int launchCount = 0;

	public static void LaunchApp()
    {
        launchCount = PlayerPrefs.GetInt("rate");
        launchCount++;
        PlayerPrefs.SetInt("rate", launchCount);
    }

    private static void Reset()
    {
        launchCount = 0;
        PlayerPrefs.SetInt("rate", launchCount);
    }

    void Start()
    {
        transform.SetParent(GameObject.FindGameObjectWithTag("HUD").transform, false);
        transform.SetAsLastSibling();

        window_1.SetActive(true);
        question_title.text = LanguageManager.GetText("RateQuestion");
        yes_label.text = LanguageManager.GetText("Accept");
        no_label.text = LanguageManager.GetText("Deny");

        rate_title.text = LanguageManager.GetText("RateTitle");
        rate_label.text = LanguageManager.GetText("RateUs");
        remind_label.text = LanguageManager.GetText("Remind");
        exit_label.text = LanguageManager.GetText("Deny");
        window_2.SetActive(false);

        DeactivateCamera();
    }

    public void YesClick()
    {
        window_1.SetActive(false);
        window_2.SetActive(true);
    }

    public void RateClick()
    {
        if (InternetChecker.isConnected && InternetChecker.ping.isDone)
        {
            Application.OpenURL("market://details?id=com.SmileApps.Felix");
            PlayerPrefs.DeleteKey("rate");
            ActivateCamera();
            DestroySelf();
        }
        else
        {
            AndroidNativeUtils.ShowMsg(LanguageManager.GetText("NoWeb"));
        }
    }

    public void ResetClick()
    {
        Reset();
        ActivateCamera();
        DestroySelf();
    }

    public void ExitClick()
    {
        launchCount = 0;
        PlayerPrefs.DeleteKey("rate");
        ActivateCamera();
        DestroySelf();
    }

}
