using UnityEngine;
using UnityEngine.UI;

public class FinalPanel : MonoBehaviour {

    [SerializeField]
    private GameObject YesNoPanel;
    [SerializeField]
    private Button[] teams_btn;

    [SerializeField]
    private GameObject TeamPanel;

    void Start()
    {
        for (int i = 0; i < teams_btn.Length; i++)
        {
            if (i >= Game.teams.Count)
            {
                teams_btn[i].GetComponentInChildren<Text>().text = "не участвует";
                teams_btn[i].interactable = false;
            }
            else
                teams_btn[i].GetComponentInChildren<Text>().text = Game.teams[i].name;
        }
    }

    void Setup()
    {
        for(int i = 0; i < Game.teams.Count; i++)
        {
            if (Game.teams[GameManager.cur_team].pos_index != Game.teams[i].pos_index)
            {
                teams_btn[i].interactable = false;
            }
            else
                teams_btn[i].interactable = true;
        }
    }

	void OnEnable () {
        YesNoPanel.SetActive(true);
        TeamPanel.SetActive(false);
    }
	
	public void YesClick()
    {
        if (GameManager.multi_game) {
            YesNoPanel.SetActive(false);
            TeamPanel.SetActive(true);
            Setup();
        }
        else
        {
            SoundManager.instance.PlaySound("success");
            GameManager.NextTurn(GameManager.cur_team);
            gameObject.SetActive(false);
        }
    }

    public void TeamSuccess(int team)
    {
        SoundManager.instance.PlaySound("success");
        GameManager.NextTurn(team);
        gameObject.SetActive(false);
    }

    public void NoClick()
    {
        SoundManager.instance.PlaySound("fail");
        GameManager.NextTurn(-1);
        gameObject.SetActive(false);
    }
}
