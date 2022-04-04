using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IslandGeneration : MonoBehaviour
{
    IslandInfo IslandInfo;
    GameObject mainIsland;
    
    void Start()
    {
        // Get variables from the IslandInfo script.
        IslandInfo = GameObject.Find("Island").GetComponent<IslandInfo>();
        // Get the mainIsland game object.
        mainIsland = GameObject.Find("Main Island");
        // Set the main island's mesh to be the generated island mesh shape.
        mainIsland.GetComponent<MeshFilter>().mesh = GenerateMainIslandShape(IslandInfo.IslandSize);
    }

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
        // 3D Vector list of the island vertices.
        Vector3[] mainIslandVertices3D = new Vector3[unsortedMainIslandVertexCount];
        if (IslandInfo.SubIslandCount == 0)
        {
            for (int i = 1; i < unsortedMainIslandVertexCount - 1; i++)
            {
                Vector2 vertex = new Vector2(Random.Range(islandBounds.x * 0.2f, islandBounds.x * 0.8f), Random.Range(islandBounds.y * 0.2f, islandBounds.y * 0.8f));
                mainIslandVertices[i] = vertex;
                mainIslandVertices3D[i] = new Vector3(vertex.x, vertex.y, 0f);
            }
        }
        else
        {
            for (int i = 1; i < unsortedMainIslandVertexCount - 1; i++)
            {
                Vector2 vertex = new Vector2(Random.Range(islandBounds.x * 0.35f, islandBounds.x * 0.65f), Random.Range(islandBounds.y * 0.35f, islandBounds.y * 0.65f));
                mainIslandVertices[i] = vertex;
                mainIslandVertices3D[i] = new Vector3(vertex.x, vertex.y, 0f);
            }
        }
        // Create a new Triangulator to create the island mesh.
        Triangulator triangulator = new Triangulator(mainIslandVertices);
        // Create an int list of indices for the island mesh.
        int[] mainIslandIndices = triangulator.Triangulate();

        // Create a new island mesh.
        Mesh islandMesh = new Mesh();
        // Set the vertices of the island mesh.
        islandMesh.vertices = mainIslandVertices3D;
        // Set the triangles of the island mesh.
        islandMesh.triangles = mainIslandIndices;
        // Set the normals of the island mesh.
        islandMesh.RecalculateNormals();
        // Set the bounds of the island mesh.
        islandMesh.RecalculateBounds();

        return islandMesh;
    }

}
