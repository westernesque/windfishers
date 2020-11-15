using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class IslandGeneration : MonoBehaviour
{
    public Dictionary<string, bool> IslandProperties = new Dictionary<string, bool>() 
    { 
        { "IslandsPlaced", false },
        { "CliffsPlaced", false },
        { "HousesPlaced", false }, 
        { "FoliagePlaced", false }, 
        { "WavesPlaced", false } 
    };
    List<GameObject> IslandList = new List<GameObject>();

    public int IslandSize = 100;

    // Start is called before the first frame update
    void Start()
    {
        Generate();
    }

    void Generate()
    {
        // Runs everything.
        void StartGeneration()
        {
            CreateIslands();
            CreateCliffs();
            CreateHouses();
            CreateFoliage();
            CreateWaves();
        }

        // Creates and places islands. Includes all mesh work and any collider stuff.
        void CreateIslands()
        {
            // There will always be a "main island" even if there are no connecting islands.
            GameObject MainIsland = new GameObject();
            MainIsland.name = "Main Island";
            MainIsland.transform.parent = GameObject.Find("Island").transform;
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
            MainIsland.GetComponent<MeshFilter>().mesh = IslandTools.GenerateMesh(mainIslandVertices);
            MainIsland.GetComponent<MeshRenderer>().material = Resources.Load<Material>("Materials/Grass");
            MainIsland.GetComponent<EdgeCollider2D>().points = mainIslandVertices;
            
            // Curve mesh stuff.
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
            IslandTools.GenerateCurveMesh(edgeVertices, MainIsland);
            IslandList.Add(MainIsland);
            int numberOfConnectedIslands = UnityEngine.Random.Range(0, 4);
            if (numberOfConnectedIslands != 0)
            {
                for(int i = 0; i < numberOfConnectedIslands; i++)
                {
                    GameObject ConnectedIsland = new GameObject();
                    ConnectedIsland.name = "Connected Island " + i;
                    ConnectedIsland.transform.parent = GameObject.Find("Island").transform;
                    Vector2[] connectingIslandVertices = new Vector2[]
                    {
                        new Vector2(-UnityEngine.Random.Range(0, IslandSize / UnityEngine.Random.Range(1, 2)), 0),
                        new Vector2(-UnityEngine.Random.Range(0, IslandSize / UnityEngine.Random.Range(1, 2)), UnityEngine.Random.Range(0, IslandSize / UnityEngine.Random.Range(1, 2))),
                        new Vector2(0, UnityEngine.Random.Range(0, IslandSize / UnityEngine.Random.Range(1, 2))),
                        new Vector2(UnityEngine.Random.Range(0, IslandSize / UnityEngine.Random.Range(1, 2)), 0)
                    };
                    ConnectedIsland.AddComponent<MeshFilter>();
                    ConnectedIsland.AddComponent<MeshRenderer>();
                    ConnectedIsland.AddComponent<EdgeCollider2D>();
                    ConnectedIsland.GetComponent<MeshFilter>().mesh = IslandTools.GenerateMesh(connectingIslandVertices);
                    ConnectedIsland.GetComponent<EdgeCollider2D>().points = connectingIslandVertices;

                    // Edge stuff for the island curves.
                    List<Vector2> ci_edgeVertices = new List<Vector2>();
                    for (int c = 0; c < connectingIslandVertices.Length; c++)
                    {
                        ci_edgeVertices.Add(connectingIslandVertices[c]);
                    }
                    ci_edgeVertices.Add(connectingIslandVertices[0]);
                    Vector3[] ci_edgeVertices2D = new Vector3[ci_edgeVertices.Count];
                    for (int c = 0; c < ci_edgeVertices.Count; c++)
                    {
                        ci_edgeVertices2D[c] = ci_edgeVertices[c];
                    }
                    IslandTools.GenerateCurveMesh(ci_edgeVertices, ConnectedIsland);
                    ConnectedIsland.GetComponent<MeshRenderer>().material = Resources.Load<Material>("Materials/Grass");
                    IslandList.Add(ConnectedIsland);
                }
            }
        }

        // Creates and places cliffs. Includes all cliff collider stuff as well.
        void CreateCliffs()
        {
            foreach (Transform island in GameObject.Find("Island").transform)
            {
                // Decide if island will have cliff in first place.
                bool hasCliff = (UnityEngine.Random.Range(0, 2) == 0);
                if (hasCliff)
                {
                    Debug.Log("Generate cliff for " + island.gameObject.name);
                    Vector3[] islandVertices = island.gameObject.GetComponent<MeshFilter>().mesh.vertices;
                    int cliffJitter = UnityEngine.Random.Range(0, 5);
                    Vector2[] cliffVertices = new Vector2[(islandVertices.Length / 2) + 1 + cliffJitter];
                    List<Vector2> c_Vertices = new List<Vector2>();
                    for (int i = 0; i < (islandVertices.Length / 2) - cliffJitter; i++)
                    {
                        cliffVertices[i] = islandVertices[i];
                        c_Vertices.Add(cliffVertices[i]);

                    }
                    float distanceBetweenCliffEnds = Vector3.Distance(cliffVertices[0], cliffVertices[cliffVertices.Length - cliffJitter]);
                    float newPointDistance = distanceBetweenCliffEnds / cliffJitter;
                    Debug.Log(distanceBetweenCliffEnds);
                    for (int i = 0; i < cliffJitter; i++)
                    {
                        float cliffJitterYOffset = UnityEngine.Random.Range(-3f, 3f);
                        cliffVertices[(cliffVertices.Length - 1) - i] = new Vector2();
                    }
                    cliffVertices[cliffVertices.Length - 1] = islandVertices[0];
                    c_Vertices.Add(islandVertices[0]);
                    GameObject cliffGameObject = new GameObject();
                    cliffGameObject.name = "Cliff";
                    cliffGameObject.transform.parent = island.transform;
                    cliffGameObject.AddComponent<MeshFilter>();
                    cliffGameObject.AddComponent<MeshRenderer>();
                    cliffGameObject.AddComponent<EdgeCollider2D>();
                    cliffGameObject.GetComponent<MeshFilter>().mesh = IslandTools.GenerateMesh(cliffVertices);
                    cliffGameObject.transform.position = new Vector3(cliffGameObject.transform.position.x, cliffGameObject.transform.position.y + 5, cliffGameObject.transform.position.z);
                    cliffGameObject.GetComponent<EdgeCollider2D>().points = cliffVertices;
                    IslandTools.GenerateCurveMesh(c_Vertices, cliffGameObject);
                    cliffGameObject.GetComponent<MeshRenderer>().material = Resources.Load<Material>("Materials/Grass");
                }

            }
        }

        // Creates and randomly places islands. Should also include interior item placement (that are not power ups).
        void CreateHouses()
        {
            IslandProperties["HousesPlaced"] = true;
        }

        // Creates and randomly places foliage. Including grass, which will need to work with sorting layers.
        void CreateFoliage()
        {
            IslandProperties["FoliagePlaced"] = true;
        }

        // Add waves to the perimeter of all the islands in the IslandList.
        void CreateWaves()
        {
            IslandProperties["WavesPlaced"] = true;
        }

        StartGeneration();
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown("space"))
        {
            SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        }
        
        if (IslandProperties["IslandsPlaced"] == false)
        {
            Dictionary<GameObject, bool> islandsPlaced = new Dictionary<GameObject, bool>();
            foreach (GameObject island in IslandList)
            {
                Vector2 randomXYDir = new Vector2(UnityEngine.Random.Range(-5, 6), UnityEngine.Random.Range(-5, 6));
                Vector2 randomXYDirNeg = new Vector2(-randomXYDir.x, -randomXYDir.y);
                islandsPlaced.Add(island, false);
                island.transform.hasChanged = false;
                if (IslandList.Count > 1)
                {
                    foreach (GameObject otherIsland in IslandList)
                    {
                        
                        if (island.GetInstanceID() != otherIsland.GetInstanceID())
                        {
                            if (island.GetComponent<EdgeCollider2D>().bounds.Intersects(otherIsland.GetComponent<EdgeCollider2D>().bounds))
                            {
                                island.transform.position = new Vector3(island.transform.position.x + randomXYDir.x, island.transform.position.y + randomXYDir.y, island.transform.position.z);
                                otherIsland.transform.position = new Vector3(otherIsland.transform.position.x + randomXYDirNeg.x, otherIsland.transform.position.y + randomXYDirNeg.y, otherIsland.transform.position.z);
                            }
                        }
                    }
                }
                if (!island.transform.hasChanged)
                {
                    islandsPlaced[island] = true;
                }
            }
            if (!islandsPlaced.ContainsValue(false))
            {
                IslandProperties["IslandsPlaced"] = true;
                Camera.main.transform.position = new Vector3(IslandList[0].transform.position.x, IslandList[0].transform.position.y, Camera.main.transform.position.z);
            }
        }
        //if (IslandProperties["CliffsPlaced"] == false)
        //{
        //    foreach (GameObject island in IslandList)
        //    {
        //        foreach (Transform cliff in island.transform)
        //        {
        //            cliff.transform.position = island.transform.position;
        //        }
        //    }
        //}

        if (!IslandProperties.ContainsValue(false))
        {
            Destroy(this.gameObject);
        }
    }
}
