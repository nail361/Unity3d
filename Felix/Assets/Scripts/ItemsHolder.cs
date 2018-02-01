using UnityEngine;

public class ItemsHolder : MonoBehaviour {

    [SerializeField]
    private string[] titles;
    [SerializeField]
    private Sprite[] sprites;
    [SerializeField]
    private Sprite defaultSprite;

    public string GetTitle( int index )
    {
        if (index >= titles.Length) return "none";
        return titles[index];
    }

    public Sprite GetSprite( int index)
    {
        if (index >= sprites.Length) return defaultSprite;
        return sprites[index];
    }

}