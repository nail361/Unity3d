using UnityEngine;
using System.Collections;

public class Cloud : MonoBehaviour {

	[HideInInspector]
	public float LifeTime = 0;
	[HideInInspector]
	public float Turbulence = 0;
	[HideInInspector]
	public float Speed = 0;

	private float curLifeTime;
	private Quaternion newRotation;

	public void SetParams( float _lifeTime, float _turbulence, float _speed, Color32 _color ){

		LifeTime = _lifeTime;
		Turbulence = _turbulence;
		Speed = _speed;
		GetComponent<ParticleSystem>().startColor = _color;

		GetComponent<ParticleSystem>().startLifetime = LifeTime;
	}

	void OnEnable(){
		curLifeTime = LifeTime;
		newRotation = transform.localRotation;
	}

	void Update() {

		if ( curLifeTime > 0 ){

			//турбуленцию добавить
			if ( Turbulence != 0 ){

				sbyte znak = Random.value > 0.5 ? (sbyte)1 : (sbyte)-1 ;
				
				float randomX = Random.Range( Turbulence/2 , Turbulence ) * znak;
				
				znak = Random.value > 0.5 ? (sbyte)1 : (sbyte)-1 ;
				
				float randomY = Random.Range( Turbulence/2 , Turbulence ) * znak;

				if ( newRotation == transform.localRotation ) newRotation = Quaternion.LookRotation( new Vector3(randomX,
				                                                                                            	 randomY,
				                                                                                                 transform.position.z - 1 )  );

				transform.localRotation = Quaternion.Lerp( transform.localRotation, newRotation, Time.deltaTime );

			}

			transform.Translate(0, 0, Speed * Time.deltaTime * 10 );

			curLifeTime -= Time.deltaTime;
		}
		else gameObject.SetActive(false);

	}
}
