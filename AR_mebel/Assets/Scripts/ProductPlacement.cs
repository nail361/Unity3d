﻿/*============================================================================== 
Copyright (c) 2018 PTC Inc. All Rights Reserved.

Vuforia is a trademark of PTC Inc., registered in the United States and other 
countries.   
==============================================================================*/

using UnityEngine;
using Vuforia;

public class ProductPlacement : MonoBehaviour
{

    #region PUBLIC_MEMBERS
    public bool IsPlaced { get; private set; }

    [Header("Placement Controls")]
    public GameObject m_TranslationIndicator;
    public GameObject m_RotationIndicator;
    private TouchHandler touchHandler;

    [Header("Placement Augmentation Size Range")]
    [Range(0.1f, 2.0f)]
    public float ProductSize = 0.65f;
    #endregion // PUBLIC_MEMBERS


    #region PRIVATE_MEMBERS

    float m_PlacementAugmentationScale;
    Vector3 ProductScaleVector;
    #endregion // PRIVATE_MEMBERS


    #region MONOBEHAVIOUR_METHODS
    void Start()
    {
                
        m_PlacementAugmentationScale = ProductSize;

        ProductScaleVector =
            new Vector3(m_PlacementAugmentationScale,
                        m_PlacementAugmentationScale,
                        m_PlacementAugmentationScale);

        gameObject.transform.localScale = ProductScaleVector;

        touchHandler = gameObject.GetComponent<TouchHandler>();
        touchHandler.m_AugmentationObject = gameObject.transform.GetChild(2);
        m_TranslationIndicator.transform.SetParent(touchHandler.m_AugmentationObject.transform);
        m_RotationIndicator.transform.SetParent(touchHandler.m_AugmentationObject.transform);
    }


    void Update()
    {
        m_RotationIndicator.SetActive(Input.touchCount == 2);
        m_TranslationIndicator.SetActive(
                TouchHandler.IsSingleFingerDragging || TouchHandler.IsSingleFingerStationary);
    }
    #endregion // MONOBEHAVIOUR_METHODS


    #region PUBLIC_METHODS
    public void Reset()
    {
        transform.position = Vector3.zero;
        transform.localEulerAngles = Vector3.zero;
        transform.localScale = ProductScaleVector;
    }

    public void SetProductAnchor(Transform transform)
    {
        if (transform)
        {
            IsPlaced = true;
            gameObject.transform.SetParent(transform);
            gameObject.transform.localPosition = Vector3.zero;
            UtilityHelper.RotateTowardCamera(gameObject);
        }
        else
        {
            IsPlaced = false;
            gameObject.transform.SetParent(null);
        }
    }
    #endregion // PUBLIC_METHODS
}