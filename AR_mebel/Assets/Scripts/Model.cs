using UnityEngine;
using UnityEngine.SceneManagement;

public class Model : MonoBehaviour {

    private void Awake()
    {
        DontDestroyOnLoad(gameObject);
        SceneManager.sceneLoaded += OnLevelLoaded;
    }

    private void OnLevelLoaded(Scene scene, LoadSceneMode mode)
    {
        Transform parent = GameObject.FindGameObjectWithTag("Player").transform;
        if (parent) transform.SetParent(parent);
    }
}
