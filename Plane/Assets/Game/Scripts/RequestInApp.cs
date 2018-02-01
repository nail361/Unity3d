using UnityEngine;

public class RequestInApp : MonoBehaviour {

    private Soomla.Store.InApp inAppPursh;

	void Start () {
        transform.SetParent(GameObject.Find("Settings").transform, false);
        transform.SetAsLastSibling();

        inAppPursh = GameObject.Find("InApp").GetComponent<Soomla.Store.InApp>();
    }
	
	public void onAcceptHandler()
    {
        inAppPursh.ClickBuy();
        Destroy(gameObject);
    }

    public void onCancelHandler()
    {
        Destroy(gameObject);
    }
}
