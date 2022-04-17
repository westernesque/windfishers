using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlaceCliffs : MonoBehaviour
{
    // Creates and randomly places cliffs at the edges of islands if IslandInfo.Cliffs is true.
    void GenerateCliffs(GameObject islandGameObject)
    {
        // Get the island mesh.
        Mesh islandMesh = islandGameObject.GetComponent<MeshFilter>().mesh;
        // Random dictionary/hash map for the cliff amount and int range.
        Dictionary<string, Vector2> cliffAmount = new Dictionary<string, Vector2>() {
            { "None", new Vector2(0, 0)},
            { "Light", new Vector2(0, 3) },
            { "Medium", new Vector2(3, 5) },
            { "Heavy", new Vector2(6, 10) }
        };
        // Pick a random point in the island mesh.
        var randomPoint = Vector3.Lerp(islandMesh.vertices[Random.Range(0,islandMesh.vertices.Length)], islandMesh.vertices[Random.Range(0, islandMesh.vertices.Length)], Random.value);
        // Place a debug circle at the random point.
        Debug.DrawLine(randomPoint, randomPoint + Vector3.up, Color.red, 10);
    }
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
