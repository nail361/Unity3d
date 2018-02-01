using UnityEngine;
using UnityEngine.UI;
using System;

public class GameHUD : MonoBehaviour {
    [SerializeField]
    private GameObject stars_panel;
    [SerializeField]
    private Image[] stars_img;
    [SerializeField]
    private Image timerIMG;
    [SerializeField]
    private Text timerText;

    [SerializeField]
    private GameObject GameOverPanel;
    [SerializeField]
    private GameObject GameWinPanel;
    [SerializeField]
    private GameObject GamePausePanel;
    [SerializeField]
    private Button slowBtn;
    [SerializeField]
    private AudioClip[] music_modes;

    private Animation timer_animation;

    private bool isSlow = false;
    public bool IsSlow
    {
        get { return isSlow; }
    }
    private bool timeIsOut = false;

    void Start () {
        switch (GameManager.instance.cur_mode)
        {
            case 0:
                Destroy(timerIMG.gameObject);
                foreach (Image star in stars_img)
                {
                    star.sprite = GameManager.instance.star_sprites[0];
                    star.GetComponent<CanvasGroup>().alpha = 0.5f;
                }
                break;
            case 1:
                foreach (Image star in stars_img) star.sprite = GameManager.instance.star_sprites[1];
                Timers.AddTimer("stop_watch", GameManager.instance.level_params[0]*1000, 0, 100, Tick, TimerComplete);
                timer_animation = timerText.GetComponentInParent<Animation>();
                break;
            case 2:
                Destroy(timerIMG.gameObject);
                foreach (Image star in stars_img) star.sprite = GameManager.instance.star_sprites[2];
                break;
        }

        GameManager.LOADED_FROM_MENU = false;
        SoundManager.instance.PlayMusic(music_modes[GameManager.instance.cur_mode]);
	}

    private void Tick()
    {
        int time = Timers.GetTime("stop_watch");
        float sec = Mathf.Floor(time / 1000);
        string millisec = (time - sec * 1000).ToString();
        millisec = millisec.Substring(0, Mathf.Min(millisec.Length, 2));

        if (sec < 5)
        {
            if (!timeIsOut)
            {
                timeIsOut = true;
                Player.instance.bubble.ShowMessage(LanguageManager.GetText("SayHurry"));
                timerText.color = Color.red;
            }
        }
        else
        {
            timeIsOut = false;
            timerText.color = Color.white;
        }

        timerText.text = sec < 10 ? "0" + sec.ToString() : sec.ToString();
        timerText.text += ":";
        timerText.text += millisec.Length < 2 ? "0" + millisec : millisec;

        CheckStars(sec);
    }

    private void CheckStars(float sec)
    {
        for (int i = 1; i <= 2; i++) {
            if (sec < GameManager.instance.level_params[i])
                stars_img[3-i].GetComponent<CanvasGroup>().alpha = 0.5f;
        }
    }

    private void TimerComplete()
    {
        stars_img[0].GetComponent<CanvasGroup>().alpha = 0.5f;

        timerText.text = "00:00";
        timer_animation["timer"].speed = 1;
        timer_animation.Play();

        GameObject.FindWithTag("Player").GetComponent<Player>().EndGame("GameOver");
    }

    public void ContinueTimer()
    {
        timer_animation.Play();
        timer_animation["timer"].speed = 0;
        timer_animation["timer"].time = 0;

        Timers.AddTimer("stop_watch", Mathf.Min(Int32.Parse(timerText.text.Split(':')[0])*1000 + GameManager.instance.level_params[0]*1000, GameManager.instance.level_params[0]*1000), 0, 100, Tick, TimerComplete);

        foreach (Image star in stars_img) star.GetComponent<CanvasGroup>().alpha = 1.0f;
    }

    public void HaveStar(uint index)
    {
        stars_img[index].GetComponent<Animation>().Play();
    }

    public void AddStar(uint index)
    {
        stars_img[index].GetComponent<Animation>().Stop();
        stars_img[index].GetComponent<CanvasGroup>().alpha = 1.0f;
    }

    public void RemoveStar(uint index)
    {
        stars_img[index].GetComponent<CanvasGroup>().alpha = 0.5f;
    }

    public void ToggleSlow()
    {
        if (!Player.instance.SwitchSlow()) return;

        ColorBlock colors = slowBtn.colors;

        if (isSlow)
        {
            colors.normalColor = new Color(1.0f, 1.0f, 1.0f, 0.5f);
        }
        else
        {
            colors.normalColor = new Color(1.0f, 1.0f, 1.0f, 1.0f);
        }

        slowBtn.colors = colors;
        isSlow = !isSlow;
    }

    public void GameWin()
    {
        Instantiate(GameWinPanel, Vector3.zero, Quaternion.identity);
    }

    public void GameOver()
    {
        Instantiate(GameOverPanel, Vector3.zero, Quaternion.identity);
    }

    public void GameOver(uint star_count, uint score)
    {
        (Instantiate(GameOverPanel, Vector3.zero, Quaternion.identity) as GameObject).GetComponent<GameOverPanel>().StarMode(star_count, score, this);
    }

    public void Pause()
    {
        Time.timeScale = 0;
        Instantiate(GamePausePanel, Vector3.zero, Quaternion.identity);
    }
    
    void OnApplicationPause(bool pauseStatus)
    {
        GameManager.instance.pause = pauseStatus;
    }

}