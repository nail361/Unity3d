using UnityEngine;
using System.Collections;

public class CameraControl : MonoBehaviour {

    private float old_pos;
    private Camera cam;

    private float maxZoom = 2.0f;
    private float minZoom = 5.0f;

    private float friction = 0.2f;
    private float speed = 0.0f;
    private int direction = 0;
    private float delta = 0.0f;

    private float end_angle;

    void Start()
    {
        cam = GetComponent<Camera>();

        transform.position = new Vector3( 0, -5, -10 );
    }

    public void GoToTarget( Transform _target, float angle)
    {
        end_angle = angle;
        speed = 0;
        StartCoroutine("Move");
    }

    IEnumerator Move()
    {
        float delta_angle;
        float start_angle = transform.rotation.eulerAngles.z;
        float new_angle;
        float old_angle = start_angle;

        float scale = 0.0f;
        
        do {
            scale += Time.deltaTime;
            if (scale > 1) scale = 1;

            new_angle = Mathf.LerpAngle( start_angle, end_angle, scale );
            delta_angle = new_angle - old_angle;
            old_angle = new_angle;
            transform.RotateAround(Vector3.zero, Vector3.forward, delta_angle);

            yield return new WaitForEndOfFrame();
        }while (scale < 1);

        yield return null;
    }

    void LateUpdate()
    {

#if UNITY_EDITOR || UNITY_STANDALONE
        
        if (Input.GetMouseButtonDown(1))
        {
            speed = 0;
            old_pos = Input.mousePosition.x;
        }
        else if (Input.GetMouseButton(1))
        {
            delta =  old_pos - Input.mousePosition.x;

            transform.RotateAround(Vector3.zero, Vector3.forward, delta/10);

            old_pos = Input.mousePosition.x;
        }
        else if (Input.GetMouseButtonUp(1))
        {
            speed = Mathf.Abs(delta);
            direction = (int)Mathf.Sign(delta);
        }

        if (Input.mouseScrollDelta.y != 0)
        {
            cam.orthographicSize -= Input.mouseScrollDelta.y / 10;
            cam.orthographicSize = Mathf.Clamp(cam.orthographicSize, maxZoom, minZoom);
        }

#else
        if (Input.touchCount == 2)
        {
            Touch touchZero = Input.GetTouch(0);
            Touch touchOne = Input.GetTouch(1);
            
            Vector2 touchZeroPrevPos = touchZero.position - touchZero.deltaPosition;
            Vector2 touchOnePrevPos = touchOne.position - touchOne.deltaPosition;
            
            float prevTouchDeltaMag = (touchZeroPrevPos - touchOnePrevPos).magnitude;
            float touchDeltaMag = (touchZero.position - touchOne.position).magnitude;
            
            float deltaMagnitudeDiff = prevTouchDeltaMag - touchDeltaMag;

            cam.orthographicSize += (deltaMagnitudeDiff/10);
            cam.orthographicSize = Mathf.Clamp(cam.orthographicSize, maxZoom, minZoom);
        }
        else if (Input.touchCount == 1)
        {
            if (Input.GetTouch(0).phase == TouchPhase.Began)
            {
                speed = 0;
            }
            else if (Input.GetTouch(0).phase == TouchPhase.Moved)
            {
                delta = -Input.GetTouch(0).deltaPosition.x;

                transform.RotateAround(Vector3.zero, Vector3.forward, delta/5);
            }
            else if (Input.GetTouch(0).phase == TouchPhase.Ended)
            {
                speed = Mathf.Abs(delta);
                direction = (int)Mathf.Sign(delta);
            }
        }
#endif

        if (speed != 0)
        {
            transform.RotateAround(Vector3.zero, Vector3.forward, Time.deltaTime * speed * direction);

            speed -= friction;

            if (speed < 0) speed = 0;
        }

    }

}
