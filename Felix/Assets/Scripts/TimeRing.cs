using UnityEngine;
using UnityEngine.UI;

public class TimeRing : MonoBehaviour {

    [SerializeField]
    private Text timer_field;
    [SerializeField]
    private Image time_ring_img;

    private float max_time = 0;
    private float cur_time = 0;
    private float prev_ceil = 0;

    void Start()
    {
        gameObject.GetComponent<RectTransform>().anchoredPosition = Vector2.zero;
    }

    public void Init (float time_sec) {
        cur_time = prev_ceil = max_time = time_sec;
        timer_field.text = prev_ceil.ToString();
    }

    void Update ()
    {
        cur_time -= Time.deltaTime;

        if (cur_time < 0)
        {
            Destroy(gameObject);
            return;
        }

        if (Mathf.CeilToInt(cur_time) != prev_ceil)
        {
            prev_ceil = Mathf.CeilToInt(cur_time);
            timer_field.text = prev_ceil.ToString();
        }

        //time_ring_img.fillAmount = cur_time / max_time;
    }
}
