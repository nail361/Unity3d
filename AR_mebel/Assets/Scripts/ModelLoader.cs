using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class ModelLoader : MonoBehaviour {

    private string model_url = "";

    [SerializeField]
    private Text warningTextField;
    [SerializeField]
    private GameObject loadingPanel;
    [SerializeField]
    private Slider loadingProgress;

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

        using (WWW www = WWW.LoadFromCacheOrDownload("file://" + Application.dataPath + "/AssetBundles/model_01", 0))
        {
            loadingProgress.value = 100 * www.progress;
            yield return www;
            if (www.error != null) {
                warningTextField.text = "Ошибка загрузки: " + www.error;
                //StartCoroutine(ReloadStage());
                throw new System.Exception("WWW download:" + www.error + " url:" + www.url);
            }

            AssetBundle assetBundle = www.assetBundle;

            AssetBundleRequest request = assetBundle.LoadAssetAsync("Model_01.prefab", typeof(GameObject));
            loadingProgress.value = 100 * www.progress;
            yield return request;
            GameObject model = Instantiate(request.asset as GameObject);
            model.AddComponent<Model>();

            assetBundle.Unload(false);
            OnBundleLoaded();
        }
    }

    private IEnumerator ReloadStage()
    {
        yield return new WaitForSeconds(3);
        SceneManager.LoadScene("QR_scene");
    }

    private void OnBundleLoaded()
    {
        SceneManager.LoadScene("AR_scene");
    }
}
