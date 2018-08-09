using UnityEngine;

public class Model : MonoBehaviour {

    private void Awake()
    {
        DontDestroyOnLoad(gameObject);
    }

    private void OnLevelWasLoaded()
    {
        Transform parent = GameObject.FindGameObjectWithTag("Player").transform;
        if (parent) transform.SetParent(parent);
    }
}
