using UnityEngine;
using UnityEngine.SceneManagement;

public class QuitRequestPanel : MonoBehaviour {

	void Start () {
        transform.SetParent(GameObject.Find("HUD").transform,false);
        transform.SetAsLastSibling();
    }
	
	public void onCancelHandler()
    {
        Destroy(gameObject);
    }

    public void onAcceptHandler()
    {
        SceneManager.LoadScene(0);
    }
}
