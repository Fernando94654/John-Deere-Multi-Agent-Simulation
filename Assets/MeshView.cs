using UnityEngine;

public class MeshVertexViewer : MonoBehaviour
{
    void Start()
    {
        MeshFilter meshFilter = GetComponent<MeshFilter>();

        if (meshFilter == null)
        {
            Debug.LogError("Este objeto no tiene Mesh Filter.");
            return;
        }

        Mesh mesh = meshFilter.sharedMesh;

        if (mesh == null)
        {
            Debug.LogError("No se encontró ningún Mesh.");
            return;
        }

        Debug.Log("Mesh: " + mesh.name);
        Debug.Log("Número de vértices: " + mesh.vertexCount);

        Vector3[] vertices = mesh.vertices;

        for (int i = 0; i < vertices.Length; i++)
        {
            Debug.Log(
                "v " +
                vertices[i].x + " " +
                vertices[i].y + " " +
                vertices[i].z
            );
        }
    }
}