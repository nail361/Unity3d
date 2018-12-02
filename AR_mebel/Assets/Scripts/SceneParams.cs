﻿using UnityEngine.UI;
using UnityEngine;

public class SceneParams : MonoBehaviour {

    [SerializeField]
    private Light mainLight;

    [Header("Pitch btn")]
    [SerializeField]
    private Image pitchImage;
    [SerializeField]
    private Sprite rotateSprite;
    [SerializeField]
    private Sprite scaleSprite;

    private bool rotate = true;

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

    public void SwitchPitchImage()
    {
        rotate = !rotate;
        pitchImage.sprite = rotate ? rotateSprite : scaleSprite;
    }

    public void SwitchShadows()
    {
        mainLight.shadows = mainLight.shadows == LightShadows.None ? LightShadows.Soft : LightShadows.None;
    }
}
