using UnityEngine;
using UnityEngine.SceneManagement;

public class Model : MonoBehaviour {

    private static int modelCount = 0;

    private void Awake()
    {
        DontDestroyOnLoad(gameObject);

        Renderer[] modelRenders = GetComponentsInChildren<Renderer>();

        foreach(Renderer render in modelRenders)
        {
            render.enabled = false;
        }

        SceneManager.sceneLoaded += OnLevelLoaded;
    }

    private void OnLevelLoaded(Scene scene, LoadSceneMode mode)
    {
        gameObject.name = "Model_" + modelCount.ToString();
        modelCount++;

        Transform parent = GameObject.FindGameObjectWithTag("Player").transform;
        if (parent) transform.SetParent(parent);
    }
}
