using UnityEngine;
using System.Collections.Generic;

public class Player : MonoBehaviour {

    private Transform modelTransform;

    private int curModelIndex = 0;

    private float dist;
    private bool dragging = false;
    private Vector3 oldPos;
    private Vector3 newPos;
    private Vector3 offset;
    private Transform toDrag;

    [Header("Placement Controls")]
    [SerializeField]
    private GameObject m_TranslationIndicator;
    [SerializeField]
    private GameObject m_RotationIndicator;
    [SerializeField]
    private GameObject m_ScaleIndicator;

    private SceneParams sceneParams;

    const float scaleRangeMin = 0.1f;
    const float scaleRangeMax = 500.0f;

    Touch[] touches;
    bool isFirstFrameWithTwoTouches;
    Vector3 intiScale;
    float cachedTouchAngle;
    float cachedTouchDistance;
    float cachedAugmentationScale;
    Vector3 cachedAugmentationRotation;

    private bool scaleMode = false;

    void Start()
    {
        sceneParams = FindObjectOfType<SceneParams>();
        Invoke("ChangeModel", 3.0f);
    }
    
    void Update()
    {
#if UNITY_EDITOR
        if (Input.GetMouseButtonDown(0))
        {

            Vector3 pos = Input.mousePosition;
            RaycastHit hit;
            Ray ray = Camera.main.ScreenPointToRay(pos);
            if (Physics.Raycast(ray, out hit) && (hit.collider.name == modelTransform.name))
            {
                toDrag = hit.transform;
                dist = Camera.main.WorldToScreenPoint(toDrag.position).z;
                newPos = new Vector3(pos.x, pos.y, dist);
                newPos = Camera.main.ScreenToWorldPoint(newPos);
                oldPos = newPos;
                offset = toDrag.position - newPos;
                dragging = true;
            }
        }
        if (dragging && Input.GetMouseButton(0))
        {
            newPos = new Vector3(Input.mousePosition.x, Input.mousePosition.y, dist);
            newPos = Camera.main.ScreenToWorldPoint(newPos);
            newPos = newPos - oldPos;// - offset;

            //offset = Vector3.zero;

            oldPos = Camera.main.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, dist));

            toDrag.position += new Vector3(newPos.x, 0, newPos.z);
        }
        if (dragging && Input.GetMouseButtonUp(0))
        {
            dragging = false;
        }
#else
        if (Input.touchCount > 0)
        {
            if ((Input.touches.Length == 2 || Input.touches.Length == 3) &&
                Input.touches.Length != touches.Length)
            {
                UpdateCashed();
            }

            touches = Input.touches;

            if (touches[0].phase == TouchPhase.Ended)
            {
                UpdateCashed();
            }

            if (touches.Length != 1)
            {
                dragging = false;
            }
            else if (touches.Length == 1)
            {
                Vector3 pos = touches[0].position;

                if (touches[0].phase == TouchPhase.Began)
                {
                    RaycastHit hit;
                    Ray ray = Camera.main.ScreenPointToRay(pos);
                    if (Physics.Raycast(ray, out hit) && (hit.collider.name == modelTransform.name))
                    {
                        toDrag = hit.transform;
                        dist = Camera.main.WorldToScreenPoint(toDrag.position).z;
                        newPos = new Vector3(pos.x, pos.y, dist);
                        newPos = Camera.main.ScreenToWorldPoint(newPos);
                        oldPos = newPos;
                        offset = toDrag.position - newPos;
                        dragging = true;
                    }
                }
                if (dragging && touches[0].phase == TouchPhase.Moved)
                {
                    newPos = new Vector3(touches[0].position.x, touches[0].position.y, dist);
                    newPos = Camera.main.ScreenToWorldPoint(newPos);
                    newPos = newPos - oldPos;

                    oldPos = Camera.main.ScreenToWorldPoint(new Vector3(touches[0].position.x, touches[0].position.y, dist));

                    toDrag.position += new Vector3(newPos.x, 0, newPos.z);
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
#endif

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

    public void SelectModel(int modelID)
    {
        Models._instance.HideModel(curModelIndex);
        curModelIndex = modelID;
        ChangeModel();
    }

    private void ChangeModel()
    {
        modelTransform = Models._instance.GetModel(curModelIndex).transform;
        Models._instance.ShowModel(curModelIndex);

        intiScale = modelTransform.localScale;

        SetIndicatorsParent();
        SetIndicatorsSizeAndPos();
        UpdateCashed();
        sceneParams.SetModelInfo(Models._instance.GetModelInfo(curModelIndex));
        sceneParams.SwitchAnimationBtn(modelTransform.GetComponent<Animation>());
    }

    public void NextModel()
    {
        Models._instance.HideModel(curModelIndex);
        curModelIndex++;

        if (curModelIndex >= Models._instance.ModelsCount)
            curModelIndex = 0;

        ListItem.SetSelectedID(curModelIndex);

        ChangeModel();
    }

    public void PrevModel()
    {
        Models._instance.HideModel(curModelIndex);
        curModelIndex--;

        if (curModelIndex < 0 )
            curModelIndex = Models._instance.ModelsCount - 1;

        ListItem.SetSelectedID(curModelIndex);

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
        modelTransform.localScale = intiScale;
        UtilityHelper.RotateTowardCamera(modelTransform.gameObject);
    }

    public void PlayModelAnimation()
    {
        modelTransform.SendMessage("PlayAnim", SendMessageOptions.DontRequireReceiver);
    }
}
