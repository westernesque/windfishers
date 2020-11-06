using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IslandTools : MonoBehaviour
{
    // TO-DO: see about consolidating the two mesh generation functions.
    public static Mesh GenerateMesh(Vector2[] vertices)
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

    public static Mesh GenerateCurveMesh(List<Vector2> edgeVertices, GameObject islandGameObject)
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

        return islandMesh;
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
