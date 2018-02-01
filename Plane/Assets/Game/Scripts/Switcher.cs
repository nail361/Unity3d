using UnityEngine;
using System.Collections;

public class Switcher : MonoBehaviour {

	[SerializeField]
	private MonoBehaviour Script;
		
	[SerializeField]
	private float SwitchTime;

	private float CurTime;

	private float _timeAtLastFrame = 0F;
	private float _timeAtCurrentFrame = 0F;
	private float deltaTime = 0F;

	void Start(){
		CurTime = SwitchTime;
	}

	void Update () {

		_timeAtCurrentFrame = Time.realtimeSinceStartup;
		deltaTime = _timeAtCurrentFrame - _timeAtLastFrame;
		_timeAtLastFrame = _timeAtCurrentFrame; 

		CurTime -= deltaTime;

		if ( CurTime <= 0 ){
			Script.enabled = !Script.enabled;

			CurTime = SwitchTime;
		}

	}

}
