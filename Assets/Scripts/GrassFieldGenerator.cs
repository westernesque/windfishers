using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GrassFieldGenerator : MonoBehaviour
{
    public int fieldSize = 15;
    public int rowOrderInt = 1;
    public bool firstRow = true;
    GameObject[] WheatGameObjects;
    public Dictionary<Vector2, int> WheatPositions;
    public Dictionary<int, GameObject> WheatList;
    //int WheatId = 0;
    public void Awake()
    {
        WheatGameObjects = Resources.LoadAll<GameObject>("Prefabs/Foliage/Wheat");
    }
    public void AddGrass()
    {
        Debug.Log("Start Add Grass");
        // Step One: pick 1 or more random grass triangles
        // int numberOfFields = Random.Range(0, 6);
        // for (int i = 0; 0 < numberOfFields; i++)

        var ChosenGrassTriangle = Random.Range(0, GameObject.Find("Island").GetComponent<IslandGenerator>().grassTriangles.Count);
        List<Vector2> grassTri = GameObject.Find("Island").GetComponent<IslandGenerator>().grassTriangles[ChosenGrassTriangle];
        //WheatPositions = new Dictionary<Vector2, int>();
        //WheatList = new Dictionary<int, GameObject>();
        // Step Two: pick random field size (0, 5).
        // in fieldSize = Random.Range(0, 6);

        // Step Three: pick a random point in the triangle.
        Vector3 grassStartPoint = GetRandomPoint(grassTri[0], grassTri[1], grassTri[2]);
        // Step Four: instantiate wheat at point.
        //var wheat = Instantiate(wheatPrefab, new Vector3(grassStartPoint.x, grassStartPoint.y, -1.0f), transform.rotation);
        //wheat.transform.parent = this.transform;
        // Step Five: if wheat bounds is touching island edge collider then destroy() it.

        // Step Six: if instantiated wheat is not destroyed then instantiate a wheat to the left and right (loop for field size?)
        //float xOffset = wheat.GetComponent<SpriteRenderer>().bounds.size.x;
        //float yOffset = wheat.GetComponent<SpriteRenderer>().bounds.size.y;
        //float xOffSetBounds = wheat.GetComponent<SpriteRenderer>().bounds.size.x;
        //float yOffSetBounds = wheat.GetComponent<SpriteRenderer>().bounds.size.y;
        //var wheat2 = Instantiate(wheatPrefab, new Vector3(grassStartPoint.x + xOffset, grassStartPoint.y, -1.0f), transform.rotation);
        //wheat2.transform.parent = this.transform;
        // Step Seven: destroy() wheat if not in triangle or is touching island edge collider.

        // Step Eight: loop and do all that for however many rows (field size)
        //bool firstRow = true;
        for (int i = 0; i < fieldSize; i++)
        {
            //var wheat = Instantiate(wheatPrefab, new Vector3(grassStartPoint.x, grassStartPoint.y, -1.0f), transform.rotation);
            //wheat.transform.parent = this.transform;
            float xOffset = 0.0f;
            float yOffset = 0.0f;
            float xOffSetBounds = 0.0f;
            float yOffSetBounds = 0.0f;
            //int rowSortingOrder = 0;
            for (int x = 0; x < fieldSize; x++)
            {
                GameObject wheatPrefab = WheatGameObjects[Random.Range(0, WheatGameObjects.Length)];
                var wheat = Instantiate(wheatPrefab, new Vector3(grassStartPoint.x + xOffset, grassStartPoint.y + yOffset, 0.0f), transform.rotation);
                wheat.transform.parent = this.transform;
                wheat.GetComponent<SpriteRenderer>().sortingOrder = Mathf.RoundToInt(transform.position.y * 100f) * -1;
                //rowOrderInt = wheat.GetComponent<FoliageInfo>().fieldRow;
                xOffSetBounds = wheat.GetComponent<SpriteRenderer>().bounds.size.x;
                yOffSetBounds = wheat.GetComponent<SpriteRenderer>().bounds.size.y * 0.75f;
                xOffset += xOffSetBounds;
                if (firstRow == false)
                {
                    wheat.GetComponent<FoliageInfo>().fieldRow += rowOrderInt;
                }
                if (!InTriangle(new Vector2(wheat.transform.position.x, wheat.transform.position.y), grassTri[0], grassTri[1], grassTri[2]))
                {
                    Destroy(wheat);
                }
            }
            firstRow = false;
            rowOrderInt += 1;
            grassStartPoint.y += yOffSetBounds;
            yOffset += yOffSetBounds;
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
}
