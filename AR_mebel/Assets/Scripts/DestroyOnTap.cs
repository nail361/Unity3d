using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DestroyOnTap : MonoBehaviour {

	void Update () {
        if (Input.touchCount > 0)
        {
            Destroy(gameObject);
        }
	}
}
