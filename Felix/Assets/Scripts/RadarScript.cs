using UnityEngine;
using UnityEngine.UI;

public class RadarScript : MonoBehaviour {

    private Transform playerTransform;
    private Rect radarRect;

    public float zoom = 10f;

    [SerializeField]
    private Sprite markerSprite;

    void Start()
    {
        if (GameManager.instance.cur_mode < 2) Invoke("Init", 1.0f);
        else Destroy(gameObject);
    }

    private void Init()
    {
        playerTransform = Player.instance.transform;

        radarRect = new Rect(-50, 50, 50, -50);

        foreach (Enemy enemy in Enemy.ENEMIES)
        {
            CreateMarker("enemyMarker", enemy.transform, Color.red, enemy.RadarDistance);
        }

        GameObject[] sosiskas = GameObject.FindGameObjectsWithTag("Star");

        foreach (GameObject sosiska in sosiskas)
            CreateMarker("sosiskaMarker", sosiska.transform, Color.green);

        CreateMarker("exitMarker", GameObject.FindGameObjectWithTag("Exit").transform, Color.yellow, 9999);
    }

    private void CreateMarker(string markerName, Transform markerTransform, Color markerColor, float radar_distance = 45)
    {
        GameObject marker = new GameObject(markerName, typeof(RectTransform), typeof(Image), typeof(RadarMarker));
        marker.GetComponent<RectTransform>().SetParent(transform,false);
        marker.GetComponent<RectTransform>().sizeDelta = new Vector2(10,10);
        marker.GetComponent<RadarMarker>().Target = markerTransform;
        marker.GetComponent<RadarMarker>().radar_distance = radar_distance;
        marker.GetComponent<Image>().raycastTarget = false;
        marker.GetComponent<Image>().sprite = markerSprite;
        marker.GetComponent<Image>().color = markerColor;
        marker.GetComponent<RectTransform>().SetAsFirstSibling();
    }
	
	public Vector2 TransformPosition(Vector2 position)
    {
        Vector2 markerPos = new Vector2(position.x - playerTransform.position.x, position.y - playerTransform.position.y) * zoom;
        markerPos.x = markerPos.x > radarRect.width ? radarRect.width : markerPos.x < radarRect.x ? radarRect.x : markerPos.x;
        markerPos.y = markerPos.y < radarRect.height ? radarRect.height : markerPos.y > radarRect.y ? radarRect.y : markerPos.y;
        return markerPos;
    }

}