using UnityEngine;
using System.Collections;

public class CameraFollow : MonoBehaviour
{
	public Transform target;
	public float distance = 10.0f;
	public float height = 5.0f;
	public float moveSpeed;	
	
	void LateUpdate ()
	{
		if (!target)
			return;

		if ( transform.position + new Vector3(0,height,-distance) != target.position ){

			Vector3 newPos = transform.position - new Vector3(0,height,-distance);

			newPos = Vector3.Lerp( newPos, target.position, moveSpeed * Time.deltaTime );

			newPos -= Vector3.forward * distance;
			newPos += Vector3.up * height;

			transform.position = newPos;
			transform.LookAt(target.position/2);
		}
		
	}
}
