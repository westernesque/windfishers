using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class IslandGenerator : MonoBehaviour

{
    public enum CloudLevel { Low, Normal, Heavy, Extreme };
    public enum Season { Summer, Winter, Fall, Spring };
    public CloudLevel CloudCoverage;
    public bool Volcano = true;
    public int VolcanoCountdownTime;
    public float IslandSize = 200.0f;
    public bool ConnectedIslands = true;
    public int NumberOfConnectedIslands;
    public List<Vector2> edgeVertices;
    public int[] triangleList;
    public int[] newTriangleList;
    public int numberOfHouses;
    public Triangulator tr;
    public EdgeCollider2D edgeCollider2D;
    public Mesh islandMesh;
    public Vector2 IslandBounds;
    public GameObject house;

    private void Awake()
    {
        Debug.Log("Island Generation Started...");
        CreateIslandMesh();
        numberOfHouses = Random.Range(10, 20);
        for (int i = 0; i < numberOfHouses; i++)
        {
            PlaceHuts();
        }
        //if (NumberOfConnectedIslands > 0)
        //{
        //    for (int i = 0; i < NumberOfConnectedIslands; i++)
        //    {
        //        // Generate a small island outside of the main island bounds
        //    }
        //}
    }

    // Start is called before the first frame update
    void Start()
    {
    }

    private void Update()
    {
        // Code to cycle through waves here.
    }

    void CreateIslandMesh()
    {
        islandMesh = new Mesh();
        if (ConnectedIslands)
        {
            NumberOfConnectedIslands = Random.Range(0, 3);
        }

        Vector2[] vertices2D = new Vector2[] {
            new Vector2(-Random.Range(0, IslandSize), 0),
            new Vector2(-Random.Range(0, IslandSize), Random.Range(0, IslandSize)),
            new Vector2(0, Random.Range(0, IslandSize)),
            new Vector2(Random.Range(0, IslandSize), Random.Range(0, IslandSize)),
            new Vector2(Random.Range(0, IslandSize), 0),
            new Vector2(Random.Range(0, IslandSize), -Random.Range(0, IslandSize)),
            new Vector2(0, -Random.Range(0, IslandSize)),
            new Vector2(-Random.Range(0, IslandSize), -Random.Range(0, IslandSize))
        };
        
        tr = new Triangulator(vertices2D);
        int[] indices = tr.Triangulate();

        Vector3[] vertices = new Vector3[vertices2D.Length];

        for (int i = 0; i < vertices.Length; i++)
        {
            vertices[i] = new Vector3(vertices2D[i].x, vertices2D[i].y, 0);
        }

        edgeVertices = new List<Vector2>();
        for (int i = 0; i < vertices.Length; i++)
        {
            edgeVertices.Add(vertices[i]);
        }
        edgeVertices.Add(vertices[0]);
        Vector3[] edgeVertices2D = new Vector3[edgeVertices.Count];
        for (int i = 0; i < edgeVertices.Count; i++)
        {
            edgeVertices2D[i] = edgeVertices[i];
        }

        islandMesh.vertices = vertices;
        islandMesh.triangles = indices;
        triangleList = islandMesh.triangles;
        islandMesh.RecalculateNormals();
        islandMesh.RecalculateBounds();

        GetComponent<MeshFilter>().mesh = islandMesh;
        //GetComponent<MeshRenderer>().material = (Material)Resources.Load("Materials/GrassSample");

        curveIslandMesh();
        VerticesToColliderPoints(islandMesh.vertices);
        PlaceSand(islandMesh.vertices);
        PlaceWaves(islandMesh.vertices, GetComponent<LineRenderer>().widthCurve);
        IslandBounds = islandMesh.bounds.extents;
        Debug.Log("Island Generated.");
    }

    void PlaceSand(Vector3[] vertices)
    {

        // Place sand along edges of polygon
        var lineRenderer = gameObject.GetComponent<LineRenderer>();
        edgeVertices = new List<Vector2>();
        for (int i = 0; i < vertices.Length; i++)
        {
            edgeVertices.Add(vertices[i]);
        }
        edgeVertices.Add(vertices[0]);
        Vector3[] edgeVertices2D = new Vector3[edgeVertices.Count];
        for (int i = 0; i < edgeVertices.Count; i++)
        {
            edgeVertices2D[i] = edgeVertices[i];
        }
        Debug.Log("Number of Verts for Place Tiles: " + edgeVertices2D.Length);
        int BeachCurveCount = Random.Range(2, edgeVertices2D.Length);
        AnimationCurve beachCurve = new AnimationCurve();
        //float[] beachCurve = new float[BeachCurveCount];
        for (int i = 0; i < BeachCurveCount; i++)
        {
            beachCurve.AddKey(Random.Range(0.25f, 1.0f), Random.Range(0.25f, 1.0f));
        }
        lineRenderer.positionCount = edgeVertices2D.Length;
        lineRenderer.widthCurve = beachCurve;
        lineRenderer.widthMultiplier = 2.0f;
        lineRenderer.SetPositions(edgeVertices2D);
    }

    void PlaceWaves(Vector3[] vertices, AnimationCurve sandWidthCurve)
    {
        GameObject waves = GameObject.Find("Island/Waves");
        LineRenderer shoreline = waves.GetComponent<LineRenderer>();
        edgeVertices = new List<Vector2>();
        for (int i = 0; i < vertices.Length; i++)
        {
            edgeVertices.Add(vertices[i]);
        }
        edgeVertices.Add(vertices[0]);
        Vector3[] edgeVertices2D = new Vector3[edgeVertices.Count];
        for (int i = 0; i < edgeVertices.Count; i++)
        {
            edgeVertices2D[i] = edgeVertices[i];
        }
        Debug.Log("Number of Verts for Place Tiles: " + edgeVertices2D.Length);
        shoreline.positionCount = edgeVertices2D.Length;
        shoreline.material = Resources.Load<Material>("Materials/Waves");
        shoreline.widthMultiplier = 2.0f;
        shoreline.SetPositions(edgeVertices2D);
    }

    void curveIslandMesh()
    {
        // STEP ONE: get halway points for all the edgeVertices, pipe into a new list.
        Vector2[] halfwayPoints = new Vector2[edgeVertices.Count];
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
        int curveNumberOfPoints = 5;
        List<Vector2> curveVerticesList = new List<Vector2>();
        for (int i = 0; i < edgeVertices.Count - 2; i++)
        {
            if (i < edgeVertices.Count)
            {
                for (int x = 0;  x < curveNumberOfPoints; x++)
                {
                    float t = x / (float)curveNumberOfPoints;
                    Vector2 position = CalculateQuadraticBezierPoint(t, halfwayPoints[i], edgeVertices[i + 1], halfwayPoints[i + 1]);
                    curveVerticesList.Add(position);
                }
            }
            else
            {
                for (int x = 0; x < curveNumberOfPoints; x++)
                {
                    float t = i / (float)curveNumberOfPoints;
                    Vector2 position = CalculateQuadraticBezierPoint(t, halfwayPoints[i], edgeVertices[0], halfwayPoints[0]);
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

        islandMesh.vertices = curveVertices;
        islandMesh.triangles = curveIndices;
        islandMesh.SetUVs(0, curveVertices);
        triangleList = islandMesh.triangles;
        islandMesh.RecalculateNormals();
        islandMesh.RecalculateBounds();

        GetComponent<MeshFilter>().mesh = islandMesh;
        GetComponent<MeshRenderer>().material = (Material)Resources.Load("Materials/Grass");

    }

    void VerticesToColliderPoints(Vector3[] vertices)
    {
        edgeCollider2D = gameObject.AddComponent<EdgeCollider2D>();
        edgeVertices = new List<Vector2>();
        for (int i = 0; i < vertices.Length; i++)
        {
            edgeVertices.Add(vertices[i]);
        }
        edgeVertices.Add(vertices[0]);
        Vector2[] edgeVertices2D = new Vector2[edgeVertices.Count];
        for (int i = 0; i < edgeVertices.Count; i++)
        {
            edgeVertices2D[i] = edgeVertices[i];
        }
        edgeCollider2D.points = edgeVertices2D;
        edgeCollider2D.edgeRadius = 0.05f;
    }

    Vector2 CalculateQuadraticBezierPoint(float t, Vector3 p0, Vector3 p1, Vector3 p2)
    {
        float u = 1 - t;
        float tt = t * t;
        float uu = u * u;
        Vector3 p = uu * p0;
        p += 2 * u * t * p1;
        p += tt * p2;
        return p;
    }

    void PlaceHuts()
    {
        var triangleVerts = GameObject.Find("Island").GetComponent<IslandGenerator>().islandMesh.vertices;
        var triangleIndices = GameObject.Find("Island").GetComponent<IslandGenerator>().islandMesh.triangles;
        int randomTriangle = Random.Range(0, (triangleVerts.Length * 2) / 3);
        bool HouseSpawned = false;
        int flipHouseX = Random.Range(0, 2);
        List<List<Vector2>> triangleList = new List<List<Vector2>>();
        for (int i = 0; i < triangleIndices.Length; i++)
        {
            if (i % 3 == 0 && i < triangleIndices.Length)
            {
                var vert1 = triangleIndices[i];
                var vert2 = triangleIndices[i + 1];
                var vert3 = triangleIndices[i + 2];
                triangleList.Add(new List<Vector2> { triangleVerts[vert1], triangleVerts[vert2], triangleVerts[vert3] });
            }
        }
        var chosenTriangle = triangleList[randomTriangle];
        Vector2[] chosenTriangleList = new Vector2[chosenTriangle.Count];
        for (int i = 0; i < chosenTriangle.Count; i++)
        {
            chosenTriangleList[i] = chosenTriangle[i];
        }
        Vector3 houseSpawnPoint = GetRandomPoint(chosenTriangle[0], chosenTriangle[1], chosenTriangle[2]);
        while (HouseSpawned == false)
        {
            if (InTriangle(houseSpawnPoint, chosenTriangle[0], chosenTriangle[1], chosenTriangle[2]))
            {
                if (TriangleArea(chosenTriangleList) >= TriangleArea(house.GetComponent<SpriteRenderer>().sprite.vertices))
                {
                    var newHouse = Instantiate(house, houseSpawnPoint, transform.rotation);
                    newHouse.GetComponent<SpriteRenderer>().sortingOrder = 3;
                    if (flipHouseX == 1)
                    {
                        newHouse.GetComponent<SpriteRenderer>().flipX = true;
                    }
                    //HumanList.Add(HIndex, newHuman);
                    newHouse.transform.parent = GameObject.Find("Island/Houses").transform;
                    //HIndex += 1;
                    //Debug.Log("Triangle Area(): " + TriangleArea(chosenTriangleList));
                    //Debug.Log("Triangle Area of House(): " + TriangleArea(house.GetComponent<SpriteRenderer>().sprite.vertices));
                    HouseSpawned = true;
                }
            }
            else
            {
                randomTriangle = Random.Range(0, (triangleVerts.Length * 2) / 3);
                chosenTriangle = triangleList[randomTriangle];
                for (int i = 0; i < chosenTriangle.Count; i++)
                {
                    chosenTriangleList[i] = chosenTriangle[i];
                }
                houseSpawnPoint = GetRandomPoint(chosenTriangle[0], chosenTriangle[1], chosenTriangle[2]);
                //Debug.Log("Try again.");
            }
        }
    }
    public Vector3 GetRandomPoint(Vector2 vec1, Vector2 vec2, Vector2 vec3)
    {
        Vector3 min = GetMinPoint(vec1, vec2, vec3);
        Vector3 max = GetMaxPoint(vec1, vec2, vec3);
        return new Vector3(Random.Range(min.x, max.x), Random.Range(min.y, max.y), Random.Range(min.z, max.z));
    }

    Vector3 GetMinPoint(Vector2 vec1, Vector2 vec2, Vector2 vec3)
    {
        Vector3 point = new Vector3();
        point.x = Mathf.Min(vec1.x, vec2.x, vec3.x);
        point.y = Mathf.Min(vec1.y, vec2.y, vec3.y);
        point.z = Mathf.Min(0.0f, 0.0f, 0.0f);
        return point;
    }

    Vector3 GetMaxPoint(Vector2 vec1, Vector2 vec2, Vector2 vec3)
    {
        Vector3 point = new Vector3();
        point.x = Mathf.Max(vec1.x, vec2.x, vec3.x);
        point.y = Mathf.Max(vec1.y, vec2.y, vec3.y);
        point.z = Mathf.Max(0.0f, 0.0f, 0.0f);
        return point;
    }

    public bool InTriangle(Vector2 p, Vector2 p0, Vector2 p1, Vector2 p2)
    {
        var a = .5f * (-p1.y * p2.x + p0.y * (-p1.x + p2.x) + p0.x * (p1.y - p2.y) + p1.x * p2.y);
        var sign = a < 0 ? -1 : 1;
        var s = (p0.y * p2.x - p0.x * p2.y + (p2.y - p0.y) * p.x + (p0.x - p2.x) * p.y) * sign;
        var t = (p0.x * p1.y - p0.y * p1.x + (p0.y - p1.y) * p.x + (p1.x - p0.x) * p.y) * sign;

        return s > 0 && t > 0 && (s + t) < 2 * a * sign;
    }

    public float TriangleArea(Vector2[] verts)
    {
        Vector3 result = Vector3.zero;
        for (int p = verts.Length - 1, q = 0; q < verts.Length; p = q++)
        {
            result += Vector3.Cross(verts[q], verts[p]);
        }
        result *= 0.5f;
        return result.magnitude;
    }
}
