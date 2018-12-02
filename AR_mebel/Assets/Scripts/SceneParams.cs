using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SceneParams : MonoBehaviour {

    [SerializeField]
    private Light mainLight;

    [SerializeField]
    private GameObject[] changeModelBtns;

    private void Update()
    {
        changeModelBtns[0].SetActive(Models.ModelsCount > 1);
        changeModelBtns[1].SetActive(Models.ModelsCount > 1);

#if UNITY_ANDROID
        {
            if (Input.GetKey(KeyCode.Escape))
            {
                Application.Quit();
                return;
            }
        }
#endif
    }

    public void SwitchShadows()
    {
        mainLight.shadows = mainLight.shadows == LightShadows.None ? LightShadows.Soft : LightShadows.None;
    }
}
