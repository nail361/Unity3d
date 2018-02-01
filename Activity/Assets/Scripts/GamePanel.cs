using UnityEngine;
using UnityEngine.UI;

public class GamePanel : MonoBehaviour {

    [SerializeField]
    private Text team_title;
    [SerializeField]
    private Text player_title;
    [SerializeField]
    private Image game_type_img;
    [SerializeField]
    private WordPanel word_panel;

    [SerializeField]
    private Sprite[] sprites;

    private Game.GAME_TYPE type;

    public void Init () {

        Team team = Game.teams[GameManager.cur_team];

        team_title.text = "Команда: " + team.name;
        team_title.color = Color.white;
        player_title.text = "Игрок: " + team.GetPlayer();
        type =  Game.map_tiles[team.pos_index].GetGameType();

        bool multi = false;

        foreach (Team _team in Game.teams)
        {
            if (_team.team_index == team.team_index) continue;

            if (_team.pos_index == team.pos_index)
            {
                multi = true;
                team_title.text += ", " + _team.name;
            }
        }

        if (multi)
        {
            team_title.color = Color.green;
            player_title.text += " (" + team.name + ")";
            GameManager.multi_game = true;
            SoundManager.instance.PlaySound("all_team");
        }
        else
            GameManager.multi_game = false;

        game_type_img.sprite = sprites[(int)type];

        gameObject.SetActive(true);
	}
	
	public void ChooseDifficult(int diff)
    {
        word_panel.Init(game_type_img.sprite, GameManager.GetWord(diff, type));
        gameObject.SetActive(false);
    }
}
