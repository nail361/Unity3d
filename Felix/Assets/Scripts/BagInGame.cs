using UnityEngine;

public class BagInGame : Bag
{
    override protected void Awake()
    {
        base.Awake();
    }

    public void Use()
    {
        if (GameManager.instance.bag[index] == (int)ItemIndex.None) return;

        if (Player.instance.UseItem((ItemIndex)GameManager.instance.bag[index])) SetItem(ItemIndex.None);
    }

    public override void Clean()
    {
        
    }

}
