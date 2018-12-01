using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ModelListWindow : MonoBehaviour {

    public GameObject listItemPref;
    public Transform listPlacement;

    private Animator animator;
    bool opened = false;

	void Start ()
    {
        animator = GetComponent<Animator>();

        FillModelsList();
    }

    private void FillModelsList()
    {
        for (int i = 0; i < Models.ModelsCount; i++)
        {
            GameObject item = Instantiate(listItemPref);
            item.transform.SetParent(listPlacement);
            item.GetComponent<ListItem>().OnSelectItem += new ListItem.ItemHandler(SelectItem);
            item.GetComponent<ListItem>().OnRemoveItem += new ListItem.ItemHandler(ItemRemove);
        }
    }

    private void SelectItem(ListItem Sender)
    {
        
    }

    private void ItemRemove(ListItem Sender)
    {
        Models.Remove(Sender.itemID);
    }

    public void OpenCloseWindow()
    {
        opened = !opened;

        animator.SetBool("open", opened);
    }
}
