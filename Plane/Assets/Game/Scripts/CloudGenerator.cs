using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class CloudGenerator : MonoBehaviour {

	[SerializeField]
	private float Radius;
	[SerializeField]
	private GameObject cloud;
	[SerializeField]
	private float respawnSpeed;
	[SerializeField]
	private int maxClouds;

	private List<GameObject> clouds;

	[SerializeField]
	private float cloudsLifeTime;
	[SerializeField]
	private float cloudsTurbulence;
	[SerializeField]
	private float cloudsSpeed;
	[SerializeField]
	private Color32 cloudsColor;

	private float respawn = 0;

	void Start () {
		clouds = new List<GameObject>();
		/*
		for( int i = 0; i < maxClouds; i++){
			clouds.Add((GameObject)Instantiate(cloud,
			                                   new Vector3(Random.Range(-Radius,Radius),Random.Range(-Radius,Radius),transform.position.z),
			                                   Quaternion.identity));
			clouds[i].name = "cloud_" + i.ToString();

			//настроить облако

		}
		*/
	
	}
	
	void Update () {

		respawn += Time.deltaTime;

		if ( respawn >= respawnSpeed ){

			int indexInactive = -1;

			for( int i = 0; i < clouds.Count; i++ ){
				if ( !clouds[i].activeSelf ){
					indexInactive = i;
					break;
				}
			}

			if ( indexInactive >= 0 ){
				clouds[indexInactive].transform.rotation = transform.rotation;
				clouds[indexInactive].transform.position = new Vector3( Random.Range(-Radius,Radius),Random.Range(-Radius,Radius), transform.position.z  );
				clouds[indexInactive].SetActive(true);
			}
			else if ( clouds.Count < maxClouds ){
				clouds.Add((GameObject)Instantiate(cloud,
				                                   new Vector3(Random.Range(-Radius,Radius),Random.Range(-Radius,Radius),transform.position.z),
				                                   transform.rotation));
				clouds[clouds.Count-1].transform.parent = transform;
				clouds[clouds.Count-1].name = "cloud_" + (clouds.Count-1).ToString();
				clouds[clouds.Count-1].GetComponent<Cloud>().SetParams( cloudsLifeTime, cloudsTurbulence, cloudsSpeed, cloudsColor );
				clouds[clouds.Count-1].SetActive(true);
			}

			respawn = 0;
		}
		/*
		cloudsSpeed += 0.1f * Time.deltaTime;
		cloudsLifeTime += 0.1f * Time.deltaTime;
		respawnSpeed += 0.0005f * Time.deltaTime;
		*/

	}
}
