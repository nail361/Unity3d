using UnityEngine;
using System;

public class MainMenuHUD : MonoBehaviour {

    [SerializeField]
    private GameObject inventoryWindowPref;
    [SerializeField]
    private GameObject dailyReward;
    [SerializeField]
    private GameObject ratePanel;

    [SerializeField]
    private AudioClip menu_music;

    void Start()
    {
        int now = DateTime.Today.Date.DayOfYear;

        if (PlayerPrefs.HasKey("last_reward"))
        {
            int past_days = now - PlayerPrefs.GetInt("last_reward");

            if (past_days == 1)
            {
                PlayerPrefs.SetInt("last_reward", now);
                Instantiate(dailyReward);
            }
            else if (past_days > 1)
                PlayerPrefs.SetInt("last_reward", now);
        }
        else
        {
            PlayerPrefs.SetInt("last_reward", now);
        }

        /*
        double now = (DateTime.Now - new DateTime(1970, 1, 1)).TotalHours;
        Debug.Log("data = " + now);
        
        if (PlayerPrefs.HasKey("last_reward"))
        {

            long past_hours = (long)(Convert.ToDouble(PlayerPrefs.GetString("last_reward")) - now);

            if (past_hours >= 1440 && past_hours < 2880)
            {
                long elapsedTime = AndroidNativeUtils.WorkingTime();
                elapsedTime -= Convert.ToUInt32(PlayerPrefs.GetString("elapsedTime"));
                elapsedTime /= 3600000;

                if (elapsedTime > 0)
                {
                    if (Mathf.Abs(past_hours - elapsedTime) > float.Epsilon) return;
                    //else if ()
                }
                else {
                    PlayerPrefs.SetString("last_reward", now.ToString());
                    PlayerPrefs.SetString("elapsedTime", AndroidNativeUtils.WorkingTime().ToString());
                    Instantiate(dailyReward, Vector3.zero, Quaternion.identity);
                }
            }
        }
        else
        {
            PlayerPrefs.SetString("last_reward", now.ToString());
            PlayerPrefs.SetString("elapsedTime", AndroidNativeUtils.WorkingTime().ToString());
        }
        */

        if (RatePanel.launchCount >= 3)
            Instantiate(ratePanel);

        SoundManager.instance.PlayMusic(menu_music);
    }

    void Update()
    {

        if (Input.GetKeyDown(KeyCode.Escape))
        {
            BackCLICK();
        }
    }

    public void BackCLICK()
    {
        SoundManager.instance.StopMusic();
        StartCoroutine( GameManager.instance.LoadLevel("MainMenu", 1.5f) );
    }

    public void InventoryCLICK()
    {
        Instantiate(inventoryWindowPref);
    }

}