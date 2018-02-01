using UnityEngine;
using System.Collections.Generic;

public class AtlasHolder : MonoBehaviour
{
    [SerializeField]
    private List<Sprite> tiles;
    [SerializeField]
    private List<Sprite> objects;
    [SerializeField]
    private List<Sprite> interactive;

    public int LengthTiles
    {
        get
        {
            return tiles.Count;
        }
    }

    public int LengthObjects
    {
        get
        {
            return objects.Count;
        }
    }

    public Sprite GetTile(int index)
    {
        if (index < 0 || index >= tiles.Count)
            return tiles[0];

        if (tiles[index] == null)
            return tiles[0];

        return tiles[index];
    }

    public Sprite GetObject(int index)
    {
        if (index < 0 || index >= objects.Count)
            return objects[0];

        if (objects[index] == null)
            return objects[0];

        return objects[index];
    }

    public Sprite GetInteractiveByName(string name)
    {
        foreach(Sprite sprite in interactive)
        {
            if (sprite != null && sprite.name == name)
                return sprite;
        }

        return objects[0];
    }
    
}