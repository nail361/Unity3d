using UnityEngine;
using System.Collections.Generic;

public class Player : MonoBehaviour {

    private List<Transform> models;
    private Transform modelTransform;

    private int curModelIndex = 0;

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
    const float scaleRangeMax = 2.0f;

    Touch[] touches;
    bool isFirstFrameWithTwoTouches;
    float cachedTouchAngle;
    float cachedTouchDistance;
    float cachedAugmentationScale;
    Vector3 cachedAugmentationRotation;

    void Start()
    {
        models = new List<Transform>();
        for (int i = 0; i < gameObject.transform.childCount; i++) {
            Transform model = gameObject.transform.GetChild(i);
            if (model.tag != "Indicator")
                models.Add(model);
        }

        modelTransform = models[curModelIndex];

        m_TranslationIndicator.transform.SetParent(modelTransform, true);
        m_RotationIndicator.transform.SetParent(modelTransform, true);
        m_ScaleIndicator.transform.SetParent(modelTransform, true);

        Vector3 modelSize = modelTransform.gameObject.GetComponent<BoxCollider>().bounds.size;
        float maxSize = Mathf.Max(modelSize.x, modelSize.z) / 10;
        modelSize = new Vector3(maxSize + 0.1f, 1.0f, maxSize + 0.1f);
        m_TranslationIndicator.transform.localScale = modelSize;
        m_RotationIndicator.transform.localScale = modelSize;
        m_ScaleIndicator.transform.localScale = modelSize;

        cachedAugmentationScale = modelTransform.localScale.x;
        cachedAugmentationRotation = modelTransform.localEulerAngles;
    }

    void Update()
    {
        if (Input.touchCount > 0)
        {
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
                float currentTouchDistance = Vector2.Distance(touches[0].position, touches[1].position);
                float diff_y = touches[0].position.y - touches[1].position.y;
                float diff_x = touches[0].position.x - touches[1].position.x;
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

                if (touches.Length == 2)
                    modelTransform.localEulerAngles = cachedAugmentationRotation - new Vector3(0, angleDelta * 3f, 0);
                if (touches.Length > 2)
                    modelTransform.localScale = new Vector3(scaleAmountClamped, scaleAmountClamped, scaleAmountClamped);
            }
            else if (touches.Length < 2)
            {
                cachedAugmentationScale = modelTransform.localScale.x;
                cachedAugmentationRotation = modelTransform.localEulerAngles;
                isFirstFrameWithTwoTouches = true;
            }

        }

        UpdateIndicators();
    }

    private void UpdateIndicators()
    {
        m_TranslationIndicator.SetActive(dragging);
        m_RotationIndicator.SetActive(Input.touchCount == 2);
        m_ScaleIndicator.SetActive(Input.touchCount > 2);
    }

    public void NextModel()
    {
        curModelIndex++;

        if (curModelIndex >= models.Count)
            curModelIndex = 0;

        modelTransform = models[curModelIndex];
    }

    public void PrevModel()
    {
        curModelIndex--;

        if (curModelIndex < 0 )
            curModelIndex = models.Count - 1;

        modelTransform = models[curModelIndex];
    }

    public void ResetModel()
    {
        modelTransform.position = Vector3.zero;
        modelTransform.localScale = Vector3.one;
        UtilityHelper.RotateTowardCamera(modelTransform.gameObject);
    }
}
