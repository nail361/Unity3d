using System.Collections;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class ModelLoader : MonoBehaviour {

    private static readonly string server_url = "http://prodesign.mediaidea.net/getmodel/?";
    private string model_url = "";

    [SerializeField]
    private Text warningTextField;
    [SerializeField]
    private GameObject loadingPanel;
    [SerializeField]
    private Slider loadingProgress;

    [SerializeField]
    private GameObject AskPanel;

    public void LoadModel(string model_url)
    {
        this.model_url = model_url;
        loadingPanel.SetActive(true);

        if (Caching.ClearCache())
        {
            Debug.Log("Successfully cleaned the cache.");
        }
        else
        {
            Debug.Log("Cache is being used.");
        }

        StartCoroutine("DownloadAssetBundle");
    }

    public IEnumerator DownloadAssetBundle()
    {
        while (!Caching.ready)
        {
            OnBundleLoaded();
            yield return null;
        }

        using (UnityWebRequest webRequest = UnityWebRequestAssetBundle.GetAssetBundle(server_url + model_url))
        {

            yield return webRequest.SendWebRequest();

            if (webRequest.isNetworkError || webRequest.isHttpError)
            {
                warningTextField.text = "Ошибка загрузки: " + webRequest.error;
                StartCoroutine(ReloadStage());
                throw new System.Exception("WWW download:" + webRequest.error + " url:" + webRequest.url);
            }
            else
            {
                AssetBundle assetBundle = DownloadHandlerAssetBundle.GetContent(webRequest);

                AssetBundleRequest request = assetBundle.LoadAssetAsync("model.prefab", typeof(GameObject));
                loadingProgress.value = 100 * request.progress;
                yield return request;
                GameObject model = Instantiate(request.asset as GameObject);
                Models._instance.AddModel(model, model_url);

                assetBundle.Unload(false);
                OnBundleLoaded();
            }
        }

    }

    private IEnumerator ReloadStage()
    {
        yield return new WaitForSeconds(3);
        SceneManager.LoadScene("QR_scene");
    }

    private void OnBundleLoaded()
    {
        AskPanel.SetActive(true);
        loadingPanel.SetActive(false);
    }

    public void LoadAnoherModel()
    {
        SceneManager.LoadScene("QR_scene");
    }

    public void LoadARScene()
    {
        SceneManager.LoadScene("AR_scene");
    }
}
