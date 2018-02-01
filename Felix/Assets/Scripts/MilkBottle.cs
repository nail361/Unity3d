using UnityEngine;
using System.Collections;

public class MilkBottle : MonoBehaviour {

    private Transform end;

    public void Take()
    {
        tag = "Untagged";

        end = GameObject.Find("bottle_end_dummy").transform;

        StartCoroutine(FlyToPlace());
    }

    IEnumerator FlyToPlace()
    {
        float scale = 0.0f;
        Vector3 start = transform.position;
        while (transform.position != end.position)
        {
            transform.position = Vector3.Lerp( start, end.position, scale );
            scale += Time.deltaTime * 2;

            if (scale >= 1) transform.position = end.position;

            yield return null;
        }

        GameManager.instance.TakeNewMilk();
        Destroy(gameObject);
        yield return null;
    }
    
}