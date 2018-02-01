using UnityEngine;
using UnityEngine.UI;

public enum ItemIndex : int { None = -1, Dust, Box, Helicopter, Voltage, Speed, Map, Milk = 99 };

public class Item : MonoBehaviour {

    public ItemIndex index;

    [SerializeField]
    private ItemsHolder itemsHolder;
    [SerializeField]
    private Image image;
    [SerializeField]
    private Text count;
    [SerializeField]
    private GameObject addItemPref;
    [SerializeField]
    private GameObject infoWindowPref;
    [SerializeField]
    private GameObject bagPref;

    private GameObject bag_ghost;

    private bool empty = false;

	void Start () {
        image.sprite = itemsHolder.GetSprite((int)index);
        Inventory.instance.delUpdate += InitItem;
        InitItem();
    }

    public void InitItem()
    {
        count.text = GameManager.instance.items[(uint)index].ToString();
        empty = GameManager.instance.items[(uint)index] == 0;
    }

    public void StartDrag()
    {
        if (empty) return;

        bag_ghost = (GameObject)Instantiate( bagPref, transform.position, Quaternion.identity );
        bag_ghost.transform.SetParent(Inventory.instance.window,false);
        bag_ghost.transform.SetAsLastSibling();
        bag_ghost.transform.GetChild(0).GetComponent<Image>().sprite = image.sprite;
        bag_ghost.GetComponent<Image>().color = new Color(1, 1, 1, 0.5f);
    }

    public void Dragging()
    {
        if (empty) return;

        Vector2 newPos;

#if UNITY_EDITOR

        newPos = Input.mousePosition;
#else
        newPos = Input.touches[0].position;
#endif
        bag_ghost.transform.position = newPos;
    }

    public void StopDrag()
    {
        if (empty) return;

        Collider2D[] colliders = Physics2D.OverlapCircleAll(bag_ghost.transform.position, 1);
        foreach ( Collider2D col in colliders)
        {
            if (col.tag == "Bag")
            {
                col.GetComponent<Bag>().SetItem(index);
                GameManager.instance.items[(uint)index]--;
                Inventory.instance.delUpdate();
                break;
            }
        }

        Destroy(bag_ghost);
    }

    public void BuyItem()
    {
        GameObject addItem = (GameObject)Instantiate(addItemPref, Vector3.zero, Quaternion.identity);
        addItem.transform.SetParent(Inventory.instance.transform.parent, false);
        addItem.transform.SetAsLastSibling();
        addItem.GetComponent<AddItem>().SetItem(index, LanguageManager.GetText(itemsHolder.GetTitle((int)index)), image.sprite);
    }

    public void OpenInfoWindow()
    {
        GameObject infoWindow = (GameObject)Instantiate(infoWindowPref, Vector3.zero, Quaternion.identity);
        infoWindow.transform.SetParent(Inventory.instance.transform.parent, false);
        infoWindow.transform.SetAsLastSibling();
        infoWindow.GetComponent<InfoItemWindow>().Init(
            LanguageManager.GetText(itemsHolder.GetTitle((int)index)),
            image.sprite);
    }

}