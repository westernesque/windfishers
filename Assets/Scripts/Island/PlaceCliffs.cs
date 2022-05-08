using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlaceCliffs : MonoBehaviour
{
    // Bool that determines if the cliff(s) is placed.
    bool cliffPlaced = false;
    // Creates and randomly places cliffs at the edges of islands if IslandInfo.Cliffs is true.
    public void GenerateCliffs(GameObject islandGameObject)
    {
        // List of islands under the islandGameObject.
        List<GameObject> islands = new List<GameObject>();
        // Get the islandGameObject's children.
        foreach (Transform child in islandGameObject.transform)
        {
            // If the child is an island, add it to the list.
            if (child.gameObject.tag == "Island")
            {
                if (child.gameObject.name == "Main Island")
                {
                    islands.Add(child.gameObject);
                }
                else if (child.gameObject.name == "Sub Islands")
                {
                    foreach (Transform subIsland in child.transform)
                    {
                        islands.Add(subIsland.gameObject);
                    }
                }
            }
        }
        // For each island in the list, if the plateau bool is true, generate a plateau.
        foreach (GameObject island in islands)
        {
            // Random bool to determine if there will be a plateau on the island. Just one for now, but maybe up to 2 eventually?
            System.Random rand = new System.Random();
            bool plateau = rand.Next(0, 2) > 0;
            plateau = true;
            // If there is a plateau, run the CreatePlateau function.
            if (plateau && island.transform.childCount == 0)
            {
                // Add a plateau game object to the island.
                GameObject plateauGameObject = new GameObject("Plateau");
                // Make the plateauGameObject a child of the islandGameObject.
                plateauGameObject.transform.parent = island.transform;
                // Add a MeshRenderer to the plateau game object.
                plateauGameObject.AddComponent<MeshRenderer>();
                // Add a MeshFilter to the plateau game object.
                MeshFilter plateauMeshFilter = plateauGameObject.AddComponent<MeshFilter>();
                plateauMeshFilter.mesh = CreatePlateau(island.GetComponent<MeshFilter>().mesh);
                plateauGameObject.GetComponent<MeshRenderer>().material = Resources.Load("Materials/Grass") as Material;
            }
        }
        
        // Random dictionary/hash map for the cliff amount and int range.
        Dictionary<string, Vector2> cliffAmount = new Dictionary<string, Vector2>() {
            { "None", new Vector2(0, 0)},
            { "Light", new Vector2(0, 3) },
            { "Medium", new Vector2(3, 5) },
            { "Heavy", new Vector2(6, 10) }
        };
 
        // Function that creates a plateau
        Mesh CreatePlateau(Mesh islandMesh)
        {
            // Get a random vertice from the island mesh on one half of the island.
            // TODO: change this logic so that it gets a point closest to the left edge of the island bounds
            Vector3 randomPoint = islandMesh.vertices[islandMesh.vertices.Length / 4];
            Debug.Log("randomPoint: " + randomPoint);
            // Get the vertex of the random point.
            int randomPointIndex = System.Array.IndexOf(islandMesh.vertices, randomPoint);
            Debug.Log("Random point index: " + randomPointIndex);
            // List of vertices that will be used to create the plateau.
            List<Vector3> plateauPoints = new List<Vector3>();
            // Add the points to the list if they are not higher than the random point y.
            for (int i = randomPointIndex; i < islandMesh.vertices.Length; i++)
            {
                if (islandMesh.vertices[i].y > randomPoint.y)
                {
                    plateauPoints.Add(islandMesh.vertices[i]);
                }
            }
            // Add the first point to the list.
            plateauPoints.Add(plateauPoints[0]);
            // Vector2 lists of vertices that will be used to create the plateau.
            Vector2[] plateauPoints2D = new Vector2[plateauPoints.Count];
            // Vector3 lists of vertices that will be used to create the plateau.
            Vector3[] plateauPoints3D = new Vector3[plateauPoints.Count];
            // Add the points to the 2D list.
            for (int i = 0; i < plateauPoints.Count; i++)
            {
                plateauPoints2D[i] = new Vector2(plateauPoints[i].x, plateauPoints[i].y);
                plateauPoints3D[i] = new Vector3(plateauPoints[i].x, plateauPoints[i].y, 0f);
            }
            // Create a new mesh for the plateau.
            Mesh plateauMesh = new Mesh();
            // Triangulate the points.
            Triangulator p_triangulator = new Triangulator(plateauPoints2D);
            int[] plateauIndices = p_triangulator.Triangulate();

            // Set the vertices and triangles of the new mesh.
            plateauMesh.vertices = plateauPoints3D;
            plateauMesh.triangles = plateauIndices;
            // Set the UVs of the new mesh.
            plateauMesh.SetUVs(0, plateauPoints3D);
            // Recalculate the normals of the new mesh.
            plateauMesh.RecalculateNormals();
            // Set the mesh bounds.
            islandMesh.RecalculateBounds();
            Debug.Log("plateau Mesh object: " + plateauMesh);
            return plateauMesh;
        }
    }
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        // If the island(s) is generated (from the IslandGeneration script) and IslandInfo.Cliffs is true, run the GenerateCliffs function.
        if (cliffPlaced == false && this.gameObject.GetComponent<IslandGeneration>().islandsGenerated == true && this.gameObject.GetComponent<IslandInfo>().Cliffs == true)
        {    
            GenerateCliffs(this.gameObject);
            Debug.Log("Cliffs placed.");
            cliffPlaced = true;
        }
    }
}
