using UnityEngine;
using UnityEngine.UI;

public class GamePausePanel : FinalPanel
{
    [SerializeField]
    private Text GamePauseText;
    [SerializeField]
    private Text continue_text;
    [SerializeField]
    private Text surrender_text;

    public override void Start () {
        base.Start();

        GamePauseText.text = LanguageManager.GetText("GamePause");
        continue_text.text = LanguageManager.GetText("Continue");
        surrender_text.text = LanguageManager.GetText("Surrender");

        Time.timeScale = 0;

        SoundManager.instance.PauseMusic();
        SoundManager.instance.PlaySound("pause");
    }

    public void Surrender()
    {
        Time.timeScale = 1;
        if (Timers.HasTimer("stop_watch")) Timers.RemoveTimer("stop_watch");
        base.MenuCLICK();
    }

    public void Continue()
    {
        Time.timeScale = 1;
        SoundManager.instance.ResumeMusic();
        DestroySelf();
    }
	
}
