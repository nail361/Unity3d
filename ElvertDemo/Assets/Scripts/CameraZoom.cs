using UnityEngine;

public class CameraZoom : MonoBehaviour {

    private float max_zoom = 70.0f;
    private float min_zoom = 20.0f;
    private Camera cam;

	void Start () {
        cam = Camera.main;
	}

	void Update () {

        if (SystemInfo.deviceType == DeviceType.Desktop)
        {
            if (Input.mouseScrollDelta.magnitude != 0)
            {
                cam.fieldOfView -= Input.mouseScrollDelta.y;
            }
        }
        else if (SystemInfo.deviceType == DeviceType.Handheld)
        {

        }

        if (cam.fieldOfView > max_zoom) cam.fieldOfView = max_zoom;
        else if (cam.fieldOfView < min_zoom) cam.fieldOfView = min_zoom;

    }
}
