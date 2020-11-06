using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditorInternal;
using UnityEngine;

public class LevelStart : MonoBehaviour
{
    public enum CloudLevel { Low, Normal, Heavy, Extreme };
    public enum Season { Summer, Winter, Fall, Spring };
    public CloudLevel CloudCoverage;
    public int IslandSize = 100;
    public bool Volcano = false;
    public int NumberOfConnectedIslands;
    public List<GameObject> IslandList;

    // Start is called before the first frame update
    void Start()
    {
        // Vertices for the main island.
        GameObject MainIsland = new GameObject();
        MainIsland.name = "Main Island";
        MainIsland.transform.parent = this.transform;
        MainIsland.AddComponent<MeshFilter>();
        MainIsland.AddComponent<MeshRenderer>();
        MainIsland.AddComponent<EdgeCollider2D>();
        Vector2[] mainIslandVertices = new Vector2[] 
        {
            new Vector2(-UnityEngine.Random.Range(0, IslandSize), 0),
            new Vector2(-UnityEngine.Random.Range(0, IslandSize), UnityEngine.Random.Range(0, IslandSize)),
            new Vector2(0, UnityEngine.Random.Range(0, IslandSize)),
            new Vector2(UnityEngine.Random.Range(0, IslandSize), UnityEngine.Random.Range(0, IslandSize)),
            new Vector2(UnityEngine.Random.Range(0, IslandSize), 0),
            new Vector2(UnityEngine.Random.Range(0, IslandSize), -UnityEngine.Random.Range(0, IslandSize)),
            new Vector2(0, -UnityEngine.Random.Range(0, IslandSize)),
            new Vector2(-UnityEngine.Random.Range(0, IslandSize), -UnityEngine.Random.Range(0, IslandSize))
        };

        Debug.Log("Island Generation Started...");
        Mesh MainIslandMesh = IslandGenerator.Generate(mainIslandVertices);
        MainIsland.GetComponent<MeshFilter>().mesh = MainIslandMesh;
        MainIsland.GetComponent<MeshRenderer>().material = Resources.Load<Material>("Materials/Grass");
        MainIsland.GetComponent<EdgeCollider2D>().points = mainIslandVertices;
        Vector3 MainIslandBounds = MainIslandMesh.bounds.max;
        
        // Edge stuff for the island curves.
        List<Vector2> edgeVertices = new List<Vector2>();
        for (int i = 0; i < mainIslandVertices.Length; i++)
        {
            edgeVertices.Add(mainIslandVertices[i]);
        }
        edgeVertices.Add(mainIslandVertices[0]);
        Vector3[] edgeVertices2D = new Vector3[edgeVertices.Count];
        for (int i = 0; i < edgeVertices.Count; i++)
        {
            edgeVertices2D[i] = edgeVertices[i];
        }

        IslandList.Add(MainIsland);
        curveIslandMesh(edgeVertices, MainIsland);
        for (int i = 0; i < NumberOfConnectedIslands; i++)
        {
            GenerateConnectingIslands();
        }

        void GenerateConnectingIslands()
        {
            // Vertices for the small connecting islands.
            Vector2[] connectingIslandVertices = new Vector2[]
            {
            new Vector2(-UnityEngine.Random.Range(0, IslandSize / UnityEngine.Random.Range(1, 2)), 0),
            new Vector2(-UnityEngine.Random.Range(0, IslandSize / UnityEngine.Random.Range(1, 2)), UnityEngine.Random.Range(0, IslandSize / 3)),
            new Vector2(0, UnityEngine.Random.Range(0, IslandSize / UnityEngine.Random.Range(1, 2))),
            new Vector2(UnityEngine.Random.Range(0, IslandSize / UnityEngine.Random.Range(1, 2)), 0)
            };

            // Generates connecting island mesh.
            GameObject connectedIsland = new GameObject();
            connectedIsland.name = "Connected Island";
            connectedIsland.transform.parent = this.transform;
            connectedIsland.AddComponent<MeshFilter>();
            connectedIsland.AddComponent<MeshRenderer>();
            connectedIsland.AddComponent<EdgeCollider2D>();
            Mesh connectedIslandMesh = IslandGenerator.Generate(connectingIslandVertices);
            connectedIsland.GetComponent<MeshFilter>().mesh = connectedIslandMesh;
            connectedIsland.GetComponent<EdgeCollider2D>().points = connectingIslandVertices;
            connectedIsland.GetComponent<MeshRenderer>().material = Resources.Load<Material>("Materials/Grass");

            // Move connecting island outside of island mesh.
            IslandList.Add(connectedIsland);

            // Edge stuff for the island curves.
            List<Vector2> ci_edgeVertices = new List<Vector2>();
            for (int i = 0; i < connectingIslandVertices.Length; i++)
            {
                ci_edgeVertices.Add(connectingIslandVertices[i]);
            }
            ci_edgeVertices.Add(connectingIslandVertices[0]);
            Vector3[] ci_edgeVertices2D = new Vector3[ci_edgeVertices.Count];
            for (int i = 0; i < ci_edgeVertices.Count; i++)
            {
                ci_edgeVertices2D[i] = ci_edgeVertices[i];
            }
            curveIslandMesh(ci_edgeVertices, connectedIsland);
        }

    }

    void curveIslandMesh(List<Vector2> edgeVertices, GameObject islandGameObject)
    {
        // STEP ONE: get halway points for all the edgeVertices, pipe into a new list.
        Vector2[] halfwayPoints = new Vector2[edgeVertices.Count];
        Mesh islandMesh = islandGameObject.GetComponent<MeshFilter>().mesh;
        for (int i = 0; i < edgeVertices.Count; i++)
        {
            if (i < edgeVertices.Count - 1)
            {
                float distanceBetweenVecs = Vector2.Distance(edgeVertices[i], edgeVertices[i + 1]);
                Vector2 halfwayPoint = Vector2.Lerp(edgeVertices[i], edgeVertices[i + 1], (distanceBetweenVecs / 2) / (edgeVertices[i] - edgeVertices[i + 1]).magnitude);
                halfwayPoints[i] = halfwayPoint;
            }
            else
            {
                float distanceBetweenVecs = Vector2.Distance(edgeVertices[i], edgeVertices[0]);
                Vector2 halfwayPoint = Vector2.Lerp(edgeVertices[i], edgeVertices[0], (distanceBetweenVecs / 2) / (edgeVertices[i] - edgeVertices[0]).magnitude);
                halfwayPoints[i] = halfwayPoint;
            }
        }

        // STEP TWO: created the curves, store the points into a new vertices list.
        int curveNumberOfPoints = 10;
        List<Vector2> curveVerticesList = new List<Vector2>();
        for (int i = 0; i < edgeVertices.Count - 1; i++)
        {
            if (i < edgeVertices.Count - 2)
            {
                for (int x = 0; x < curveNumberOfPoints; x++)
                {
                    float t = x / (float)curveNumberOfPoints;
                    Vector2 position = IslandGenerator.CalculateQuadraticBezierPoint(t, halfwayPoints[i], edgeVertices[i + 1], halfwayPoints[i + 1]);
                    curveVerticesList.Add(position);
                }
            }
            else
            {
                for (int x = 0; x < curveNumberOfPoints; x++)
                {
                    float t = x / (float)curveNumberOfPoints;
                    Vector2 position = IslandGenerator.CalculateQuadraticBezierPoint(t, halfwayPoints[i], edgeVertices[0], halfwayPoints[0]);
                    curveVerticesList.Add(position);
                }
            }
        }
        Vector2[] vertsList = new Vector2[curveVerticesList.Count];
        for (int i = 0; i < curveVerticesList.Count; i++)
        {
            vertsList[i] = curveVerticesList[i];
        }

        // STEP THREE: triangulate and do all the mesh stuff.
        Triangulator ntr = new Triangulator(vertsList);
        int[] curveIndices = ntr.Triangulate();

        Vector3[] curveVertices = new Vector3[vertsList.Length];

        for (int i = 0; i < curveVertices.Length; i++)
        {
            curveVertices[i] = new Vector3(vertsList[i].x, vertsList[i].y, 0);
        }

        islandGameObject.GetComponent<EdgeCollider2D>().points = vertsList;
        islandMesh.vertices = curveVertices;
        islandMesh.triangles = curveIndices;
        islandMesh.SetUVs(0, curveVertices);
        islandMesh.RecalculateNormals();
        islandMesh.RecalculateBounds();
    }

    bool OffsetIslands()
    {
        GameObject MainIsland = GameObject.Find("Main Island");
        GameObject connectedIsland = GameObject.Find("Connected Island");
        Vector2 randomXYDir = new Vector2(UnityEngine.Random.Range(-1, 1), UnityEngine.Random.Range(-1, 1));
        if (MainIsland.GetComponent<EdgeCollider2D>().bounds.Intersects(connectedIsland.GetComponent<EdgeCollider2D>().bounds))
        {
            connectedIsland.transform.position = new Vector3(connectedIsland.transform.position.x + randomXYDir.x, connectedIsland.transform.position.y + randomXYDir.y, connectedIsland.transform.position.z);
            return true;
        }
        else
        {
            return false;
        }
    }


    // Update is called once per frame
    void Update()
    {
        
    }
}
