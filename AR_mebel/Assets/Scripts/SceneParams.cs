using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SceneParams : MonoBehaviour {

    [SerializeField]
    private Light mainLight;

    [SerializeField]
    private GameObject[] changeModelBtns;

    private void Awake()
    {
        if (Models.ModelsCount < 2) {
            changeModelBtns[0].SetActive(false);
            changeModelBtns[1].SetActive(false);
        }
    }

    public void SwitchShadows()
    {
        mainLight.shadows = mainLight.shadows == LightShadows.None ? LightShadows.Soft : LightShadows.None;
    }
}
