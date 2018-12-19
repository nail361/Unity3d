using UnityEngine;

[ExecuteInEditMode]
public static class AttachBoxCollider {

    public static void Init(GameObject model) {
        BoxCollider collider = model.GetComponent<BoxCollider>();

        Bounds bounds = new Bounds(Vector3.zero, Vector3.zero);
        MeshFilter[] filters = model.GetComponentsInChildren<MeshFilter>();

        float summ = 0;
        int i = 0;

        foreach (MeshFilter f in filters)
        {
            bounds.Encapsulate(f.sharedMesh.bounds);
            summ += f.transform.position.y;
            i++;
        }

        collider.size = bounds.size;
        collider.center = new Vector3(0f, summ / i, 0f);
    }
}
