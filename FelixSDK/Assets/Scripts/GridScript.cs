using UnityEngine;
using UnityEngine.UI;

public class GridScript : MonoBehaviour {

    [SerializeField]
    private Text index_text;

	void Start () {
        index_text.text = "(" + transform.position.x + "," + -transform.position.y + ")";
        name = "grid_" + index_text.text;
	}
	
}