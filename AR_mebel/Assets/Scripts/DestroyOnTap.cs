using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DestroyOnTap : MonoBehaviour
{

    void Update()
    {
#if UNITY_EDITOR
        if (Input.GetMouseButtonDown(0))
        {
            Destroy(gameObject);
        }
#else
        if (Input.touchCount > 0)
        {
            Destroy(gameObject);
        }
#endif
    }
}
