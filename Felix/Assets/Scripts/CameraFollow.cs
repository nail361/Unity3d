using UnityEngine;

public class CameraFollow : MonoBehaviour {

    private Transform target;

    private float dampTime = 0.4f;
    private Vector3 velocity = Vector3.zero;

    private float normal_zoom;
    private float cur_zoom = 0;
    private float need_zoom;
    private Camera cam;

	void Start () {
        target = Player.instance.transform;

        cam = GetComponent<Camera>();

        need_zoom = normal_zoom = cam.orthographicSize;
	}
	
	void LateUpdate () {

        Vector3 point = cam.WorldToViewportPoint(target.position);
        Vector3 delta = target.position - cam.ViewportToWorldPoint(new Vector3(0.5f, 0.5f, point.z)); //(new Vector3(0.5, 0.5, point.z));
        Vector3 destination = transform.position + delta;
        transform.position = Vector3.SmoothDamp(transform.position, destination, ref velocity, dampTime);

        if (cam.orthographicSize != need_zoom)
            cam.orthographicSize = Mathf.SmoothDamp(cam.orthographicSize, need_zoom, ref cur_zoom, dampTime);

        //transform.position = new Vector3(target.position.x,target.position.y,transform.position.z);
    }

    public void ChangeZoom( float new_zoom, float delay = 0 )
    {
       need_zoom = new_zoom;

        if (delay != 0)
            Invoke("ZoomNormal", delay);
    }

    public void ZoomNormal()
    {
        need_zoom = normal_zoom;
    }
    
}