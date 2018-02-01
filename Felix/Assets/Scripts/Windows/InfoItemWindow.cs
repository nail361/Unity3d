using UnityEngine;
using UnityEngine.UI;

public class InfoItemWindow : BaseWindow {

    [SerializeField]
    private Text itemTitle;
    [SerializeField]
    private Text itemDescription;
    [SerializeField]
    private Image itemImage;

	void Start () {
        _animation = GetComponent<Animation>();
        anim_name = "info_item";
        DeactivateCamera();
    }

    public void Init(string itemName, Sprite itemSprite)
    {
        itemTitle.text = itemName;
        itemDescription.text = LanguageManager.GetText(itemName+"Info");
        itemImage.sprite = itemSprite;
    }

}
