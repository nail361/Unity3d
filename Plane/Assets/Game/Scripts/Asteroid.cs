using UnityEngine;
using System.Collections;

public class Asteroid : MonoBehaviour {

    private float maxScale = 0.0f;
    [SerializeField]
    private GameObject mesh;

    private float rotationSpeed = 180.0f;
    private float scaleSpeed = 2.0f;

	void Awake () {
        maxScale = Random.Range( 0.8f, 1.2f);
        transform.localScale = Vector3.zero;
        mesh.transform.localRotation = Random.rotation;
    }

    void Start()
    {
        StartCoroutine("Scaler");
    }

    IEnumerator Scaler()
    {
        Vector3 startScale = transform.localScale;
        Vector3 endScale = new Vector3(maxScale,maxScale,maxScale);
        float scale = 0.0f;

        while (startScale != endScale)
        {
            transform.localScale = Vector3.Lerp(startScale, endScale, scale);

            scale += scaleSpeed * Time.deltaTime;

            yield return null;
        }

        yield return null;
    }

	void Update () {
        mesh.transform.Rotate(Vector3.forward, rotationSpeed * Time.deltaTime, Space.Self);
	}
}
