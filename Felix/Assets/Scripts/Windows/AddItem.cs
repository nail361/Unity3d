using UnityEngine;
using UnityEngine.UI;

public class AddItem : BaseWindow {

    private ItemIndex index;
    [SerializeField]
    private Text itemTitle;
    [SerializeField]
    private Image itemImage;

    [SerializeField]
    private Text itemCount;
    [SerializeField]
    private Text fishCount;
    [SerializeField]
    private Text textBtn;
    [SerializeField]
    private Slider slider;

    private FishPanel fish_panel;

    private uint count = 1;
    private uint cost;

    void Start () {
        DeactivateCamera();

        fish_panel = GameObject.Find("FishPanel").GetComponent<FishPanel>();

        textBtn.text = LanguageManager.GetText("Exchange");
        _animation = GetComponent<Animation>();
        anim_name = "add_item";
        OnChangeValue( );
    }

    public void SetItem(ItemIndex index, string itemName, Sprite itemSprite, int maxValue = 20 )
    {
        this.index = index;
        itemTitle.text = itemName;
        itemImage.sprite = itemSprite;
        slider.maxValue = maxValue;
        slider.value = 1;

        if (maxValue == 1) slider.interactable = false;
    }

    public void OnChangeValue( )
    {
        count = (uint)slider.value;
        itemCount.text = "X " + count.ToString();
        cost = index == ItemIndex.Milk ? GameManager.instance.MILK_COST[count-1] : (uint)Mathf.Ceil(count * GameManager.instance.ITEM_COST[(uint)index]);

        fishCount.text = cost.ToString();
    }

    public void ExchangeClick()
    {
        if (fish_panel.SubFish(cost))
        {
            SoundManager.instance.PlaySound("exchange");
            if (index == ItemIndex.Milk) {
                GameManager.instance.BuyMilk(count);
                if (GameManager.instance.MissMilk() == 0) CloseWindow();
                else
                {
                    SetItem(ItemIndex.Milk, LanguageManager.GetText("Milk"), itemImage.sprite, GameManager.instance.MissMilk());
                }
            }
            else{
                GameManager.instance.items[(uint)index] += count;
                Inventory.instance.delUpdate();
            }

            slider.value = slider.minValue;
            OnChangeValue();
        }
        else
        {
            SoundManager.instance.PlaySound("no_fish");
        }
    }

}