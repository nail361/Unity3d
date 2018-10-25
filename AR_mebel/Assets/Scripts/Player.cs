using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour {

	private bool holding;
    public Transform modelTransform;
 
    void Start()
    {
        holding = false;
    }
 
    void Update()
    {
 
        if (holding)
        {
            Move();
        }

#if UNITY_EDITOR
        if (Input.GetMouseButtonDown(0))
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;
            if (Physics.Raycast(ray, out hit, 100f))
            {
                if (hit.transform == modelTransform)
                {
                    holding = true;
                }
            }
        }
        else if (Input.GetMouseButtonUp(0)) {
            holding = false;
        }
#else
        // One finger
        if (Input.touchCount == 1)
        {
         
            // Tap on Object
            if (Input.GetTouch(0).phase == TouchPhase.Began)
            {
                Ray ray = Camera.main.ScreenPointToRay(Input.GetTouch(0).position);
                RaycastHit hit;
 
                if (Physics.Raycast(ray, out hit, 100f))
                {
                    if (hit.transform == modelTransform)
                    {
                        holding = true;
                    }
                }
            }
 
            // Release
            if (Input.GetTouch(0).phase == TouchPhase.Ended)
            {
                holding = false;
            }
        }
#endif
    }

    void Move()
    {
        RaycastHit hit;
#if UNITY_EDITOR
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
#else
        Ray ray = Camera.main.ScreenPointToRay(Input.GetTouch(0).position);
#endif
        if (Physics.Raycast(ray, out hit, 100.0f))
        {
            transform.position = new Vector3(hit.point.x,
                                             transform.position.y,
                                             hit.point.z);
        }
    }
}
