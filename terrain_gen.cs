// A copy of the Brackey's tutorial on proc gen mesh. I just made the variables adjustable in the inspector.

using System;
using UnityEngine;

[RequireComponent(typeof(MeshFilter))]
[RequireComponent(typeof(MeshRenderer))]
public class terrain_gen : MonoBehaviour
{
    Mesh _mesh;
    Vector3[] vertices;
    int[] triangles;
    Color[] colors;

    [SerializeField] int xSize, zSize, maxHeight;
    [SerializeField] float zoomLevel;
    [SerializeField] Gradient _gradient;

    float minTerrainHeight, maxTerrainHeight;
    void Start()
    {
        _mesh = new();
        GetComponent<MeshFilter>().mesh = _mesh;
        CreateMesh();
    }

    void Update()
    {
        UpdateMesh();
    }

    void CreateMesh()
    {
        vertices = new Vector3[(xSize + 1) * (zSize + 1)];

        for (int i = 0, z = 0; z <= zSize; z++)
        {
            for (int x = 0; x <= xSize; x++)
            {
                float y = Mathf.PerlinNoise(x * zoomLevel, z * zoomLevel) * maxHeight;
                vertices[i] = new Vector3(x, y, z);

                if (y > maxTerrainHeight)
                {
                    maxTerrainHeight = y;
                }

                if (y < minTerrainHeight)
                {
                    minTerrainHeight = y;
                }
                i++;
            }
        }

        triangles = new int[xSize * zSize * 6];
        int vert = 0;
        int tris = 0;

        for (int z = 0; z < zSize; z++)
        {
            for (int x = 0; x < xSize; x++)
            {

                triangles[tris + 0] = vert + 0;
                triangles[tris + 1] = vert + xSize + 1;
                triangles[tris + 2] = vert + 1;
                triangles[tris + 3] = vert + 1;
                triangles[tris + 4] = vert + xSize + 1;
                triangles[tris + 5] = vert + xSize + 2;
                vert++;
                tris += 6;
            }
            vert++;
        }

        colors = new Color[vertices.Length];

        for (int i = 0, z = 0; z <= zSize; z++)
        {
            for (int x = 0; x <= xSize; x++)
            {
                float height = Mathf.InverseLerp(minTerrainHeight, maxTerrainHeight, vertices[i].y);
                colors[i] = _gradient.Evaluate(height);
                i++;
            }
        }
        
    }

    void UpdateMesh()
    {
        _mesh.Clear();
        _mesh.vertices = vertices;
        _mesh.triangles = triangles;
        _mesh.colors = colors;
        _mesh.RecalculateNormals();
    }
}
