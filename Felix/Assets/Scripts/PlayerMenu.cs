using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

public class PlayerMenu : MonoBehaviour {
    
    [SerializeField]
    private GameObject levelWindowPref;
    [SerializeField]
    private GameObject trainingPref;

    private SpriteAnimator sa;

    private float cur_level;

    private float speed = 5;

    private MenuCamera m_camera;

    //Установка игрока на место последней игры
    void Start () {
        GameManager.instance.pause = false;

        //значит первый раз запустили игру
        if (GameManager.instance.cur_level < 0)
        {
            GameManager.instance.cur_level = 0;
            Instantiate(trainingPref, Vector3.zero, Quaternion.identity);
        }
        else
        {
            transform.position = GameObject.Find("Stage_" + GameManager.instance.cur_stage.ToString() + "/Level_" + GameManager.instance.cur_level.ToString()).
                transform.FindChild("point_1").position;
        }

        cur_level = GameManager.instance.cur_level;

        sa = GetComponent<SpriteAnimator>();

        m_camera = Camera.main.GetComponent<MenuCamera>();

        if (GameManager.LOADED_FROM_MENU)
            m_camera.FindPlayer(transform.position.y, 0.5f, 1.0f);
        else
        {
            GameObject.Find("HUD/Clouds").GetComponent<Destroyer>().DestroyNow();
            m_camera.FindPlayer(transform.position.y);
        }
    }

    public void GoToLevel()
    {
        if (cur_level == GameManager.instance.cur_level)
        {
            MovingEnd();
            return;
        }

        StopCoroutine("Moving_" + GameManager.instance.cur_stage.ToString());
        m_camera.FindPlayer(transform.position.y);
        StartCoroutine("Moving_" + GameManager.instance.cur_stage.ToString());
    }

    IEnumerator Moving_0()
    {
        Transform point;
        GameObject cur_levelGO = GameObject.Find("Stage_" + GameManager.instance.cur_stage.ToString() + "/Level_" + cur_level.ToString());

        float scale;
        Vector3 start;

        sa.Play();

        do
        {
            if (cur_level < GameManager.instance.cur_level) {
                cur_level++;
                cur_levelGO = GameObject.Find("Stage_" + GameManager.instance.cur_stage.ToString() + "/Level_" + cur_level.ToString());
                m_camera.FindPlayer(transform.position.y);
            }

            point = cur_levelGO.transform.FindChild("point_2");

            scale = 0.0f;
            start = transform.position;

            sa.FlipX(true);
            while (transform.position != point.position)
            {
                transform.position = Vector3.Lerp(start, point.position, scale);
                scale += Time.deltaTime * 2;

                if (scale >= 1) transform.position = point.position;
                yield return null;
            }

            if (cur_level > GameManager.instance.cur_level)
            {
                cur_level--;
                cur_levelGO = GameObject.Find("Stage_" + GameManager.instance.cur_stage.ToString() + "/Level_" + cur_level.ToString());
                m_camera.FindPlayer(transform.position.y);
            }

            point = cur_levelGO.transform.FindChild("point_1");

            scale = 0.0f;
            start = transform.position;

            sa.FlipX(false);
            while (transform.position != point.position)
            {
                transform.position = Vector3.Lerp(start, point.position, scale);
                scale += Time.deltaTime * 2;

                if (scale >= 1) transform.position = point.position;
                yield return null;
            }

        } while (cur_level != GameManager.instance.cur_level);

        sa.Stop();
        sa.GoToStart();

        MovingEnd();
    }

    private void MovingEnd()
    {
        Instantiate(levelWindowPref, Vector3.zero, Quaternion.identity);
    }

}