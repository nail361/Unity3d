using UnityEngine;
using System.Collections;

public class Rotator : MonoBehaviour {

	[SerializeField]
	private float Speed;
	[SerializeField]
	private Vector3 Direction;

	void Update () {
		transform.rotation = Quaternion.Euler( new Vector3(transform.rotation.eulerAngles.x + Time.deltaTime * Speed * Direction.x,
		                                             transform.rotation.eulerAngles.y + Time.deltaTime * Speed * Direction.y,
		                                             transform.rotation.eulerAngles.z + Time.deltaTime * Speed * Direction.z) );
	}
}
