using UnityEngine;
using UnityEngine.UI;

public class StarType : MonoBehaviour {

    [SerializeField]
    private int type = -1;

	void Start () {
        if (type < 0)
        {
            GetComponent<Image>().sprite = GameManager.instance.star_sprites[GameManager.instance.cur_mode];
        }
        else
        {
            GetComponent<SpriteRenderer>().sprite = GameManager.instance.star_sprites[type];
        }
	}
}