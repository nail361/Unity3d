using UnityEngine;

public class Training : MonoBehaviour {

    [SerializeField]
    private GameObject[] Steps;
    private uint curStep;

    void Start () {
        curStep = 0;

        Camera.main.GetComponent<LevelChooser>().DeActiveSwitch();
        
        //для правильной работы, необходимо расположить тексты в texts по порядковому индексу = в LanguageManager
        foreach(GameObject step in Steps)
        {
            Transform texts = step.transform.Find("texts");
            for(var i=0; i < texts.childCount; i++)
            {
                if (i != (int)LanguageManager.curLanguage)
                    Destroy(texts.GetChild(i).gameObject);
            }
        }

        LoadStep();
	}

    void Update()
    {

#if UNITY_EDITOR
        if (Input.GetMouseButtonUp(0))
        {
            NextStep();
        }
#else
        if (Input.touchCount > 0)
        {
            if (Input.GetTouch(0).phase == TouchPhase.Ended){
                NextStep();
            }
        }
#endif
    }

    private void NextStep()
    {
        curStep++;

        if (curStep > Steps.Length - 1)
        {
            EndTraning();
        }
        else
        {
            LoadStep();
        }
    }

    private void LoadStep()
    {
        for(int i = 0; i < Steps.Length; i++)
        {
            Steps[i].SetActive(curStep == i);
        }
    }

    private void EndTraning()
    {
        Camera.main.GetComponent<LevelChooser>().ActiveSwitch();
        Destroy(gameObject);
    }

}