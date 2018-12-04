﻿using UnityEngine;
using UnityEngine.UI;

public class ListItem : MonoBehaviour {

    public int itemID;

    [Header("Item Fields")]
    [SerializeField]
    private Text nameField;
    [SerializeField]
    private Text descriptionField;
    [SerializeField]
    private Text priceField;
    [SerializeField]
    private GameObject removeBtn;

    [Header("Color active and inactive list")]
    [SerializeField]
    private Color activeColor;
    [SerializeField]
    private Color inactiveColor;

    private Image image;

    private static int curSelectedID = 0;
    
    public delegate void ItemHandler(int modelId);

    public event ItemHandler OnRemoveItem;
    public event ItemHandler OnSelectItem;

    private void Start()
    {
        image = GetComponent<Image>();
    }

    public void Init(ModelInfo modelInfo, int id)
    {
        if (nameField) nameField.text = modelInfo.name;
        if (descriptionField) descriptionField.text = modelInfo.description;
        if (priceField) priceField.text = modelInfo.price;

        SetID(id);
    }

    public void SetID(int id)
    {
        itemID = id;
    }

    void Update()
    {
        removeBtn.SetActive(curSelectedID == itemID && Models.ModelsCount > 1);
        image.color = curSelectedID == itemID ? activeColor : inactiveColor;
    }

    public void SelectItem()
    {
        curSelectedID = itemID;
        OnSelectItem(curSelectedID);
    }

    public void RemoveItem()
    {
        OnRemoveItem(itemID);
        curSelectedID = 0;
        OnSelectItem(curSelectedID);
        Destroy(gameObject);
    }
}