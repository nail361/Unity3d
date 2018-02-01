using UnityEngine;
#if UNITY_IOS || UNITY_ANDROID
using UnityEngine.Advertisements;
#endif

public class QuestionPanel : MonoBehaviour {

    private string zoneID = null;

	void Start () {
        transform.SetParent( GameObject.Find("HUD").transform, false );
	}

#if UNITY_IOS || UNITY_ANDROID
    public void OnAcceptClick()
    {
        if (Advertisement.IsReady(zoneID))
        {
            var options = new ShowOptions { resultCallback = OnShowResult };
            Advertisement.Show(zoneID, options);
        }
        else
        {
            GameManager.instance.UpdateWords();
            Destroy(gameObject);
        }
    }

    private void OnShowResult(ShowResult result)
    {
        switch (result)
        {
            case ShowResult.Finished:
                GameManager.instance.UpdateWords();
                break;
        }

        Destroy(gameObject);
    }
#else
    public void OnAcceptClick()
    {
        GameManager.instance.UpdateWords();
        Destroy(gameObject);
    }
#endif

    public void OnDenyClick()
    {
        Destroy(gameObject);
    }

}
