using UnityEngine;
using System;
using System.Collections.Generic;
using System.Collections;

public class LevelManager : MonoBehaviour
{

    [SerializeField]
    private Sprite[] mode_sprites_on;
    [SerializeField]
    private Sprite[] mode_sprites_off;

    [SerializeField]
    private GameObject bottle_of_milk;

    [SerializeField]
    private GameObject[] stages;

    void Start()
    {
        StartCoroutine("GoToCurStage");
    }

    private void FillLevels()
    {
        GameObject mode;
        int stage = GameManager.instance.cur_stage;

        for (int level = 0; level < GameManager.instance.levels[stage].Count; level++)
        {

            for (uint mode_number = 0; mode_number < 3; mode_number++)
            {
                mode = GameObject.Find("Stage_" + stage.ToString() + "/" + "Level_" + level.ToString() + "/mode_" + mode_number.ToString());
                if (mode == null) continue;

                if (mode_number == 0)
                {
                    if (level == 0)
                    {
                        mode.GetComponent<SpriteRenderer>().sprite = mode_sprites_on[stage];
                    }
                    else
                    {
                        if (GameManager.instance.levels[stage][level - 1][mode_number] == 0)
                            mode.GetComponent<SpriteRenderer>().sprite = mode_sprites_off[stage];
                        else
                            mode.GetComponent<SpriteRenderer>().sprite = mode_sprites_on[stage];
                    }
                }
                else
                {
                    if (GameManager.instance.levels[stage][level][mode_number - 1] == 0)
                        mode.GetComponent<SpriteRenderer>().sprite = mode_sprites_off[stage];
                    else
                        mode.GetComponent<SpriteRenderer>().sprite = mode_sprites_on[stage];
                }

                int new_stars = 0;
                if (stage == GameManager.instance.cur_stage & level == GameManager.instance.cur_level & mode_number == GameManager.instance.cur_mode & PlayerPrefs.HasKey("level_complete"))
                {
                    new_stars = PlayerPrefs.GetInt("level_complete");
                    PlayerPrefs.DeleteKey("level_complete");
                }

                uint stars = GameManager.instance.levels[stage][level][mode_number];
                for (uint star = 1; star <= 3; star++)
                {
                    GameObject star_object = mode.transform.Find("star_" + star.ToString()).gameObject;
                    if (mode.GetComponent<SpriteRenderer>().sprite.name == mode_sprites_off[stage].name) star_object.SetActive(false);
                    else
                    {
                        star_object.SetActive(true);

                        if (new_stars >= star)
                        {
                            star_object.GetComponent<SpriteRenderer>().color = Color.grey;
                            StartCoroutine(GameManager.instance.waitThenCallback(star * 0.5f, new Action(delegate ()
                            {
                                star_object.GetComponent<Animation>().Play("new_star");
                                star_object.GetComponent<SpriteRenderer>().color = Color.white;
                            })));
                        }
                        else
                            star_object.GetComponent<SpriteRenderer>().color = stars >= star ? Color.white : Color.grey;
                    }
                }
            }
        }

    }

    private void FillNewMilk()
    {
        for (uint i = 0; i < GameManager.instance.new_milk.GetConverted<uint>(); i++)
        {
            if (GameManager.instance.milk_pos[i] > 0) CreateMilkBottle(GameManager.instance.milk_pos[i] - 1);
            else
                AddNewMilk(i);
        }
    }

    public IEnumerator GoToCurStage()
    {
        GameObject.Find("blank").GetComponent<Animation>().Play("blank");

        yield return new WaitForSeconds(0.5f);

        for (int i = 0; i < stages.Length; i++)
            stages[i].SetActive(GameManager.instance.cur_stage == i);

        FillLevels();
        FillNewMilk();

        yield return null;
    }

    public void AddNewMilk(uint milk_index)
    {
        List<uint> levels = new List<uint>();

        for (uint i = 0; i < GameManager.instance.levels[GameManager.instance.cur_stage].Count; i++)
        {
            bool empty = true;

            foreach (uint j in GameManager.instance.milk_pos)
            {
                if (j - 1 == i) empty = false;
            }
            
            if (empty) levels.Add(i);
        }

        uint level = levels[UnityEngine.Random.Range(0, levels.Count)];

        GameManager.instance.milk_pos[milk_index] = level + 1;

        CreateMilkBottle(level);
    }

    private void CreateMilkBottle(uint level)
    {
        Transform levelGO = GameObject.Find("Stage_" + GameManager.instance.cur_stage.ToString() + "/" + "Level_" + level.ToString()).transform;
        Vector3 bottle_pos = levelGO.Find("milk_place").localPosition;
        GameObject bottle = Instantiate(bottle_of_milk, levelGO.transform, false) as GameObject;
        bottle.transform.localPosition = new Vector3(bottle_pos.x + UnityEngine.Random.Range(-0.4f, 0.1f), bottle_pos.y, bottle_pos.z);
        bottle.transform.localScale = new Vector3(0.6f,0.6f,0.6f);
    }

}