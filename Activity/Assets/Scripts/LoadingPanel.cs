using UnityEngine;
using UnityEngine.UI;

public class LoadingPanel : MonoBehaviour {

    [SerializeField]
    private Text title;
    [SerializeField]
    private Text info;
    [SerializeField]
    private GameObject loadingObject;

    void Start()
    {
        GameManager.instance.loading_panel = this;
        HideLoading();
    }

    public void ShowLoading(string title, string info)
    {
        loadingObject.SetActive(true);
        this.title.text = title;
        this.info.text = info;
    }

    public void HideLoading()
    {
        loadingObject.SetActive(false);
    }

    void OnDelete()
    {
        GameManager.instance.loading_panel = null;
    }

}
