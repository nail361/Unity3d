using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class WinPanel : MonoBehaviour {

    [SerializeField]
    private Text title;

	public void Init( string team_name )
    {
        transform.SetParent(GameObject.Find("HUD").transform, false);
        title.text = "Победила команда: " + team_name;
    }

    public void RepeatClick()
    {
        SceneManager.LoadScene("main_menu");
    }

}
