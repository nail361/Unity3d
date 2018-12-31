using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Models : MonoBehaviour {

    public int ModelsCount{
        get{
            return models.Count;
        }
    }

    public List<GameObject> models;

    private List<ModelInfo> modelsInfo;

    public static Models _instance;

    private void Awake()
    {
        if (_instance != null) {
            Destroy(gameObject);
            return;
        }
        else
        {
            _instance = this;
            DontDestroyOnLoad(gameObject);
            models = new List<GameObject>();
            modelsInfo = new List<ModelInfo>();
        }

        SceneManager.sceneLoaded += _instance.OnLevelLoaded;
    }

    private void OnLevelLoaded(Scene scene, LoadSceneMode mode)
    {
        if (scene.name != "AR_scene") return;

        Transform parent = GameObject.FindGameObjectWithTag("Player").transform;
        if (parent)
            models.ForEach((model) => model.transform.SetParent(parent) );
    }

    public void OnPlayerDestroy()
    {
        Debug.Log("PLAYER WAS DESTROYED");
        models.ForEach((model) => { Debug.Log(model.name); model.transform.SetParent(transform); });
    }

    public void AddModel(GameObject model, string model_url)
    {
        model.AddComponent<AnimManager>();
        models.Add(model);

        string new_url = model_url.Substring(model_url.IndexOf("&t=") + 3);
        string[] url_arr = new_url.Split('&');

        ModelInfo modelInfo = new ModelInfo
        {
            name = url_arr[0],
            description = "",
            price = url_arr[1].Substring(2)
        };
        modelsInfo.Add(modelInfo);

        DontDestroyOnLoad(model);

        HideModel(models.Count-1);

        model.name = "Model_" + models.Count.ToString();
    }

    public void Remove(int modelIndex)
    {
        Destroy(models[modelIndex]);
        models.RemoveAt(modelIndex);
        modelsInfo.RemoveAt(modelIndex);
    }

    public ModelInfo GetModelInfo(int modelIndex)
    {
        return modelsInfo[modelIndex];
    }

    public GameObject GetModel(int modelIndex)
    {
        return models[modelIndex];
    }
    
    public void HideModel(int modelIndex)
    {
        models[modelIndex].SetActive(false);
    }

    public void ShowModel(int modelIndex)
    {
        models[modelIndex].SetActive(true);
    }
}
