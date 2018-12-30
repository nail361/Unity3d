using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Models : MonoBehaviour {

    public static int ModelsCount{
        get{
            return models.Count;
        }
    }

    private static List<GameObject> models;

    private static List<ModelInfo> modelsInfo;

    public static Models _instance;

    private void Awake()
    {
        if (_instance != null) return;

        _instance = this;
        DontDestroyOnLoad(gameObject);
        models = new List<GameObject>();
        modelsInfo = new List<ModelInfo>();
        SceneManager.sceneLoaded += OnLevelLoaded;
    }

    private void OnLevelLoaded(Scene scene, LoadSceneMode mode)
    {
        if (scene.name != "AR_scene") return;

        Transform parent = GameObject.FindGameObjectWithTag("Player").transform;
        if (parent)
            models.ForEach( (model)=> model.transform.SetParent(parent));
    }

    public static void AddModel(GameObject model, string model_url)
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

    public static void Remove(int modelIndex)
    {
        Destroy(models[modelIndex]);
        models.RemoveAt(modelIndex);
        modelsInfo.RemoveAt(modelIndex);
    }

    public static ModelInfo GetModelInfo(int modelIndex)
    {
        return modelsInfo[modelIndex];
    }

    public static GameObject GetModel(int modelIndex)
    {
        return models[modelIndex];
    }
    
    public static void HideModel(int modelIndex)
    {
        models[modelIndex].SetActive(false);
    }

    public static void ShowModel(int modelIndex)
    {
        models[modelIndex].SetActive(true);
    }
}
