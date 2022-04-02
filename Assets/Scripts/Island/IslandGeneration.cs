using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IslandGeneration : MonoBehaviour
{
    // Get variables from IslandInfo script.
    IslandInfo IslandInfo = GameObject.Find("Island").GetComponent<IslandInfo>();

    // Generates the shape of the main island based on the IslandSize var from the IslandInfo script.
    Mesh GenerateMainIslandShape(int IslandSize)
    {
        // Set the bounds of the island in 2D space.
        Vector2 islandBounds = new Vector2(IslandSize, IslandSize);
        Vector2 mainIslandStartPoint = new Vector2();
        // Picks a random point within the islandBounds with an 80% margin if SubIslandCount count is 0, else within a 65% margin.
        if (IslandInfo.SubIslandCount == 0)
        {
            mainIslandStartPoint = new Vector2(Random.Range(islandBounds.x * 0.2f, islandBounds.x * 0.8f), Random.Range(islandBounds.y * 0.2f, islandBounds.y * 0.8f));
        }
        else
        {
            mainIslandStartPoint = new Vector2(Random.Range(islandBounds.x * 0.35f, islandBounds.x * 0.65f), Random.Range(islandBounds.y * 0.35f, islandBounds.y * 0.65f));
        }
        // Randomly generate the number of vertex points the island will have.
        int unsortedMainIslandVertexCount = Random.Range(5, 25);
        // List of island vertices.
        Vector2[] mainIslandVertices = new Vector2[unsortedMainIslandVertexCount];
        // Set the first vertex to the main island start point.
        mainIslandVertices[0] = mainIslandStartPoint;
        // Set the rest of the vertices to be randomly generated within the islandBounds with an 80% margin if SubIslandCount is 0, else within a 65% margin.
        if (IslandInfo.SubIslandCount == 0)
        {
            for (int i = 1; i < unsortedMainIslandVertexCount - 1; i++)
            {
                mainIslandVertices[i] = new Vector2(Random.Range(islandBounds.x * 0.2f, islandBounds.x * 0.8f), Random.Range(islandBounds.y * 0.2f, islandBounds.y * 0.8f));
            }
        }
        else
        {
            for (int i = 1; i < unsortedMainIslandVertexCount - 1; i++)
            {
                mainIslandVertices[i] = new Vector2(Random.Range(islandBounds.x * 0.35f, islandBounds.x * 0.65f), Random.Range(islandBounds.y * 0.35f, islandBounds.y * 0.65f));
            }
        }
        // Set the last vertex to be the same as the first vertex.
        mainIslandVertices[unsortedMainIslandVertexCount - 1] = mainIslandVertices[0];
        // List of sorted island vertices.
        Vector3[] sortedMainIslandVertices = new Vector3[unsortedMainIslandVertexCount];
        // Sort the unsorted island vertices by the distance from the previous vertex in the list.
        for (int i = 0; i < unsortedMainIslandVertexCount; i++)
        {
            // Loop through the unsorted island vertices.
            for (int j = 0; j < unsortedMainIslandVertexCount; j++)
            {
                // If the current vertex is closer to the previous vertex than the current vertex is to the next vertex.
                if (Vector2.Distance(sortedMainIslandVertices[i - 1], sortedMainIslandVertices[i]) > Vector2.Distance(sortedMainIslandVertices[i], sortedMainIslandVertices[i + 1]))
                {
                    // Set the current vertex to be the next vertex.
                    sortedMainIslandVertices[i] = sortedMainIslandVertices[i + 1];
                }
            }
        }
        // Set the last vertex to be the same as the first vertex.
        sortedMainIslandVertices[unsortedMainIslandVertexCount - 1] = sortedMainIslandVertices[0];
        // List of island triangles.
        int[] mainIslandTriangles = new int[(sortedMainIslandVertices.Length - 1) * 3];
        // Loop through the sorted island vertices.
        for (int i = 0; i < sortedMainIslandVertices.Length - 1; i++)
        {
            // Set the current triangle to be the current vertex, the next vertex, and the previous vertex.
            mainIslandTriangles[i * 3] = i;
            mainIslandTriangles[i * 3 + 1] = i + 1;
            mainIslandTriangles[i * 3 + 2] = i + sortedMainIslandVertices.Length - 1;
        }
        // Create a new island mesh.
        Mesh islandMesh = new Mesh();
        // Set the island vertices.
        islandMesh.vertices = sortedMainIslandVertices;
        // Set the island triangles.
        islandMesh.triangles = mainIslandTriangles;
        // Set the island normals.
        islandMesh.RecalculateNormals();
        // Return the island mesh.
        return islandMesh;
    }
}
