using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class Team : MonoBehaviour {

    [SerializeField]
    private Sprite[] sprites;

    [SerializeField]
    private GameObject winPref;

    private Text points_title;

    public int pos_index;

    public int team_index;
    private float angle;

    private string[] players;

    private int cur_player;

    public int points = 0;

    public void Init(int index, Transform team_place)
    {
        name = GameManager.teams[index];
        players = GameManager.players[index];

        cur_player = Random.Range(0,players.Length);

        team_index = index;

        pos_index = 36 / 4 * team_index;
 
        GetComponent<SpriteRenderer>().sprite = sprites[index];

        transform.SetParent(team_place, false);

        points_title = GameObject.Find("HUD/player_"+index).GetComponentInChildren<Text>();

        GoToPos( 0 );
    }

    public string GetPlayer()
    {
        string name = players[cur_player];

        cur_player++;

        if (cur_player >= players.Length) cur_player = 0;

        return name;
    }

    public void GoToPos( int count )
    {
        pos_index += count;

        if (pos_index >= Game.map_tiles.Count)
            pos_index -= Game.map_tiles.Count;
        else if (pos_index < 0)
            pos_index = Game.map_tiles.Count + pos_index;

        if (count < 0)
        {
            if (GameManager.ProfiMode)
            {
                points += count;
                if (points < 0) points = 0;
            }
        }
        else
            points += count;
        
        points_title.text = points.ToString();
        angle = Game.map_tiles[pos_index].GetAngle();
        StartCoroutine("Moving");

        if (points >= GameManager.FinishPoints)
        {
            Instantiate(winPref).GetComponent<WinPanel>().Init(name);
            SoundManager.instance.PlaySound("win");
        }
    }

    IEnumerator Moving()
    {
        Quaternion startRotation = transform.rotation;
        Quaternion endRotation = Game.map_tiles[pos_index].transform.rotation;
        float scale = 0.0f;

        do {
            scale += Time.deltaTime;
            if (scale > 1) scale = 1;

            transform.rotation = Quaternion.Lerp(startRotation, endRotation, scale);
            transform.position = new Vector3((Mathf.Cos(transform.rotation.eulerAngles.z * Game.Rad - Game.Pi2) * (Game.Radius - team_index / 2.0f - 0.3f)), (Mathf.Sin(transform.rotation.eulerAngles.z * Game.Rad - Game.Pi2) * (Game.Radius - team_index / 2.0f - 0.3f)), 0);
            yield return new WaitForEndOfFrame();
        } while (scale < 1);
        
        yield return null;
    }

    public float GetAngle()
    {
        return angle;
    }

}
