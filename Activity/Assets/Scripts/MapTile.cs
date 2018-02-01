using UnityEngine;

public class MapTile : MonoBehaviour {

    [SerializeField]
    private Sprite[] sprites;
    private float angle;
    private Game.GAME_TYPE type = Game.GAME_TYPE.DRAW;

    public void Init(int index, float angle, Game.GAME_TYPE _type, Transform map)
    {
        name = "map_tile_" + index;
        type = _type;
        this.angle = angle;

        GetComponent<SpriteRenderer>().sprite = sprites[ (int)_type ];

        transform.SetParent( map, false );
    }

    public Game.GAME_TYPE GetGameType()
    {
        return type;
    }

    public Vector2 GetPos()
    {
        return transform.position;
    }

    public float GetAngle()
    {
        return angle;
    }

}
