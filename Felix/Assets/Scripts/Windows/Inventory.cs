using UnityEngine;
using UnityEngine.UI;

public delegate void DelUpdater();

public class Inventory : BaseWindow {

    [HideInInspector]
    public static Inventory instance;

    public Transform window;

    [HideInInspector]
    public DelUpdater delUpdate;

    [SerializeField]
    private Text inventory_title;
    [SerializeField]
    private Text bag_title;

    void Start()
    {
        inventory_title.text = LanguageManager.GetText("Inventory");
        bag_title.text = LanguageManager.GetText("Bag");
        _animation = GetComponent<Animation>();
        anim_name = "inventory";

        transform.SetParent(GameObject.Find("HUD").transform, false);
        transform.SetAsLastSibling();

        instance = this;

        DeactivateCamera();
    }

}