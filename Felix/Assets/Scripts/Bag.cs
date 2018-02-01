using UnityEngine;
using UnityEngine.UI;

public class Bag : MonoBehaviour
{
    public int index;

    [SerializeField]
    private Image bagIMG;
    [SerializeField]
    private Text bagTitle;
    [SerializeField]
    private Sprite emptyBag;

    [SerializeField]
    private ItemsHolder itemsHolder;

    private float lastClickTime = 0;

    protected virtual void Awake()
    {
        SetBag();
    }

    private void SetBag()
    {
        if (GameManager.instance.bag[index] == (int)ItemIndex.None)
        {
            bagIMG.sprite = emptyBag;
            bagIMG.color = new Color32(178,233,255,130);
            if (bagTitle!=null) bagTitle.text = "";
        }
        else
        {
            bagIMG.sprite = itemsHolder.GetSprite(GameManager.instance.bag[index]);
            bagIMG.color = new Color32(255,255,255,255);
            if (bagTitle != null) bagTitle.text = LanguageManager.GetText(itemsHolder.GetTitle(GameManager.instance.bag[index]));
        }
    }

    public void SetItem(ItemIndex value)
    {
        if ( value != ItemIndex.None & GameManager.instance.bag[index] != (int)ItemIndex.None)
            GameManager.instance.items[GameManager.instance.bag[index]]++;

        GameManager.instance.bag[index] = (int)value;
        SetBag();
    }

    public virtual void Clean()
    {
        if (GameManager.instance.bag[index] == (int)ItemIndex.None) return;

        if  (Mathf.Abs(lastClickTime - Time.realtimeSinceStartup) < 0.5f)
        {
            GameManager.instance.items[GameManager.instance.bag[index]]++;
            SetItem(ItemIndex.None);
            Inventory.instance.delUpdate();
            return;
        }

        lastClickTime = Time.realtimeSinceStartup;
    }
    
}