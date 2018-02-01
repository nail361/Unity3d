using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class LoadingImg : MonoBehaviour {

    public void Start()
    {
        DontDestroyOnLoad(gameObject.transform.parent);

        GameManager.instance.loading_img = this;
        gameObject.SetActive(false);
    }

	public void Activate()
    {
        gameObject.SetActive(true);
        gameObject.GetComponent<Image>().material.SetFloat("_Cutoff", 0);
        StartCoroutine("CloseCircle");
    }

    IEnumerator CloseCircle()
    {
        float cutoff = 1;
        while (cutoff > 0)
        {
            cutoff -= 0.02f;
            gameObject.GetComponent<Image>().material.SetFloat("_Cutoff", cutoff);
            yield return true;
        }
    }

    public void DeActivate() {
        gameObject.SetActive(true);
        gameObject.GetComponent<Image>().material.SetFloat("_Cutoff", 1);
        StartCoroutine("OpenCircle");
    }

    IEnumerator OpenCircle()
    {
        float cutoff = 0;
        while (cutoff < 1)
        {
            cutoff += 0.02f;
            gameObject.GetComponent<Image>().material.SetFloat("_Cutoff", cutoff);
            yield return true;
        }

        gameObject.SetActive(false);
    }
}
