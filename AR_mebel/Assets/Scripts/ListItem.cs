using UnityEngine;
using UnityEngine.UI;

public class ListItem : MonoBehaviour {

    public int itemID;

    [SerializeField]
    private Text nameField;
    [SerializeField]
    private Text descriptionField;
    [SerializeField]
    private Text priceField;
    
    public delegate void ItemHandler(ListItem sender);

    public event ItemHandler OnRemoveItem;
    public event ItemHandler OnSelectItem;

    public void Init(ModelInfo modelInfo, int id)
    {
        if (nameField) nameField.text = modelInfo.name;
        if (descriptionField) descriptionField.text = modelInfo.description;
        if (priceField) priceField.text = modelInfo.price;

        itemID = id;
    }

    private void Update()
    {

    }

    public void RemoveItem()
    {
        OnRemoveItem(this);
    }
}
