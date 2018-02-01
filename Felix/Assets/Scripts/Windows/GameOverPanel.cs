using UnityEngine;
using UnityEngine.UI;

public class GameOverPanel : FinalPanel {

    private const int continue_cost = 15;

    [SerializeField]
    private Text GameOverText;
    [SerializeField]
    private Text continue_text;
    [SerializeField]
    private Text surrender_text;
    [SerializeField]
    private FishPanel fish_panel;

    public override void Start () {

        base.Start();

        SoundManager.instance.ChangeMusicVolume(0.5f);
        SoundManager.instance.PlaySound("game_over");

        if (GameManager.instance.cur_mode == 1) GameObject.Find("Timer").transform.SetAsLastSibling();

        continue_text.text = LanguageManager.GetText("Continue") + " " + continue_cost.ToString();
        surrender_text.text = LanguageManager.GetText("Surrender");

        GameOverText.text = LanguageManager.GetText("GameOver");
    }

    private uint star_count;
    private uint score;
    private bool star_mode = false;
    private GameHUD game_hud;

    public void StarMode(uint star_count, uint score, GameHUD game_hud)
    {
        this.star_count = star_count;
        this.score = score;
        star_mode = true;
        this.game_hud = game_hud;
    }

    public override void RetryCLICK()
    {
        if (!isActive) return;

        if (GameManager.instance.UseMilk())
        {
            isActive = false;
            Invoke("Retry", 1.0f );
        }
        else
        {
            isActive = true;
        }
    }

    private void Retry()
    {
        Close();
        isActive = true;
        base.RetryCLICK();
    }

    public void SurrenderCLICK()
    {
        if (star_mode)
        {
            GameManager.instance.LevelComplete(star_count, score);
            game_hud.GameWin();
            Close();
        }
        else
        {
            GetComponent<Animation>().PlayQueued("btns_appear");
        }
    }

    public void ContinueClick()
    {
        if (!isActive) return;

        if (fish_panel.SubFish(continue_cost))
        {
            isActive = false;
            Invoke("Close", 1.0f);
            Invoke("Continue", _animation["game_over"].length + 1.0f);
        }
    }

    private void Continue()
    {
        SoundManager.instance.ChangeMusicVolume(1.0f);
        GameObject.FindGameObjectWithTag("Player").GetComponent<Player>().Continue();
        DestroySelf();
    }

    private void Close()
    {
        _animation["game_over"].speed = -1;
        _animation["game_over"].time = _animation["game_over"].length;
        _animation.Play();
    }

}