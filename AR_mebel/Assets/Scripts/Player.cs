using UnityEngine;
using System.Collections.Generic;

public class Player : MonoBehaviour {

    private Transform modelTransform;

    private int curModelIndex = 0;
    private int modelsCount = 0;

    private float dist;
    private bool dragging = false;
    private Vector3 offset;
    private Transform toDrag;

    [Header("Placement Controls")]
    [SerializeField]
    private GameObject m_TranslationIndicator;
    [SerializeField]
    private GameObject m_RotationIndicator;
    [SerializeField]
    private GameObject m_ScaleIndicator;

    const float scaleRangeMin = 0.1f;
    const float scaleRangeMax = 500.0f;

    Touch[] touches;
    bool isFirstFrameWithTwoTouches;
    float cachedTouchAngle;
    float cachedTouchDistance;
    float cachedAugmentationScale;
    Vector3 cachedAugmentationRotation;

    private bool scaleMode = false;

    void Start()
    {
        modelsCount = Models.ModelsCount;

        Invoke("ChangeModel", 3.0f);
    }

    void Update()
    {
        if (Input.touchCount > 0)
        {
            if ((Input.touches.Length == 2 || Input.touches.Length == 3) &&
                Input.touches.Length != touches.Length)
            {
                UpdateCashed();
            }

            touches = Input.touches;

            if (touches .Length != 1)
            {
                dragging = false;
            }
            else if (touches.Length == 1)
            {
                Vector3 pos = touches[0].position;
                Vector3 v3;

                if (touches[0].phase == TouchPhase.Began)
                {
                    RaycastHit hit;
                    Ray ray = Camera.main.ScreenPointToRay(pos);
                    if (Physics.Raycast(ray, out hit) && (hit.collider.name == modelTransform.name))
                    {
                        toDrag = hit.transform;
                        dist = hit.transform.position.z - Camera.main.transform.position.z;
                        v3 = new Vector3(pos.x, pos.y, dist);
                        v3 = Camera.main.ScreenToWorldPoint(v3);
                        offset = toDrag.position - v3;
                        dragging = true;
                    }
                }
                if (dragging && touches[0].phase == TouchPhase.Moved)
                {
                    v3 = new Vector3(Input.mousePosition.x, Input.mousePosition.y, dist);
                    v3 = Camera.main.ScreenToWorldPoint(v3);
                    toDrag.position = v3 + offset;
                }
                if (dragging && (touches[0].phase == TouchPhase.Ended || touches[0].phase == TouchPhase.Canceled))
                {
                    dragging = false;
                }
            }

            if (touches.Length > 1)
            {
                Touch touch1 = touches[touches.Length - 2];
                Touch touch2 = touches[touches.Length - 1];

                float currentTouchDistance = Vector2.Distance(touch1.position, touch2.position);
                float diff_y = touch1.position.y - touch2.position.y;
                float diff_x = touch1.position.x - touch2.position.x;
                float currentTouchAngle = Mathf.Atan2(diff_y, diff_x) * Mathf.Rad2Deg;
                

                if (isFirstFrameWithTwoTouches)
                {
                    cachedTouchDistance = currentTouchDistance;
                    cachedTouchAngle = currentTouchAngle;
                    isFirstFrameWithTwoTouches = false;
                }

                float angleDelta = currentTouchAngle - cachedTouchAngle;
                float scaleMultiplier = (currentTouchDistance / cachedTouchDistance);
                float scaleAmount = cachedAugmentationScale * scaleMultiplier;
                float scaleAmountClamped = Mathf.Clamp(scaleAmount, scaleRangeMin, scaleRangeMax);

                if (scaleMode)
                    modelTransform.localScale = new Vector3(scaleAmountClamped, scaleAmountClamped, scaleAmountClamped);
                else
                    modelTransform.localEulerAngles = cachedAugmentationRotation - new Vector3(0, angleDelta * 3f, 0);

            }
            else if (touches.Length < 2)
            {
                UpdateCashed();
            }

        }

        UpdateIndicators();
    }

    private void UpdateCashed()
    {
        cachedAugmentationScale = modelTransform.localScale.x;
        cachedAugmentationRotation = modelTransform.localEulerAngles;
        isFirstFrameWithTwoTouches = true;
    }

    private void UpdateIndicators()
    {
        m_TranslationIndicator.SetActive(dragging);
        m_RotationIndicator.SetActive(!scaleMode && Input.touchCount == 2);
        m_ScaleIndicator.SetActive(scaleMode && Input.touchCount == 2);
    }

    private void SetIndicatorsSizeAndPos()
    {
        Vector3 modelSize = modelTransform.gameObject.GetComponent<BoxCollider>().bounds.size;
        float maxSize = Mathf.Max(modelSize.x, modelSize.z) / 10;
        modelSize = new Vector3(maxSize + 0.1f, 1.0f, maxSize + 0.1f);
        m_TranslationIndicator.transform.localScale = modelSize;
        m_RotationIndicator.transform.localScale = modelSize;
        m_ScaleIndicator.transform.localScale = modelSize;

        m_TranslationIndicator.transform.localPosition = new Vector3(0, 0, modelSize.y / 2 * -1);
        m_RotationIndicator.transform.localPosition = new Vector3(0, 0, modelSize.y / 2 * -1);
        m_ScaleIndicator.transform.localPosition = new Vector3(0, 0, modelSize.y / 2 * -1);
    }

    private void SetIndicatorsParent()
    {
        m_TranslationIndicator.transform.SetParent(modelTransform, true);
        m_RotationIndicator.transform.SetParent(modelTransform, true);
        m_ScaleIndicator.transform.SetParent(modelTransform, true);
    }

    private void ChangeModel()
    {
        modelTransform = Models.GetModel(curModelIndex).transform;
        Models.ShowModel(curModelIndex);

        SetIndicatorsParent();
        SetIndicatorsSizeAndPos();
        UpdateCashed();
    }

    public void NextModel()
    {
        Models.HideModel(curModelIndex);
        curModelIndex++;

        if (curModelIndex >= modelsCount)
            curModelIndex = 0;

        ChangeModel();
    }

    public void PrevModel()
    {
        Models.HideModel(curModelIndex);
        curModelIndex--;

        if (curModelIndex < 0 )
            curModelIndex = modelsCount - 1;

        ChangeModel();
    }

    public void ChangeTouchesMode()
    {
        scaleMode = !scaleMode;
        UpdateCashed();
    }

    public void ResetModel()
    {
        modelTransform.position = Vector3.zero;
        modelTransform.localScale = Vector3.one;
        UtilityHelper.RotateTowardCamera(modelTransform.gameObject);
    }
}
