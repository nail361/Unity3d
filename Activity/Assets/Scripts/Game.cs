using UnityEngine;
using System.Collections.Generic;
using UnityEngine.UI;

public class Game : MonoBehaviour {

    public enum GAME_TYPE : int { TELL, DRAW, SHOW };

    public static float Radius = 36 / (2 * Mathf.PI);
    public static float Rad = Mathf.PI / 180;
    public static float Pi2 = Mathf.PI / 2;

    [SerializeField]
    private GameObject quitRequestPanel;

    public static List<MapTile> map_tiles;
    public static List<Team> teams;

    [SerializeField]
    private GamePanel game_panel;
    [SerializeField]
    private GameObject map_tile;
    [SerializeField]
    private GameObject team_tile;
    [SerializeField]
    private Transform team_place;
    [SerializeField]
    private Transform map;

    [SerializeField]
    private Text finish_points_title;

	void Start () {
        SoundManager.instance.PlaySound("start");

        finish_points_title.text = GameManager.FinishPoints.ToString();

        GenerateMap();
        GenerateTeams();
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            QuitRequest();
        }
    }

    private void GenerateMap()
    {
        map_tiles = new List<MapTile>();

        GAME_TYPE type = 0;

        for (int i = 360; i > 0; i -= 10)
        {
            int j = i == 360 ? 0 : i;
            Vector3 point = new Vector3((Mathf.Cos(j * Rad - Pi2) * Radius), (Mathf.Sin(j * Rad - Pi2) * Radius), 0);
            Quaternion angle = Quaternion.Euler(0,0,j);
            map_tiles.Add(((GameObject)Instantiate(map_tile, point, angle)).GetComponent<MapTile>());

            if (GameManager.RandomMap)
            {
                type = RandomType();
            }
            else
            {
                if (type == GAME_TYPE.SHOW) type = GAME_TYPE.TELL;
                else type++;
            }

            //map_tiles[(360-i)/10].Init((360-i)/10, i, type, map);
            map_tiles[map_tiles.Count-1].Init(j / 10, j, type, map);
        }

    }

    private void GenerateTeams()
    {
        teams = new List<Team>();

        for (int i = 0; i < 4; i++)
        {
            if (i < GameManager.teams.Count)
            {
                teams.Add(Instantiate(team_tile).GetComponent<Team>());
                teams[i].Init(i, team_place);
            }
            else
            {
                Destroy(GameObject.Find("HUD/player_"+i));
            }
        }

        GameManager.cur_team = Random.Range(0, GameManager.teams.Count);
    }

    private GAME_TYPE RandomType()
    {
        return (GAME_TYPE)Random.Range(0,3);
    }

    public static Color32 GetColor( float angle )
    {
        Color32 color = Color.white;
        switch (map_tiles[ (int)Mathf.Round(angle/10) ].GetGameType())
        {
            case GAME_TYPE.TELL: color = new Color32(215,100,100,255); break;
            case GAME_TYPE.SHOW: color = new Color32(100,215,100,255); break;
            case GAME_TYPE.DRAW: color = new Color32(100,100,215,255); break;
        }
        return color;
    }

    public void PlayClick()
    {
        game_panel.Init();

        Camera.main.GetComponent<CameraControl>().GoToTarget( teams[GameManager.cur_team].transform, teams[GameManager.cur_team].GetAngle() );
    }

    public void QuitRequest()
    {
        Instantiate(quitRequestPanel);
    }

}
