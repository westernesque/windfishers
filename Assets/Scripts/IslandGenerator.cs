using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IslandGenerator : MonoBehaviour
{
    // Generates and returns a mesh based on the vertices given (curved mesh created in this function).
    public static Mesh Generate(Vector2[] vertices)
    {
        Triangulator tr = new Triangulator(vertices);
        int[] indices = tr.Triangulate();
        Vector3[] vertices3D = new Vector3[vertices.Length];
        
        for (int i = 0; i < vertices3D.Length; i++)
        {
            vertices3D[i] = new Vector3(vertices[i].x, vertices[i].y, 0);
        }

        Mesh islandMesh = new Mesh();

        islandMesh.vertices = vertices3D;
        islandMesh.triangles = indices;
        islandMesh.SetUVs(0, vertices);
        islandMesh.RecalculateNormals();
        islandMesh.RecalculateBounds();


        return islandMesh;
    }

    // Adds random buildings to the island.
    void AddBuildings(int numberOfBuildings)
    {

    }

    // Adds random foliage to island.
    void AddFoliage(float islandCoverage)
    {

    }

    void AddGrass()
    {
        // Be mindful of sorting layers of... everything
    }

    void AddCliffs(int numberOfCliffs)
    {
        // Again, have to be sure to be mindful of sorting layers.
    }

    public static Vector2 CalculateQuadraticBezierPoint(float t, Vector3 p0, Vector3 p1, Vector3 p2)
    {
        float u = 1 - t;
        float tt = t * t;
        float uu = u * u;
        Vector3 p = uu * p0;
        p += 2 * u * t * p1;
        p += tt * p2;
        return p;
    }
}
