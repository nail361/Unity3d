using UnityEngine;
using UnityEngine.UI;

public class LevelWindow : BaseWindow {

    [SerializeField]
    private Text score_text;
    [SerializeField]
    private Text play_text;
    [SerializeField]
    private Image[] stars_img;

    private LoadingImg loading_img;

    void Start () {
        DeactivateCamera();
        transform.SetParent(GameObject.Find("HUD").transform, false);
        transform.SetSiblingIndex(transform.parent.childCount-3);

        score_text.text = LanguageManager.GetText("Score") + ":  " + GameManager.instance.GetCurScore().ToString();
        play_text.text = LanguageManager.GetText("PlayBTN");

        for (int i = 1; i <= stars_img.Length; i++)
        {
            stars_img[i-1].color = i <= GameManager.instance.GetCurStars() ? Color.white : Color.grey;
        }
    }

    public void PlayClick()
    {
        if (!isActive) return;

        if (GameManager.instance.UseMilk())
        {
            isActive = false;
            SoundManager.instance.ChangeMusicVolume(0.5f);
            SoundManager.instance.PlaySound("play");
            Invoke("StartLevel", 1.0f);
        }
    }

    private void StartLevel()
    {
        StartCoroutine(GameManager.instance.LoadLevel("Level", 1.5f));
    }
	
	public override void CloseWindow()
    {
        if (!isActive) return;
        isActive = false;
        GetComponent<Animation>().Play("level_window_close");
        Invoke("ActivateCamera", GetComponent<Animation>()["level_window_close"].length);
    }

    private void ActivateCamera()
    {
        base.ActivateCamera();
        DestroySelf();
    }
}