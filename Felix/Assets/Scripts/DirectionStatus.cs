using UnityEngine;
using System.Collections;

public class DirectionStatus : MonoBehaviour
{

    private SpriteRenderer sr;

    void Start()
    {
        sr = GetComponent<SpriteRenderer>();
        sr.color = new Color(1.0f, 1.0f, 1.0f, 0.0f);
    }

    public void Show(Vector3 move_dir, Vector3 move_end)
    {
        StopCoroutine("Dissapearing");
        Vector3 new_rotation = transform.localRotation.eulerAngles;
        if (move_dir.x > 0) new_rotation.z = 0;
        else if (move_dir.x < 0) new_rotation.z = 180;
        else if (move_dir.y > 0) new_rotation.z = 90;
        else new_rotation.z = 270;
        transform.localRotation = Quaternion.Euler(new_rotation.x, new_rotation.y, new_rotation.z);
        transform.position = move_end;
        sr.color = new Color(1.0f, 1.0f, 1.0f, 0.5f);
    }

    public void Hide()
    {
        StartCoroutine("Dissapearing");
    }

    private IEnumerator Dissapearing()
    {
        while (sr.color.a > 0.0f)
        {
            sr.color = new Color(1.0f, 1.0f, 1.0f, sr.color.a - 0.5f * Time.deltaTime);
            yield return null;
        }
    }

    public bool isShow()
    {
        return sr.color.a != 0;
    }

}
