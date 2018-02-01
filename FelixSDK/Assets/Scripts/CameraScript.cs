using UnityEngine;

public class CameraScript : MonoBehaviour {

    private Vector3 old_pos;

    public float maxZoom = 1.0f;
    public float minZoom = 7.0f;

    private Camera cam;

    void Start()
    {
        cam = GetComponent<Camera>();
    }

	void Update () {

#if UNITY_EDITOR || UNITY_STANDALONE
        if (ObjectItem.inUse == null && Input.GetMouseButtonDown(0))
        {
            Vector2 rayPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            RaycastHit2D hit = Physics2D.Raycast(rayPos, Vector2.zero, 0f, HUD.working_layer);

            if (hit)
            {
                hit.transform.GetComponent<ObjectItem>().OnTouch(rayPos);
            }
        }

        if (Input.GetMouseButtonDown(1))
        {
            old_pos = Input.mousePosition;
        }

	    if (Input.GetMouseButton(1))
        {
            transform.Translate((Input.mousePosition - old_pos) / -10);

            old_pos = Input.mousePosition;
        }

        if (Input.mouseScrollDelta.y != 0)
        {
            cam.orthographicSize -= Input.mouseScrollDelta.y/10;

            cam.orthographicSize = Mathf.Clamp(cam.orthographicSize, maxZoom, minZoom);
        }
#else
        if (inUse == null && Input.touchCount == 1 && Input.GetTouch(0).phase == TouchPhase.Ended)
        {
            Vector2 rayPos = Camera.main.ScreenToWorldPoint(Input.GetTouch(0).position);
            RaycastHit2D hit = Physics2D.Raycast(rayPos, Vector2.zero, 0f, HUD.working_layer);

            if (hit)
            {
                hit.transform.GetComponent<ObjectItem>().OnTouch(rayPos);
            }
        }
        else if (Input.touchCount == 2)
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
        else if (Input.touchCount == 1 && !ObjectItem.inUse && Input.GetTouch(0).phase == TouchPhase.Moved)
        {
            transform.Translate(Input.GetTouch(0).deltaPosition/-15);
        }
#endif

    }

}