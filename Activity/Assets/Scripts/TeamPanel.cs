using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

public class TeamPanel : MonoBehaviour {

    [SerializeField]
    private InputField TeamName;
    [SerializeField]
    private GameObject PlayerPref;
    [SerializeField]
    private Transform PlayersPlace;

    private LayoutElement rect_transform;

    private List<InputField> players;
    private string[] playersNames = new string[0];

    void Awake()
    {
        rect_transform = GetComponent<LayoutElement>();
    }

    void Start () {
        players = new List<InputField>();

        if (playersNames.Length == 0)
        {
            AddPlayer();
            AddPlayer();
        }
        else
        {
            for(int i = 0; i < playersNames.Length; i++)
            {
                AddPlayer();
                players[i].text = playersNames[i];
            }
        }
    }

    public void SetColor( Color _color )
    {
        GetComponent<Image>().color = _color;
    }

    private void ChangeHeight()
    {
        float _height = 50 + (players.Count * 32);

        rect_transform.preferredHeight = _height;

        MainMenu.onSizeChange();
    }

    public float GetHeight()
    {
        return rect_transform.preferredHeight;
    }

    public void AddPlayer()
    {
        GameObject player = Instantiate(PlayerPref);
        player.transform.SetParent(PlayersPlace,false);
        players.Add(player.GetComponent<InputField>());

        ChangeHeight();
    }

    public void RemovePlayer()
    {
        if (players.Count == 2) return;

        Destroy(players[players.Count - 1].gameObject);
        players.RemoveAt(players.Count - 1);

        ChangeHeight();
    }

    public void SetTeamName(string name)
    {
        TeamName.text = name;
    }

    public void SetPlayers(string[] playersNames)
    {
        this.playersNames = playersNames;
    }

    public string GetTeamName()
    {
        string _name = TeamName.text;

        if (_name.Length == 0) _name = "Команда";

        return _name;
    }
    
    public string[] GetPlayers()
    {
        string[] playersName = new string[players.Count];

        for (int i = 0; i < players.Count; i++)
        {
            playersName[i] = players[i].text;

            if (playersName[i].Length == 0)
                playersName[i] = "Игрок_" + i;
        }

        return playersName;
    }

}
