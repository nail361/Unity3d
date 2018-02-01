using UnityEngine;
using System.Collections;

public class MenuCamera : MonoBehaviour {

    [SerializeField]
    private Transform upper_dummy;
    [SerializeField]
    private Transform bottom_dummy;

    private float upper_limit;
    private float bottom_limit;

    private float scroll_speed = 20;
    private float inert_speed = 0;
    private float friction = 1;

    private float find_speed = 1.5f;

    private int _active = 0;

    private float end_y = 0;

    void Start()
    {
        upper_limit = upper_dummy.position.y;
        bottom_limit = bottom_dummy.position.y;

        upper_dummy = null;
        bottom_dummy = null;
    }

    public void FindPlayer(float player_y, float find_speed = 1.5f, float delay = 0.0f)
    {
        end_y = player_y;
        this.find_speed = find_speed;

        if (delay == 0.0f) RestartCoroutine();
        else Invoke("RestartCoroutine", delay);
    }

    private void RestartCoroutine()
    {
        StopCoroutine("FlyToPlayer");
        StartCoroutine("FlyToPlayer");
    }

    private IEnumerator FlyToPlayer()
    {
        float start_y = transform.position.y;
        float scale = 0.0f;

        while (scale < 1.0f)
        {
            transform.position = new Vector3(
                transform.position.x,
                Mathf.Lerp(start_y, end_y, scale),
                transform.position.z);

            scale += Time.deltaTime * find_speed; 
            yield return null;
        }
    }

    public void ActiveSwitch()
    {
        _active--;
    }

    public void DeActiveSwitch()
    {
        _active++;
    }

    void Update () {

        if (_active > 0) return;

#if UNITY_EDITOR
        if (Input.mouseScrollDelta.y != 0)
        {
            inert_speed = scroll_speed * Input.mouseScrollDelta.y;
        }
#else

        if (Input.touchCount > 0)
        {
            if (Input.GetTouch(0).phase == TouchPhase.Moved)
            {
                inert_speed = scroll_speed * (-Input.GetTouch(0).deltaPosition.y / 10);
            }
        }
#endif

        if (inert_speed != 0)
        {
            transform.Translate(Vector3.up * inert_speed * Time.deltaTime);

            if (transform.position.y < bottom_limit)
            {
                transform.position = new Vector3(transform.position.x, bottom_limit, transform.position.z);
                inert_speed = 0;
                return;
            }
            else if (transform.position.y > upper_limit)
            {
                transform.position = new Vector3(transform.position.x, upper_limit, transform.position.z);
                inert_speed = 0;
                return;
            }

            if (Mathf.Abs(inert_speed) - (friction * Time.deltaTime) < friction) {
                inert_speed = 0;
                return;
            }

            inert_speed += inert_speed > 0 ? -friction : friction;
        }
    }
}