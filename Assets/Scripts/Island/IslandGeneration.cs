using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IslandGeneration : MonoBehaviour
{
    IslandInfo IslandInfo;
    GameObject mainIsland;
    // Create the islandTools object.
    IslandTools islandTools = new IslandTools();

    void Start()
    {
        // Get variables from the IslandInfo script.
        IslandInfo = GameObject.Find("Island").GetComponent<IslandInfo>();
        // Get the mainIsland game object.
        mainIsland = GameObject.Find("Main Island");
        // Set the main island's mesh to be the generated island mesh shape.
        mainIsland.GetComponent<MeshFilter>().mesh = GenerateMainIslandShape(IslandInfo.IslandSize);
        mainIsland.GetComponent<MeshRenderer>().material = Resources.Load("Materials/Grass") as Material;
    }

    // Generates the shape of the main island based on the IslandSize var from the IslandInfo script.
    Mesh GenerateMainIslandShape(int IslandSize)
    {
        // Set the bounds of the island in 2D space.
        Vector2 islandBounds = new Vector2(IslandSize, IslandSize);
        // Randomly generate the number of vertex points the island will have.
        int mainIslandVertexCount = Random.Range(5, 15);
        // List of island vertices.
        Vector2[] mainIslandVertices = new Vector2[mainIslandVertexCount];
        // 3D Vector list of the island vertices.
        Vector3[] mainIslandVertices3D = new Vector3[mainIslandVertexCount];
        // Set the rest of the vertices to be randomly generated within the islandBounds with an 80% margin if SubIslandCount is 0, else within a 65% margin.
        if (IslandInfo.SubIslandCount == 0)
        {
            for (int i = 0; i < mainIslandVertexCount; i++)
            {
                Vector2 vertex = new Vector2(Random.Range(islandBounds.x * 0.2f, islandBounds.x * 0.8f), Random.Range(islandBounds.y * 0.2f, islandBounds.y * 0.8f));
                mainIslandVertices[i] = vertex;
                mainIslandVertices3D[i] = new Vector3(vertex.x, vertex.y, 0f);
            }
        }
        else
        {
            for (int i = 0; i < mainIslandVertexCount; i++)
            {
                Vector2 vertex = new Vector2(Random.Range(islandBounds.x * 0.35f, islandBounds.x * 0.65f), Random.Range(islandBounds.y * 0.35f, islandBounds.y * 0.65f));
                mainIslandVertices[i] = vertex;
                mainIslandVertices3D[i] = new Vector3(vertex.x, vertex.y, 0f);
            }
        }

        // Create a new island mesh.
        Mesh islandMesh = new Mesh();
        islandMesh.vertices = mainIslandVertices3D;
        islandMesh.RecalculateBounds();

        // Sort the mainIslandPoints clockwise.
        Vector2 islandCenterPoint = new Vector2(islandMesh.bounds.center.x, islandMesh.bounds.center.y);
        System.Array.Sort(mainIslandVertices, new ClockwiseComparer(islandCenterPoint));

        // List of halfway points between the main island vertices.
        Vector2[] halfwayPoints = new Vector2[mainIslandVertices.Length];

        // Get the halfway points between the main island vertices, put them in the halfwayPoints list.
        for (int i = 0; i < mainIslandVertices.Length; i++)
        {
            if (i < mainIslandVertices.Length - 2)
            {
                float distanceBetweenVecs = Vector2.Distance(mainIslandVertices[i], mainIslandVertices[i + 1]);
                Vector2 halfwayPoint = Vector2.Lerp(mainIslandVertices[i], mainIslandVertices[i + 1], (distanceBetweenVecs / 2) / (mainIslandVertices[i] - mainIslandVertices[i + 1]).magnitude);
                halfwayPoints[i] = halfwayPoint;
            }
            else
            {
                float distanceBetweenVecs = Vector2.Distance(mainIslandVertices[i], mainIslandVertices[0]);
                Vector2 halfwayPoint = Vector2.Lerp(mainIslandVertices[i], mainIslandVertices[0], (distanceBetweenVecs / 2) / (mainIslandVertices[i] - mainIslandVertices[0]).magnitude);
                halfwayPoints[i] = halfwayPoint;
            }
        }
        // int for the number of points between two main island vertices (makes the curve more smooth).
        int curvePoints = 8;
        // List of the curve points between the main island vertices. Using List<Vector2> because we don't know what the size of the list will be and C# be like that...
        List<Vector2> curvePointsList = new List<Vector2>();
        for (int i = 0; i < mainIslandVertices.Length - 1; i++)
        {
            if (i < mainIslandVertices.Length - 2)
            {
                for (int x = 0; x < curvePoints; x++)
                {
                    float t = x / (float)curvePoints;
                    Vector2 position = islandTools.CalculateQuadraticBezierPoint(t, halfwayPoints[i], mainIslandVertices[i + 1], halfwayPoints[i + 1]);
                    curvePointsList.Add(position);
                }
            }
            else
            {
                for (int x = 0; x < curvePoints; x++)
                {
                    float t = x / (float)curvePoints;
                    Vector2 position = islandTools.CalculateQuadraticBezierPoint(t, halfwayPoints[i], mainIslandVertices[0], halfwayPoints[0]);
                    curvePointsList.Add(position);
                }
            }
        }
        curvePointsList.Add(curvePointsList[0]);
        // Convert the curve points list to an array.
        Vector2[] curvePointsArray = curvePointsList.ToArray();
        Vector2[] mainIslandPointsSorted = curvePointsArray;
        // Put the sorted main island points into a new Vector3 list.
        Vector3[] mainIslandPointsSorted3D = new Vector3[mainIslandPointsSorted.Length];
        for (int i = 0; i < mainIslandPointsSorted.Length; i++)
        {
            mainIslandPointsSorted3D[i] = new Vector3(mainIslandPointsSorted[i].x, mainIslandPointsSorted[i].y, 0f);
        }
        // Triangulate the new curve points.
        Triangulator triangulator = new Triangulator(mainIslandPointsSorted);
        int[] mainIslandIndices = triangulator.Triangulate();

        // Set the island mesh vertices to the mainIslandPoints list.
        islandMesh.vertices = mainIslandPointsSorted3D;
        // Set the island mesh triangles to the mainIslandIndices list.
        islandMesh.triangles = mainIslandIndices;
        // Set the island mesh UVs.
        islandMesh.SetUVs(0, mainIslandPointsSorted3D);
        // Set the island mesh normals.
        islandMesh.RecalculateNormals();
        // Set the island mesh bounds.
        islandMesh.RecalculateBounds();
        // Return the island mesh.
        return islandMesh;
    }

    // Generates the shaps and locations of the sub islands.
    Mesh[] GenerateSubIslandShapes(int NumberOfIslands, Mesh MainIsland)
    {
        // List of the sub island meshes.
        Mesh[] subIslandMeshes = new Mesh[NumberOfIslands];
        // For each sub island, do all the same things as the main island.
        for (int i = 0; i < NumberOfIslands; i++)
        {
            // Create a new sub island mesh.
            Mesh subIslandMesh = new Mesh();
            // Randomly generate the number of vertices for the sub island.
            int subIslandVertexCount = Random.Range(3, 7);
            // Randomly generate the size (bounds) of the sub island based on the main island size.
            Vector2 subIslandBounds = new Vector2(IslandInfo.IslandSize * Random.Range(0.1f, 0.3f), IslandInfo.IslandSize * Random.Range(0.1f, 0.3f));
            // Vector2 list of the sub island vertices.
            Vector2[] subIslandVertices = new Vector2[subIslandVertexCount];
            // Vector3 list of the sub island vertices.
            Vector3[] subIslandVertices3D = new Vector3[subIslandVertexCount];
        }
        return subIslandMeshes;
    }
}
