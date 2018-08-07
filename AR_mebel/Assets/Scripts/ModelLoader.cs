using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ModelLoader : MonoBehaviour {

    private string model_url = "";

    public void LoadModel(string model_url)
    {
        this.model_url = model_url;
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
            yield return www;
            if (www.error != null) {
                throw new System.Exception("WWW download:" + www.error + " url:" + www.url);
                //Писать что ссылка не правильная
            }

            AssetBundle assetBundle = www.assetBundle;

            AssetBundleRequest request = assetBundle.LoadAssetAsync("Cube.prefab", typeof(GameObject));
            yield return request;
            GameObject model = request.asset as GameObject;
            model.AddComponent<Model>();
            OnBundleLoaded();
            assetBundle.Unload(false);
        }
    }

    private void OnBundleLoaded()
    {
        SceneManager.LoadScene("AR_scene");
    }
}
