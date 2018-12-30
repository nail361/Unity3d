using UnityEngine.UI;
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

    [SerializeField]
    private Text priceField;
    [SerializeField]
    private Text nameField;

    private bool rotate = true;

    [SerializeField]
    private GameObject[] changeModelBtns;

    [SerializeField]
    private Button animationBtn;

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

    public void SetModelInfo(ModelInfo info)
    {
        priceField.text = info.price;
        nameField.text = info.name;
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

    public void SwitchAnimationBtn(Animation anim)
    {
        if (anim != null && anim.GetClipCount() > 0)
        {
            animationBtn.interactable = true;
        }
        else animationBtn.interactable = false;
    }
}
