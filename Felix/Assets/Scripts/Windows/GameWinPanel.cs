using UnityEngine;
using UnityEngine.UI;

public class GameWinPanel : FinalPanel {

    [SerializeField]
    private Text GameWinText;
    [SerializeField]
    private Image[] stars_img;
    [SerializeField]
    private Text[] stars_time;
    [SerializeField]
    private Text score_text;
    [SerializeField]
    private Image fish_img;

    public override void Start () {

        base.Start();

        SoundManager.instance.ChangeMusicVolume(0.5f);
        SoundManager.instance.PlaySound("win");

        GameManager.instance.ReturnMilk();

        for (int i = 1; i <= stars_img.Length; i++)
        {
            stars_img[i - 1].color = i <= GameManager.instance.star_count ? Color.white : Color.grey;
            if (GameManager.instance.cur_mode != 1) Destroy(stars_time[i-1].gameObject);
            else
            {
                stars_time[i-1].text = GameManager.instance.level_params[4-i] + " - " + GameManager.instance.level_params[3-i] + " " + LanguageManager.GetText("SecShort");
            }
        }

        score_text.text = LanguageManager.GetText("Score") + ":  " + GameManager.instance.score.ToString();

        GameWinText.text = LanguageManager.GetText("GameWin");
        if (GameManager.instance.new_record)
        {
            GetComponent<Animation>().PlayQueued("best_score");
        }

        if (GameManager.instance.add_level_fish > 0)
        {
            fish_img.GetComponentInChildren<Text>().text = "+" + GameManager.instance.add_level_fish.ToString();
            Invoke("ActivateFishImg", 1.0f);
        }
    }

    public override void RetryCLICK()
    {
        if (GameManager.instance.UseMilk())
        {
            base.RetryCLICK();
        }
    }

    private void ActivateFishImg()
    {
        fish_img.gameObject.SetActive(true);
    }

}