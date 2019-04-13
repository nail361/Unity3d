using UnityEngine;
using UnityEditor;

public class RecalcualteMesh : Editor {

    static bool recalculateNormals = false;
    static Vector3[] baseVertices = null;

    public static void ScaleModel(GameObject model, float scalePercent)
    {
        MeshFilter[] filters = model.GetComponentsInChildren<MeshFilter>();

        foreach (MeshFilter filter in filters) {

            Mesh mesh = filter.sharedMesh;

            baseVertices = mesh.vertices;

            Vector3[] vertices = new Vector3[baseVertices.Length];

            for (var i = 0; i < vertices.Length; i++)
            {
                Vector3 vertex = baseVertices[i];
                vertex = new Vector3(
                    vertex.x - vertex.x / 100 * scalePercent,
                    vertex.y - vertex.y / 100 * scalePercent,
                    vertex.z - vertex.z / 100 * scalePercent
                    );

                vertices[i] = vertex;
            }

            mesh.vertices = vertices;

            if (recalculateNormals)
                mesh.RecalculateNormals();

            mesh.RecalculateBounds();

            baseVertices = null;
        }
    }
}