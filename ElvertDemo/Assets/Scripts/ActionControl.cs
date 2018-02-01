using UnityEngine;

public class ActionControl : MonoBehaviour {

    private Animation _animation;

    private Ray ray;
    private RaycastHit hit;

    public LayerMask layerMask;

    void Start () {
        _animation = GetComponent<Animation>();
        _animation[_animation.clip.name].speed *= -1;
    }

	void Update () {

        if (SystemInfo.deviceType == DeviceType.Desktop)
        {
            if (Input.GetMouseButtonDown(0))
            {
                ray = Camera.main.ScreenPointToRay(Input.mousePosition);
                Physics.Raycast(ray, out hit, 50, layerMask.value);
                if (hit.collider)
                {
                    if (hit.transform.gameObject == gameObject) PlayAnimation();
                }
            }
        }
        else if (SystemInfo.deviceType == DeviceType.Handheld)
        {
            if (Input.touchCount > 0 && Input.GetTouch(0).phase == TouchPhase.Began)
            {
                ray = Camera.main.ScreenPointToRay(Input.GetTouch(0).position);
                Physics.Raycast(ray, out hit, 50, layerMask.value);
                if (hit.collider)
                {
                    if (hit.transform.gameObject == gameObject) PlayAnimation();
                }
            }
        }

    }

    private void PlayAnimation()
    {
        _animation[_animation.clip.name].speed *= -1;

        if (!_animation.isPlaying)
        if (_animation[_animation.clip.name].speed < 0)
        {
            _animation[_animation.clip.name].time = _animation[_animation.clip.name].length;
        }

        _animation.Play();
    }

}
