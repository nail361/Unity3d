﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ModelListWindow : MonoBehaviour {

    public GameObject listItemPref;
    public Transform listPlacement;

    private Animator animator;
    bool opened = false;

    private Player player;

	void Start ()
    {
        animator = GetComponent<Animator>();

        player = GameObject.FindGameObjectWithTag("Player").GetComponent<Player>();

        FillModelsList();
    }

    private void FillModelsList()
    {
        for (int i = 0; i < Models.ModelsCount; i++)
        {
            GameObject item = Instantiate(listItemPref, listPlacement, false);
            item.GetComponent<ListItem>().Init(Models.GetModelInfo(i), i);
            item.GetComponent<ListItem>().OnSelectItem += new ListItem.ItemHandler(SelectItem);
            item.GetComponent<ListItem>().OnRemoveItem += new ListItem.ItemHandler(ItemRemove);
        }
    }

    private void SelectItem(int itemID)
    {
        player.SelectModel(itemID);
    }

    private void ItemRemove(int itemID)
    {
        Models.Remove(itemID);

        ListItem[] items = listPlacement.GetComponentsInChildren<ListItem>();

        int i = 0;
        foreach(ListItem item in items)
        {
            item.SetID(i);
            i++;
        }
    }

    public void OpenCloseWindow()
    {
        opened = !opened;

        animator.SetBool("open", opened);
    }
}
