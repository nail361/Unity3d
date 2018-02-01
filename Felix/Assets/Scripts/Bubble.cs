using UnityEngine;
using UnityEngine.UI;

public class Bubble : MonoBehaviour {

    [SerializeField]
    private RectTransform canvasRectT;
    [SerializeField]
    private Text msg;
    private Animation bubble_animation;

    private RectTransform bubble_transform;

    private const float showTime = 2.0f;
    private float curTime = 0.0f;

    private float offsetX = 25.0f;
    private float offsetY = 35.0f;

    void Awake () {
        bubble_transform = GetComponent<RectTransform>();
        bubble_animation = GetComponent<Animation>();
    }

    public void ShowMessage(string text)
    {
        msg.text = text;
        curTime = showTime;
        ShowBubble();
    }

	void Update () {
        if (curTime > 0)
        {
            curTime -= Time.deltaTime;

            Vector2 playerScreen = Camera.main.WorldToScreenPoint(Player.instance.transform.position);
            bubble_transform.anchoredPosition = new Vector2(
                (playerScreen.x + offsetX) / canvasRectT.localScale.x,
                (playerScreen.y + offsetY) / canvasRectT.localScale.y
                );

            if (curTime <= 0)
            {
                curTime = 0;
                HideBubble();
            }
        }
	}

    private void ShowBubble()
    {
        if (bubble_animation["bubble"].time == bubble_animation["bubble"].length) return;
        bubble_animation["bubble"].speed = 1;
        bubble_animation.Play();
    }

    private void HideBubble()
    {
        bubble_animation["bubble"].time = bubble_animation["bubble"].length;
        bubble_animation["bubble"].speed = -1;
        bubble_animation.Play();
    }
}
