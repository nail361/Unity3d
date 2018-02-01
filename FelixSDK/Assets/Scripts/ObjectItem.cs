using UnityEngine;
using UnityEngine.UI;

public class ObjectItem : MonoBehaviour {

    [SerializeField]
    private GameObject hud;
    [SerializeField]
    private SpriteRenderer image;
    [SerializeField]
    private Button special_btn;

    public Vector2 pos;

    private float trehsoldPos = 0.1f;
    private Vector2 old_touchPos;
    private Vector2 old_pos;

    public int index;
    public int special;

    private string prefix;

    public static ObjectItem inUse = null;

    public void Init( Sprite sprite, uint type, int index, int special, Vector2 pos, Transform holder ) {

        prefix = type == 0 ? "tile_" : "object_";

        name = prefix + pos.x + "," + pos.y;

        transform.parent = holder;
        transform.position = new Vector3(pos.x,-pos.y,0);
        old_pos = pos;

        image.sprite = sprite;

        if (type == 0) {
            special_btn.interactable = false;
            gameObject.layer = 8;
        }
        else {
            image.sortingOrder = 1;
            gameObject.layer = 9;
        }
        
        this.index = index;
        this.pos = pos;

        hud.SetActive(false);

        this.special = special;
        image.transform.Rotate(new Vector3(0, 0, special*90));
    }

    public void OnTouch(Vector2 position)
    {
        if (inUse != null) inUse.hud.SetActive(false);
        inUse = this;
        hud.SetActive(true);
        old_touchPos = position;
    }

    public void OnDrag(Vector2 position)
    {
        transform.position = new Vector3(transform.position.x+position.x, transform.position.y+position.y, -5);
    }

    public void OnDrop()
    {
        pos = new Vector2(Mathf.Round(transform.position.x), -Mathf.Round(transform.position.y));

        if (GameObject.Find(prefix + pos.x + "," + pos.y) || pos.x < 0 || pos.y < 0 || pos.x >= HUD.width || pos.y >= HUD.height) pos = old_pos;

        transform.position = new Vector3(pos.x, -pos.y, 0);
        old_pos = pos;
        name = prefix + pos.x + "," + pos.y;
    }

    void Update()
    {
        if (inUse != this) return;
#if UNITY_EDITOR || UNITY_STANDALONE
        if (Input.GetMouseButtonDown(0))
        {
            old_touchPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        }
        else if (Input.GetMouseButton(0))
        {
            Vector2 rayPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);

            if ((rayPos - old_touchPos).magnitude < trehsoldPos) return;

            OnDrag(rayPos-old_touchPos);
            old_touchPos = rayPos;
        }
        else if (Input.GetMouseButtonUp(0))
        {
            OnDrop();
        }

#else
        if (Input.touchCount == 1)
        {
            if (Input.GetTouch(0).phase == TouchPhase.Began)
            {
                old_touchPos = Camera.main.ScreenToWorldPoint(Input.GetTouch(0).position);
            }
            else if (Input.GetTouch(0).phase == TouchPhase.Moved)
            {
                if (Input.GetTouch(0).deltaPosition.magnitude < trehsoldPos) return;
                OnDrag(Input.GetTouch(0).deltaPosition);
            }
            else if (Input.GetTouch(0).phase == TouchPhase.Ended)
            {
                OnDrop();
            }
        }
#endif
    }

    public string GetIndex()
    {
        string str = index.ToString();

        if (special > 0) str = "\"" + str + "_" + special + "\"";

        return str;
    }

    public void SpecialClick()
    {
        special++;

        if (special > 3) special = 0;
        image.transform.Rotate(new Vector3(0, 0, 90));

        HUD.Console.text = "special = " + special.ToString() + " ";
    }

    public void ApplyClick()
    {
        inUse = null;
        hud.SetActive(false);
    }

    public void DeleteClick()
    {
        inUse = null;
        Destroy(gameObject);
    }
}