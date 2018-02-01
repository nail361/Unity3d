using UnityEngine;
using UnityEngine.UI;

public class RadarMarker : MonoBehaviour {

    [HideInInspector]
    public Transform Target;

    [HideInInspector]
    public float radar_distance;

    private RadarScript radar;
    private RectTransform myRectTransform;
    private Image image;
    private Vector2 newPosition;

	void Start () {
        radar = GetComponentInParent<RadarScript>();
        myRectTransform = GetComponent<RectTransform>();
        image = GetComponent<Image>();
    }
	
	
	void LateUpdate () {

        if ( Target == null)
        {
            Destroy(gameObject);
            return;
        }

        newPosition = radar.TransformPosition(Target.position);
        myRectTransform.anchoredPosition = newPosition;
        
        if (Mathf.Max(Mathf.Abs(newPosition.x),Mathf.Abs(newPosition.y)) > radar_distance)
            image.enabled = false;
        else
            image.enabled = true;
    }
}