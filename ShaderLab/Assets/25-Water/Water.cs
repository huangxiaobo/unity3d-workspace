using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[RequireComponent(typeof(MeshFilter), typeof(MeshRenderer))]
public class Water : MonoBehaviour
{
    private int xSize = 200;
    private int zSize = 200;
    private float width = 0.1f;
    private Mesh mesh;

    private Vector3[] vertices;
    private Vector2[] uvs;

    void Start()
    {
        Generate();
    }
    
    private void Generate()
    {
        GetComponent<MeshFilter>().mesh = mesh = new Mesh();
        mesh.name = "Procedural Grid";

        vertices = new Vector3[(xSize + 1) * (zSize + 1)];
        uvs = new Vector2[(xSize + 1) * (zSize + 1)];

        for (int i = 0, z = -zSize / 2; z <= zSize/2; z++)
        {
            for (int x = -xSize / 2; x <= xSize/2; x++, i++)
            {
                vertices[i] = new Vector3(x*width, 0, z*width);
                float u = (x + xSize / 2) * 1.0f / xSize;
                float v = (z + zSize / 2) * 1.0f / zSize;
                uvs[i] = new Vector2(u, v);
            }
        }
        mesh.vertices = vertices;
        mesh.uv = uvs;

        int[] triangles = new int[xSize * zSize * 6];
        for (int ti = 0, vi = 0, z = 0; z < zSize; z++, vi++)
        {
            for (int x = 0; x < xSize; x++, ti += 6, vi++)
            {
                triangles[ti] = vi;
                triangles[ti + 3] = triangles[ti + 2] = vi + 1;
                triangles[ti + 4] = triangles[ti + 1] = vi + xSize + 1;
                triangles[ti + 5] = vi + xSize + 2;
            }
        }
        mesh.triangles = triangles;
        mesh.RecalculateNormals();
    }
}