using UnityEngine;
using System.Collections;

public class PlayerManager : MonoBehaviour {

	public GameObject Magnet;
	public GameObject Shield;

	public bool ShieldON = false; //дополнительная проверка что щит

	private float moveSpeed = 1;
	private float turbulence = 15;

	private GameObject planeMesh;
	private int step = 0;
	private Vector3 newPos;

	private float m_delta = 12f;

	private sbyte invert = 1;

	private Vector2[,] setka = new Vector2[3,3]{
		{new Vector2(-1,1),new Vector2(0,1),new Vector2(1,1)},
		{new Vector2(-1,0),new Vector2(0,0),new Vector2(1,0)},
		{new Vector2(-1,-1),new Vector2(0,-1),new Vector2(1,-1)}
	};
	private Vector2 index = new Vector2( 1, 1 );

	void Start () {
		planeMesh = Instantiate( Resources.Load("Plans/Plane_"+GameManager.instance.PlayerPlane.ToString(), typeof(GameObject)), Vector3.zero, Quaternion.identity ) as GameObject;
		planeMesh.transform.parent = transform;
		planeMesh.name = "mesh";
		planeMesh.tag = "Player";
		newPos = transform.position;

		moveSpeed += GameManager.instance.GetPlaneControl( GameManager.instance.PlayerPlane );

		step = LevelManager.instance.Step;
	}
//#if UNITY_EDITOR
	private Vector2 delta = Vector2.zero;
	private Vector2 lastPos = Vector2.zero;
//#endif
	private float angle = 0;
	private sbyte znak = 1;
	void Update () {

		if ( LevelManager.instance.Paused ){
			lastPos = new Vector2( Input.mousePosition.x, Input.mousePosition.y );
			return;
		}

		if ( angle >= turbulence ) znak = -1;
		else if ( angle <= -turbulence ) znak = 1;
		
		angle += turbulence*Time.deltaTime*turbulence/6*znak;
		planeMesh.transform.rotation = Quaternion.AngleAxis( angle, Vector3.forward );

//#if UNITY_EDITOR
		if ( Input.GetMouseButtonDown(0) )
		{
			lastPos = new Vector2( Input.mousePosition.x, Input.mousePosition.y );
		}
		else if ( Input.GetMouseButtonUp(0) )
		{
			delta = new Vector2( Input.mousePosition.x, Input.mousePosition.y ) - lastPos;
			lastPos = new Vector2( Input.mousePosition.x, Input.mousePosition.y );

			if ( Mathf.Abs(delta.x) <= m_delta && Mathf.Abs(delta.y) <= m_delta ) return;

			LevelManager.instance.Swipe();

			CalculateMovement( delta );
		}
//#else
		/*
		if (Input.touchCount > 0 && Input.GetTouch(0).phase == TouchPhase.Moved) {
			Vector2 touchDeltaPosition = Input.GetTouch(0).deltaPosition;
			CalculateMovement( touchDeltaPosition );
		}
		*/
//#endif

		if (transform.position != newPos){
			transform.position = Vector3.Lerp( transform.position, newPos, moveSpeed * Time.deltaTime );
		}

	}

	private void CalculateMovement( Vector2 delta ){
		delta.x *= invert;
		delta.y *= invert;
		
		if ( Mathf.Abs(delta.x) >= Mathf.Abs( delta.y ) ) delta.y = 0;
		else delta.x = 0;
		
		delta = delta.normalized;
		
		if ( delta.x != 0 ){
			if (index.x + delta.x < 3 && index.x + delta.x >= 0){
				index.x += delta.x;
				Vector2 pos = setka[(int)index.y,(int)index.x];
				newPos = new Vector3( pos.x*step, -pos.y*step, 0 );
			}
		}
		else if ( delta.y != 0 ){
			if (index.y + delta.y < 3 && index.y + delta.y >= 0){
				index.y += delta.y;
				Vector2 pos = setka[(int)index.y,(int)index.x];
				newPos = new Vector3( pos.x*step, -pos.y*step, 0 );
			}
		}
	}

	public void MagnetOnOff( bool trigger ){
		Magnet.SetActive( trigger );
	}

	public void ShieldOnOff( bool trigger ){
		ShieldON = trigger;
		Shield.SetActive( trigger );
	}

	public void InvertOnOff( bool trigger ){
		if ( trigger ) invert = -1;
		else invert = 1;
	}

}
