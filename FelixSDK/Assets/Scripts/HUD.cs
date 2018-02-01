using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System;
using System.IO;

public class HUD : MonoBehaviour {

    public static Text Console;
    public static LayerMask working_layer = 1 << 8;

    private uint itemType = 0;
    [SerializeField]
    private GameObject itemPref;
    [SerializeField]
    private Slider slider;
    [SerializeField]
    private Toggle override_toggle;
    [SerializeField]
    private Text index_text;
    [SerializeField]
    private InputField json_name;
    [SerializeField]
    private Image preview_image;
    [SerializeField]
    private Transform tiles_holder;
    [SerializeField]
    private Transform objects_holder;
    [SerializeField]
    private Transform grid_holder;

    [SerializeField]
    private GameObject gridPref;
    [SerializeField]
    private InputField width_text;
    [SerializeField]
    private InputField height_text;

    [SerializeField]
    private InputField posX_text;
    [SerializeField]
    private InputField posY_text;

    private int cur_index = 1;

    public static int width = 10;
    public static int height = 10;

    private AtlasHolder atlas_holder;

    private int cur_stage = 0;
    private int max_stage = 1;

    void Start() {

        Console = GameObject.Find("Console").GetComponentInChildren<Text>();

        ChangeStageClick();

        SetItem(1);

        SetGrid();
    }

#if UNITY_EDITOR || UNITY_STANDALONE
    private float waiteUpdate = 0.0f;
    void Update()
    {
        if (Input.GetMouseButton(2))
        {
            waiteUpdate -= Time.deltaTime;

            if (waiteUpdate <= 0)
            {
                Vector2 rayPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
                RaycastHit2D hit = Physics2D.Raycast(rayPos, Vector2.zero, 0f, HUD.working_layer);
                if (!hit)
                {
                    CreateItem(new Vector2(Mathf.Floor(rayPos.x+0.5f), Mathf.Floor(-(rayPos.y-0.5f))));
                    waiteUpdate = 0.3f;
                }
            }
        }
    }
#endif

    public void SetGrid()
    {
        try
        {
            width = Int32.Parse(width_text.text);
            height = Int32.Parse(height_text.text);
        }
        catch
        {
            width = 10;
            height = 10;
            Console.text = "Ошибка, выставлено в 10;10";
        }

        for (int c = 0; c < grid_holder.childCount; c++)
            Destroy(grid_holder.GetChild(c).gameObject);

        for (uint i = 0; i < width; i++)
            for (uint j = 0; j < height; j++)
            {
                (Instantiate(gridPref, new Vector3(i, -j, 0), Quaternion.identity) as GameObject).transform.parent = grid_holder;
            }
    }

    private void SetItem(int delta = 0)
    {
        index_text.text = cur_index.ToString();
        preview_image.sprite = itemType == 0 ? atlas_holder.GetTile(cur_index) : atlas_holder.GetObject(cur_index);

        if (preview_image.sprite == null)
        {
            ChangeIndex(delta);
        }
    }

    public void ChangeItemType()
    {
        itemType = (uint)slider.value;

        working_layer.value = 1 << (int)(itemType + 8);

        cur_index = 1;
        SetItem();
    }

    public void ChangeIndex(int delta)
    {
        cur_index += delta;

        if (cur_index < 1) cur_index = 1;
        else
        {
            if (itemType == 0)
            {
                if (cur_index >= atlas_holder.LengthTiles) cur_index = atlas_holder.LengthTiles - 1;
            }
            else
                if (cur_index >= atlas_holder.LengthObjects) cur_index = atlas_holder.LengthObjects - 1;
        }

        SetItem(delta > 0 ? 1 : -1);
    }

    public void ChangeStageClick()
    {
        atlas_holder = ((GameObject)Resources.Load("Atlases/atlas_stage_" + cur_stage)).GetComponent<AtlasHolder>();

        cur_stage++;

        if (cur_stage >= max_stage)
        {
            cur_stage = 0;
        }
    }

    public void CreateItem()
    {
        try
        {
            CreateItem( new Vector2(Int32.Parse(posX_text.text), Int32.Parse(posY_text.text)));
        }
        catch
        {
            Console.text = "Некорректное поле";
            return;
        }
    }

    private void CreateItem(Vector2 pos)
    {
        if (pos.x >= width || pos.y >= height || pos.x < 0 || pos.y < 0)
        {
            Console.text = "Некорректное поле";
            return;
        }

        if (GameObject.Find((itemType == 0 ? "tile_" : "object_") + pos.x + "," + pos.y)) {
            Console.text = "В этом месте уже что-то есть";
        }
        else {
            ObjectItem obj_item = (Instantiate(itemPref, Vector3.zero, Quaternion.identity) as GameObject).GetComponent<ObjectItem>();
            obj_item.Init(itemType == 0 ? atlas_holder.GetTile(cur_index) : atlas_holder.GetObject(cur_index), itemType, cur_index, 0, pos, itemType == 0 ? tiles_holder : objects_holder);
        }
    }

    public void ResetCLICK()
    {
        SceneManager.LoadScene(0);
    }

    public void SaveJSON()
    {
        if (json_name.text.Length == 0) {
            Console.text = "Имя не может быть пустым ";
            return;
        }

        if (File.Exists(Application.persistentDataPath + "/" + json_name.text + ".json"))
        {
            Console.text = json_name.text + " уже существует";

            if (!override_toggle.isOn) return;
        }

        int i = 0;
        int j = 0;
        string tile_line = "\"mode_X_level\": [";
        string object_line = "\"mode_X_object\": [";
        GameObject tile_item;
        GameObject object_item;

        for (i = 0; i < height; i++)
        {
            tile_line += "[";
            object_line += "[";

            for (j = 0; j < width; j++)
            {
                tile_item = GameObject.Find("tiles_holder/tile_" + j + "," + i);
                if (tile_item == null)
                {
                    tile_line += "0";
                }
                else
                {
                    tile_line += tile_item.GetComponent<ObjectItem>().GetIndex();
                }

                object_item = GameObject.Find("objects_holder/object_" + j + "," + i);
                if (object_item == null)
                {
                    object_line += "0";
                }
                else
                {
                    object_line += object_item.GetComponent<ObjectItem>().GetIndex();
                }

                if (width - j != 1) {
                    tile_line += ",";
                    object_line += ",";
                }
            }

            tile_line += "]";
            object_line += "]";

            if (height - i != 1)
            {
                tile_line += ",";
                object_line += ",";
            }
        }

        tile_line += "],";
        object_line += "],";

        using (StreamWriter file = File.CreateText(Application.persistentDataPath + "/" + json_name.text + ".json"))
        {
            file.WriteLine("{");

            file.WriteLine(tile_line);
            file.WriteLine(object_line);

            file.WriteLine("\"mode_X_param\": [1]");
            file.Write("}");

            file.Close();
        }
        Console.text = "SUCESS - " + Application.persistentDataPath + "/" + json_name.text + ".json ";
    }

    public void ClearConsole()
    {
        Console.text = "";
    }

    public void LoadJSON()
    {
        if (!File.Exists(Application.persistentDataPath + "/" + json_name.text + ".json"))
        {
            Console.text = json_name.text + " не существует ";
            return;
        }

        JSONObject json = new JSONObject(File.ReadAllText(Application.persistentDataPath + "/" + json_name.text + ".json"));
        ObjectItem obj_item;
        //Generate Level
        int x = 0;
        int y = 0;
        foreach (JSONObject j in json.list[0].list)
        {
            x = 0;
            foreach (JSONObject tile in j.list)
            {
                if ((int)tile.n != 0)
                {
                    obj_item = (Instantiate(itemPref, Vector3.zero, Quaternion.identity) as GameObject).GetComponent<ObjectItem>();
                    obj_item.Init(atlas_holder.GetTile((int)tile.n), 0, (int)tile.n, 0, new Vector2(x, y), tiles_holder);
                }

                x++;
            }

            y++;
        }
        //End

        //Generate objects
        y = 0;
        foreach (JSONObject j in json.list[1].list)
        {
            x = 0;
            foreach (JSONObject tile in j.list)
            {
                int special = 0;
                int index;
                try
                {
                    string[] arr = tile.str.Split('_');
                    index = Int32.Parse(arr[0]);
                    special = Int32.Parse(arr[1]);
                }
                catch (NullReferenceException)
                {
                    index = (int)tile.n;
                }

                if (index != 0)
                {
                    obj_item = (Instantiate(itemPref, Vector3.zero, Quaternion.identity) as GameObject).GetComponent<ObjectItem>();
                    obj_item.Init(atlas_holder.GetObject(index), 1, index, special, new Vector2(x, y), objects_holder);
                }
                x++;
            }

            y++;

        }

        width_text.text = x.ToString();
        height_text.text = y.ToString();
        SetGrid();
    }

}