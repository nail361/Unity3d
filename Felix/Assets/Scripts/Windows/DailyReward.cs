using UnityEngine;
using UnityEngine.UI;

public class DailyReward : BaseWindow {

    private static uint[] reward = {1,2,3,5,10};

    [SerializeField]
    private Text title;
    [SerializeField]
    private Text take;

    [SerializeField]
    private Image[] daysIcons;

    private uint days = 0;

    void Start()
    {
        title.text = LanguageManager.GetText("DailyReward");
        take.text = LanguageManager.GetText("Take");

        _animation = GetComponent<Animation>();
        anim_name = "daily_reward";

        transform.SetParent(GameObject.Find("HUD").transform, false);
        transform.SetAsLastSibling();

        DeactivateCamera();

        Init();
    }

    private void Init()
    {
        if (PlayerPrefs.HasKey("days"))
        {
            days = (uint)PlayerPrefs.GetInt("days");
        }

        days++;

        if (days >= 6) days = 1;

        SetReward(days);

        PlayerPrefs.SetInt("days", (int)days);
    }

    private void SetReward(uint days)
    {
        for( uint i = 1; i <= 5; i++)
        {
            daysIcons[i-1].color = i > days ? Color.grey : Color.white;
            daysIcons[i - 1].GetComponentInChildren<Text>().text = i.ToString() + " " + LanguageManager.GetText("Days");
            if (i == days) daysIcons[i - 1].GetComponentInChildren<Text>().color = Color.white;
        }
    }

    public void TakeReward()
    {
        GameObject.Find("FishPanel").GetComponent<FishPanel>().AddFish(reward[days-1]);
        CloseWindow();
    }

}
