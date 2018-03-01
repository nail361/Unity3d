using UnityEngine;
using System;

public class LevelChooser : MonoBehaviour {

    [SerializeField]
    private PlayerMenu player_menu;

    private RaycastHit2D hit;
    private Vector2 point;

    private bool isTouched = false;
    private bool isMoved = false;

    private int _active = 0;

    public void ActiveSwitch()
    {
        _active--;
    }

    public void DeActiveSwitch()
    {
        _active++;
    }

    void Update() {

        if (_active > 0) return;

#if UNITY_EDITOR
        if (Input.GetMouseButtonUp(0))
        {
            point = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            isTouched = true;
        }
#else
        if (Input.touchCount > 0)
        {
            if (Input.GetTouch(0).phase == TouchPhase.Moved)
            {
                isMoved = true;
            }
            else if (Input.GetTouch(0).phase == TouchPhase.Ended){
                if (isMoved)
                {
                    isMoved = false;
                    return;
                }
                point = Camera.main.ScreenToWorldPoint(Input.GetTouch(0).position);
                isTouched = true;
            }
        }
#endif

        if (!isTouched) return;

        isTouched = false;

        hit = Physics2D.Raycast(point, Vector2.zero, 50);

        if (hit.collider != null)
        {
            if (hit.collider.tag == "Level")
            {
                if (!hit.transform.GetChild(0).gameObject.activeSelf)
                {
                    return;
                }
                
                string[] arr = (hit.transform.parent.parent.name + '_' + hit.transform.parent.name + '_' + hit.transform.name).Split('_'); //stage_0_level_0_mode_0

                //потом выключить
                if (Int32.Parse(arr[3]) >= 5) {
					#if UNITY_ANDROID
                    AndroidNativeUtils.ShowMsg("Sorry, not in alpha version");
					#endif
                    return;
                }
                
                GameManager.instance.cur_stage = Int32.Parse(arr[1]);
                GameManager.instance.cur_level = Int32.Parse(arr[3]);
                GameManager.instance.cur_mode = Int32.Parse(arr[5]);

                player_menu.GoToLevel();
                
            }
            else if (hit.collider.tag == "Milk")
            {
                hit.collider.GetComponent<MilkBottle>().Take();
            }
            
        }

    }

}
