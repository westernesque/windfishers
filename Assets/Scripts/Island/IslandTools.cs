using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IslandTools
{
    // Calculate the bezier point for a given vector3.
    public Vector2 CalculateQuadraticBezierPoint(float t, Vector3 p0, Vector3 p1, Vector3 p2)
    {
        float u = 1 - t;
        float tt = t * t;
        float uu = u * u;
        Vector3 p = uu * p0;
        p += 2 * u * t * p1;
        p += tt * p2;
        return p;
    }
    public float CalculateArea(Vector3[] meshVertices)
    {
        var result = 0f;
        for (int p = meshVertices.Length - 1, q = 0; q < meshVertices.Length; p = q++)
            result += (Vector3.Cross(meshVertices[q], meshVertices[p])).magnitude;

        return result * .5f;
    }
    
    public void RotateIsland(Vector3 islandCenterPivot, Vector3[] islandVertices, Vector3 rotation)
    {
        // The center point that the island will be rotated around.
        Vector3 center = new Vector3(islandCenterPivot.x, islandCenterPivot.y, islandCenterPivot.z);
        Quaternion newRotation = new Quaternion();
        newRotation.eulerAngles = rotation;//the degrees the vertices are to be rotated, for example (0,90,0) 

        for (int i = 0; i < islandVertices.Length; i++)
        {//vertices being the array of vertices of your mesh
            islandVertices[i] = newRotation * (islandVertices[i] - center) + center;
        }
    }
}
// A function to order a list of vertices clockwise.
public class ClockwiseComparer : IComparer<Vector2>
{
    private Vector2 m_Origin;
    public Vector2 origin { get { return m_Origin; } set { m_Origin = value; } }
    public ClockwiseComparer(Vector2 origin)
    {
        m_Origin = origin;
    }
    public int Compare(Vector2 first, Vector2 second)
    {
        return IsClockwise(first, second, m_Origin);
    }
    public static int IsClockwise(Vector2 first, Vector2 second, Vector2 origin)
    {
        if (first == second)
            return 0;

        Vector2 firstOffset = first - origin;
        Vector2 secondOffset = second - origin;

        float angle1 = Mathf.Atan2(firstOffset.x, firstOffset.y);
        float angle2 = Mathf.Atan2(secondOffset.x, secondOffset.y);

        if (angle1 < angle2)
            return -1;

        if (angle1 > angle2)
            return 1;

        // Check to see which point is closest
        return (firstOffset.sqrMagnitude < secondOffset.sqrMagnitude) ? 1 : -1;
    }
}
