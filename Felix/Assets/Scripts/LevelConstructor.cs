using UnityEngine;
using System;
public class LevelConstructor : MonoBehaviour {

    [SerializeField]
    private Material sprite_material;

    [SerializeField]
    private Transform tilePlace;

    [SerializeField]
    private Transform objectPlace;

    [SerializeField]
    private GameObject prefPlayer;

    [SerializeField]
    private GameObject prefStar;

    [SerializeField]
    private GameObject prefFastFloor;
    [SerializeField]
    private GameObject prefSlowFloor;
    [SerializeField]
    private GameObject prefBox;
    [SerializeField]
    private GameObject prefLaser;
    [SerializeField]
    private GameObject prefInteractive;

    [SerializeField]
    private GameObject prefEnemy;

    [SerializeField]
    private GameObject prefExit;

    public static AtlasHolder atlasHolder;
    //private List<uint[]> level_tiles;
    //private List<uint[]> objects;

    void Awake () {

        atlasHolder = ((GameObject)Resources.Load("Atlases/atlas_stage_" + GameManager.instance.cur_stage)).GetComponent<AtlasHolder>();
        LoadLevelConfig();

        Resources.UnloadUnusedAssets();
    }

    private void LoadLevelConfig()
    {
        TextAsset info = (TextAsset)Resources.Load("Levels/Stage_"+GameManager.instance.cur_stage + "/level_" + GameManager.instance.cur_level, typeof(TextAsset));
        EDITOR.JSONObject json = new EDITOR.JSONObject(info.text);
        //Generate Level
        int x;
        int y = 0;
        //level_tiles = new List<uint[]>();
        foreach (EDITOR.JSONObject j in json.list[GameManager.instance.cur_mode*3].list)
        {
            //uint[] line = new uint[j.Count];
            x = 0;
            foreach (EDITOR.JSONObject tile in j.list)
            {
                //line[x] = (uint)tile.n;
                CreateTile(x,y, (int)tile.n);
                x++;
            }

            y++;

            //level_tiles.Add( line );
        }
        //End

        //Generate objects
        y = 0;
        //objects = new List<uint[]>();
        foreach (EDITOR.JSONObject j in json.list[GameManager.instance.cur_mode*3+1].list)
        {
            //uint[] line = new uint[j.Count];
            x = 0;
            foreach (EDITOR.JSONObject tile in j.list)
            {
                int special = 0;
                int index;
                try
                {
                    string[] arr = tile.str.Split('_');
                    //line[x] = (uint)Int32.Parse(arr[0]);
                    index = Int32.Parse(arr[0]);
                    special = Int32.Parse(arr[1]);
                }
                catch (NullReferenceException)
                {
                    //line[x] = (uint)tile.n;
                    index = (int)tile.n;
                }

                //CreateObject(x, y, line[x], rotation);
                CreateObject(x, y, index, special);
                x++;
            }

            y++;

            //objects.Add(line);
        }
        
        GameManager.instance.level_params = new int[json.list[GameManager.instance.cur_mode * 3 + 2].list.Count];
        int i = 0;
        foreach (EDITOR.JSONObject param in json.list[GameManager.instance.cur_mode * 3 + 2].list)
        {
            GameManager.instance.level_params[i] = (int)param.n;
            i++;
        }
        
        //End
    }

    private void CreateTile(int x, int y, int tile_index)
    {
        //0 - темнота; 1-10 - стены; 11-infinity - пол;
        if (tile_index == 0) return;

        GameObject go = new GameObject("tile_" + x + "_" + y, typeof(SpriteRenderer));
        go.transform.position = new Vector3(x, -y, 0);
        go.transform.parent = tilePlace;
        go.GetComponent<SpriteRenderer>().sprite  = atlasHolder.GetTile(tile_index);
        go.GetComponent<SpriteRenderer>().sortingLayerName = "floor";
        go.GetComponent<SpriteRenderer>().material = sprite_material;

        if (tile_index <= 10)
        {
            go.AddComponent<BoxCollider2D>();
            go.tag = "Block";
            go.layer = 9;
        }

    }

    private void CreateObject(int x, int y, int object_index, int special)
    {
        if (object_index == 0) return;//0- ничего

        //1 - Player; 2-14 - враги; 15 - сосиска; 16-80 - предметы; 81-98 - интерактивные предметы; 99 - выход
        GameObject go;

        if (object_index >= 16 && object_index <= 80)
        {
            go = new GameObject("object_" + x + "_" + y, typeof(SpriteRenderer));
            go.transform.position = new Vector3(x, -y, 0);
            go.transform.Rotate(new Vector3(0, 0, special*90));
            go.transform.parent = objectPlace;
            go.name = "object_" + x + "_" + y;
            go.GetComponent<SpriteRenderer>().sprite = atlasHolder.GetObject(object_index);
            go.GetComponent<SpriteRenderer>().sortingLayerName = "object";
            go.GetComponent<SpriteRenderer>().material = sprite_material;
            go.AddComponent<BoxCollider2D>();
            go.tag = "Block";
            go.layer = 9;
        }
        else if (object_index >= 85 && object_index <= 98)
        {
            ((GameObject)Instantiate(prefInteractive, new Vector3(x, -y, 0), Quaternion.Euler(0, 0, special * 90))).GetComponent<InteractiveObjects>().Init(object_index,special);
        }
        else 
        {
            switch (object_index)
            {
                case 1://Player
                    go = (GameObject)Instantiate(prefPlayer, new Vector3(x, -y, 0), Quaternion.Euler(0, 0, special * 90));
                    go.GetComponent<Player>().Init();
                    break;
                case 15://Star
                    go = (GameObject)Instantiate(prefStar, new Vector3(x, -y, 0), Quaternion.identity);
                    go.name = "Star";
                    break;
                case 81:
                    go = (GameObject)Instantiate(prefFastFloor, new Vector3(x, -y, 0), Quaternion.Euler(0, 0, special * 90));
                    break;
                case 82:
                    go = (GameObject)Instantiate(prefSlowFloor, new Vector3(x, -y, 0), Quaternion.Euler(0, 0, special * 90));
                    break;
                case 83:
                    go = (GameObject)Instantiate(prefBox, new Vector3(x, -y, 0), Quaternion.Euler(0, 0, special * 90));
                    break;
                case 84:
                    go = (GameObject)Instantiate(prefLaser, new Vector3(x, -y, 0), Quaternion.Euler(0, 0, special * 90));
                    break;
                case 99:
                    go = (GameObject)Instantiate(prefExit, new Vector3(x, -y, 0), Quaternion.Euler(0, 0, special * 90));
                    go.name = "Exit";
                    break;

                default:
                    go = (GameObject)Instantiate(prefEnemy, new Vector3(x, -y, 0), Quaternion.identity);
                    go.GetComponent<Enemy>().enemy_level = object_index - 2;
                    go.GetComponent<Enemy>().special = special;
                    break;
            }
            go.GetComponent<SpriteRenderer>().sprite = atlasHolder.GetObject(object_index);

        }
        
    }

}