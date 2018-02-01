using UnityEngine;

public class CloudsSwitcher : MonoBehaviour {

    public GameObject Clouds;

    void OnEnable() {
        Clouds.SetActive(true);
    }

    void OnDisable() {
        if (Clouds) Clouds.SetActive(false);
    }
}