using UnityEngine;

[ExecuteInEditMode]
[RequireComponent(typeof(BoxCollider))]
public class AttachBoxCollider : MonoBehaviour {

    void Start() {
        BoxCollider collider = gameObject.GetComponent<BoxCollider>();

        Bounds bounds = new Bounds(Vector3.zero, Vector3.zero);
        MeshFilter[] filters = gameObject.GetComponentsInChildren<MeshFilter>();
        foreach (MeshFilter f in filters)
        {
            bounds.Encapsulate(f.sharedMesh.bounds);
            bounds.center = Vector3.zero;

        }
        collider.size = bounds.size;
        collider.center = bounds.center;
    }
}
