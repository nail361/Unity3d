using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Collections.Generic;
using System.Collections;
using System;

public class MainMenu : MonoBehaviour {

    public delegate void Sizer();

    public static Sizer onSizeChange;
    [SerializeField]
    private GameObject logo;
    [SerializeField]
    private LoadingPanel loadingPanel;
    [SerializeField]
    private Toggle random_toggle;
    [SerializeField]
    private Toggle profi_toggle;
    [SerializeField]
    private Transform Content;
    [SerializeField]
    private GameObject TeamPref;
    [SerializeField]
    private Button deleteTeamBtn;
    [SerializeField]
    private Button addTeamBtn;
    [SerializeField]
    private InputField length_input;

    private RectTransform content_transform;

    private List<TeamPanel> teams;
    
	void Start () {
        content_transform = Content.GetComponent<RectTransform>();

        onSizeChange += CalculateContentSize;

        teams = new List<TeamPanel>();
        deleteTeamBtn.interactable = false;

        if (GameManager.teams.Count > 0)
        {
            GenerateTeams();
            GameManager.ResetTeams();
        }
        else
            AddTeam();

        StartCoroutine("DestroyLogo");
    }

    private IEnumerator DestroyLogo()
    {
        float timer = 2.0f;

        while (timer > 0)
        {
            timer -= Time.deltaTime;
            yield return null;
        }

        Destroy(logo);

        yield return null;
    }

    private void GenerateTeams()
    {
        for(int i = 0; i < GameManager.teams.Count; i++)
        {
            AddTeam();
            teams[i].SetTeamName(GameManager.teams[i]);
            teams[i].SetPlayers(GameManager.players[i]);
        }
    }

    public void AddTeam()
    {
        GameObject team = Instantiate(TeamPref);
        team.GetComponent<TeamPanel>().SetColor( teams.Count % 2 == 0 ? Color.white : Color.grey );
        team.transform.SetParent(Content,false);
        teams.Add(team.GetComponent<TeamPanel>());

        if (teams.Count >= 4)
        {
            addTeamBtn.interactable = false;
        }
        else if (teams.Count >= 2)
            deleteTeamBtn.interactable = true;
    }

    public void RemoveTeam()
    {
        Destroy(teams[teams.Count - 1].gameObject);
        teams.RemoveAt(teams.Count-1);

        addTeamBtn.interactable = true;

        if (teams.Count <= 1)
        {
            deleteTeamBtn.interactable = false;
        }

        onSizeChange();
    }

    private void CalculateContentSize()
    {
        float _height = 10;

        foreach(TeamPanel tp in teams)
        {
            _height += tp.GetHeight() + 5;
        }

        content_transform.sizeDelta = new Vector2(content_transform.sizeDelta.x, _height);
    }

    public void SetRandomMap()
    {
        GameManager.RandomMap = random_toggle.isOn;
    }

    public void SetProfiMode()
    {
        GameManager.ProfiMode = profi_toggle.isOn;
    }

    public void UpdateWords()
    {
        if (Application.internetReachability == NetworkReachability.NotReachable)
        {
            loadingPanel.ShowLoading("Нет подключения к интернету", "");
            Invoke("HideLoading", 1.5f);
        }
        else GameManager.instance.StartCheckUpdate();
    }

    private void HideLoading()
    {
        loadingPanel.HideLoading();
    }

    public void CheckLengthInputField()
    {
        int value = Int32.Parse(length_input.text);

        if (value < 5) value = 5;
        else if (value > 99) value = 99;

        length_input.text = value.ToString();
    }

    public void StartGame()
    {
        foreach( TeamPanel team in teams)
        {
            GameManager.FillTeam(team.GetTeamName(), team.GetPlayers());
        }

        GameManager.FinishPoints = Int32.Parse(length_input.text);

        SceneManager.LoadScene("game");
    }

    public void ExitGame()
    {
        Application.Quit();
    }

    void OnDestroy()
    {
        onSizeChange -= CalculateContentSize;
    }
}
