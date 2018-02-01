using UnityEngine;
using System.Collections;

public class Swipe: MonoBehaviour {

	public float step;
	public float delta;
	public byte itemCount;

	private float[] positions;
	private float needPosX = 0f;
	private bool needSwipe = false;
	private RectTransform rectTransform;

	private static sbyte curIndex = 0;

	void Start(){

		positions = new float[itemCount];
		for( int i = 0; i < positions.Length; i++ ){
			positions[i] = step * i;
		}

		rectTransform = GetComponent<RectTransform>();
	}

	public static byte Index(){
		return (byte)curIndex;
	}

	public void StartDrag(){
		//return;
		needSwipe = false;
	}

	public void StopDrag(){
		//return;
		if ( positions[curIndex] - rectTransform.anchoredPosition.x > delta ){
			Right();
		}
		else if ( positions[curIndex] - rectTransform.anchoredPosition.x < delta ){
			Left();
		}

		needPosX = positions[curIndex];
		needSwipe = true;
	}

	private void Right(){
		curIndex++;
		if ( curIndex >= (sbyte)positions.Length ) curIndex = (sbyte)(positions.Length - 1);
	}

	private void Left(){
		curIndex--;
		if ( curIndex < 0 ) curIndex = 0;
	}

	public void SwipeRight(){
		Right();
		needPosX = positions[curIndex];
		needSwipe = true;
	}

	public void SwipeLeft(){
		Left();
		needPosX = positions[curIndex];
		needSwipe = true;
	}

	void Update(){
		if ( needSwipe ){
			rectTransform.anchoredPosition = Vector2.Lerp(rectTransform.anchoredPosition,
			                                              new Vector2( needPosX,rectTransform.anchoredPosition.y ),
			                                              Time.deltaTime * 10 );

			if ( Mathf.Abs( needPosX - rectTransform.anchoredPosition.x ) <= 0.1f ) needSwipe = false;
		}
	}

}