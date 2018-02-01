using UnityEngine;
using UnityEngine.UI;

public class WordPanel : MonoBehaviour {

    [SerializeField]
    private Text timer_title;
    [SerializeField]
    private Text word_title;
    [SerializeField]
    private Image game_type_img;
    [SerializeField]
    private FinalPanel final_panel;
    [SerializeField]
    private Button start_btn;
    [SerializeField]
    private Button stop_btn;

    private bool isActive = false;

    private float timer = 0.0f;

	public void Init (Sprite _sprite, string word) {

        word_title.text = word;
        timer_title.text = "60";
        timer_title.color = Color.white;
        game_type_img.sprite = _sprite;
        gameObject.SetActive(true);

        start_btn.gameObject.SetActive(true);
        stop_btn.gameObject.SetActive(false);

        final_panel.gameObject.SetActive(false);  
	}

    public void StartClick()
    {
        timer = 60.0f;
        isActive = true;

        SoundManager.instance.PlaySound("start_play");

        start_btn.gameObject.SetActive(false);
        stop_btn.gameObject.SetActive(true);
    }

    public void StopClick()
    {
        isActive = false;
        ShowFinalPanel();
    }
	
	void Update () {

        if (!isActive) return;

        timer -= Time.deltaTime;

        float time = Mathf.Ceil(timer);

        timer_title.text = time.ToString();

        if ( time == 7.0f )
        {
            SoundManager.instance.PlaySound("timer");
        }

        if (timer <=0)
        {
            isActive = false;
            timer_title.text = "время вышло";
            timer_title.color = Color.red;
            SoundManager.instance.PlaySound("timer_end");
            Invoke("ShowFinalPanel", 3.0f);
        }
    }

    private void ShowFinalPanel()
    {
        SoundManager.instance.StopPlaying();
        gameObject.SetActive(false);
        final_panel.gameObject.SetActive(true);
    }

    void OnDisable()
    {
        isActive = false;
    }
}
