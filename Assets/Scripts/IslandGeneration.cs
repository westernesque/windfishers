using System;
using System.Linq;
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
            CreateMountains();
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
            MainIsland.GetComponent<MeshRenderer>().material.SetColor("_Color", new Color(0.9811321f, 0.8957226f, 0.644832f, 1.0f));
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
                        new Vector2(-UnityEngine.Random.Range(0, IslandSize / UnityEngine.Random.Range(2, 4)), UnityEngine.Random.Range(0, IslandSize / UnityEngine.Random.Range(2, 4))),
                        new Vector2(0, UnityEngine.Random.Range(0, IslandSize / UnityEngine.Random.Range(2, 4))),
                        new Vector2(UnityEngine.Random.Range(0, IslandSize / UnityEngine.Random.Range(2, 4)), UnityEngine.Random.Range(0, IslandSize / UnityEngine.Random.Range(2, 4))),
                        new Vector2(UnityEngine.Random.Range(0, IslandSize / UnityEngine.Random.Range(2, 4)), -UnityEngine.Random.Range(0, IslandSize / UnityEngine.Random.Range(2, 4))),
                        new Vector2(0, UnityEngine.Random.Range(0, -(IslandSize / UnityEngine.Random.Range(2, 4)))),
                        new Vector2(-UnityEngine.Random.Range(0, IslandSize / UnityEngine.Random.Range(2, 4)), -UnityEngine.Random.Range(0, IslandSize / UnityEngine.Random.Range(2, 4)))
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
                    //ci_edgeVertices.Add(connectingIslandVertices[0]);
                    Vector3[] ci_edgeVertices2D = new Vector3[ci_edgeVertices.Count];
                    for (int c = 0; c < ci_edgeVertices.Count; c++)
                    {
                        ci_edgeVertices2D[c] = ci_edgeVertices[c];
                    }
                    ci_edgeVertices.Add(connectingIslandVertices[0]);
                    IslandTools.GenerateCurveMesh(ci_edgeVertices, ConnectedIsland);
                    ConnectedIsland.GetComponent<MeshRenderer>().material = Resources.Load<Material>("Materials/Grass");
                    ConnectedIsland.GetComponent<MeshRenderer>().material.SetColor("_Color", new Color(0.9811321f, 0.8957226f, 0.644832f, 1.0f));
                    IslandList.Add(ConnectedIsland);
                }
            }
        }

        // Creates and places cliffs. Includes all cliff collider stuff as well.
        void CreateCliffs()
        {
            Vector3 LerpByDistance(Vector3 A, Vector3 B, float x)
            {
                Vector3 P = x * Vector3.Normalize(B - A) + A;
                return P;
            }

            foreach (Transform island in GameObject.Find("Island").transform)
            {
                // Decide if island will have cliff in first place.
                bool hasCliff = (UnityEngine.Random.Range(0, 2) == 0);
                if (hasCliff)
                {
                    GameObject Cliff = new GameObject();
                    Cliff.name = "Cliff";
                    Cliff.transform.parent = island.transform;
                    Cliff.AddComponent<MeshFilter>();
                    Cliff.AddComponent<MeshRenderer>();
                    Cliff.AddComponent<EdgeCollider2D>();
                    Vector3[] islandVertices = island.GetComponent<MeshFilter>().mesh.vertices;
                    int cliffJitter = UnityEngine.Random.Range(30, 50);
                    Vector3 closestPoint1 = new Vector3();
                    Vector3 closestPoint2 = new Vector3();
                    Vector3 referencePoint1 = new Vector3(island.GetComponent<MeshFilter>().mesh.bounds.min.x, island.GetComponent<MeshFilter>().mesh.bounds.center.y, 0.0f);
                    Vector3 referencePoint2 = new Vector3(island.GetComponent<MeshFilter>().mesh.bounds.max.x, island.GetComponent<MeshFilter>().mesh.bounds.center.y, 0.0f);
                    float pointDistance1 = Vector3.Distance(referencePoint1, islandVertices[0]);
                    float pointDistance2 = Vector3.Distance(referencePoint2, islandVertices[0]);
                    foreach (Vector3 point in islandVertices)
                    {
                        if (Vector3.Distance(point, referencePoint1) < pointDistance1)
                        {
                            closestPoint1 = point;
                            pointDistance1 = Vector3.Distance(point, referencePoint1);
                        }
                        if (Vector3.Distance(point, referencePoint2) < pointDistance2)
                        {
                            closestPoint2 = point;
                            pointDistance2 = Vector3.Distance(point, referencePoint2);
                        }
                    }

                    int currentIndexPos = Array.IndexOf(islandVertices, closestPoint1);
                    List<Vector2> _cliffVertices = new List<Vector2>();
                    List<Vector2> _cliffsideVertices = new List<Vector2>();
                    int pointsToAdd = 0;
                    if (closestPoint1.y > islandVertices[0].y)
                    {
                        pointsToAdd = Mathf.Abs(Array.IndexOf(islandVertices, closestPoint2) - Array.IndexOf(islandVertices, closestPoint1));
                    }
                    else
                    {
                        pointsToAdd = Mathf.Abs(islandVertices.Length - Array.IndexOf(islandVertices, closestPoint1)) + Array.IndexOf(islandVertices, closestPoint2);
                    }
                    for (int i = 0; i < pointsToAdd; i++)
                    {
                        if (currentIndexPos < islandVertices.Length)
                        {
                            _cliffVertices.Add(islandVertices[currentIndexPos]);
                            currentIndexPos += 1;
                        }
                        else
                        {
                            _cliffVertices.Add(islandVertices[0]);
                            currentIndexPos = 1;
                        }
                    }
                    float cliffJitterDistance = Vector3.Distance(_cliffVertices[0], _cliffVertices.Last()) / cliffJitter;
                    float cliffPointDistance = Vector3.Distance(_cliffVertices[0], _cliffVertices.Last());
                    GameObject debugCircle1 = Resources.Load<GameObject>("Prefabs/Debug Circle");
                    for (int i = 0; i < cliffJitter; i++)
                    {
                        Vector3 point = LerpByDistance(_cliffVertices[0], _cliffVertices.Last(), cliffPointDistance);
                        point = new Vector3(point.x, point.y + UnityEngine.Random.Range(-3.0f, 0.0f), point.z);
                        _cliffVertices.Add(point);
                        _cliffsideVertices.Add(point);
                        //Instantiate(debugCircle1, _cliffsideVertices[i], Quaternion.identity, Cliff.transform);
                        cliffPointDistance -= cliffJitterDistance;
                    }
                    int cliffsidePointsToAdd = _cliffsideVertices.Count;
                    for (int i = 0; i < cliffsidePointsToAdd; i++)
                    {
                        Vector3 point = new Vector3(_cliffsideVertices[(cliffsidePointsToAdd - 1) - i].x, _cliffsideVertices[(cliffsidePointsToAdd -1) - i].y - 2.5f, 0.0f);
                        _cliffsideVertices.Add(point);
                    }
                    _cliffVertices.Add(_cliffVertices[0]);
                    _cliffsideVertices.Add(_cliffsideVertices[0]);
                    Vector2[] cliffVertices = _cliffVertices.ToArray();
                    Vector2[] cliffsideVertices = _cliffsideVertices.ToArray();
                    Cliff.GetComponent<EdgeCollider2D>().points = cliffVertices;
                    IslandTools.GenerateCurveMesh(_cliffVertices, Cliff);
                    Cliff.transform.position = new Vector3(Cliff.transform.position.x, Cliff.transform.position.y + 2.5f, -1.0f);
                    Cliff.GetComponent<MeshRenderer>().material = Resources.Load<Material>("Materials/Grass");

                    // Generate cliffside texture.
                    GameObject Cliffside = new GameObject();
                    Cliffside.name = "Cliffisde";
                    Cliffside.transform.parent = Cliff.transform;
                    Cliffside.transform.position = new Vector3(Cliff.transform.position.x, Cliff.transform.position.y, Cliff.transform.position.z);
                    Cliffside.AddComponent<MeshFilter>();
                    Cliffside.AddComponent<MeshRenderer>();
                    Cliffside.GetComponent<MeshFilter>().mesh = IslandTools.GenerateMesh(cliffsideVertices);
                    Cliffside.transform.position = new Vector3(Cliff.transform.position.x, Cliff.transform.position.y, Cliff.transform.position.z);
                    Cliffside.GetComponent<MeshRenderer>().material = Resources.Load<Material>("Materials/Cliffside/Cliffside 02");

                    // 1. load all cliffside textures
                    Texture[] cliffsideTextures = Resources.LoadAll<Texture>("Materials/Cliffside");

                    // 2. get total distance between all cliff points.
                    float distanceBetweenCliffPoints = 0.0f;
                    for (int i = 0; i < cliffVertices.Length - 1; i++)
                    {
                        distanceBetweenCliffPoints += Vector2.Distance(cliffVertices[i], cliffVertices[i + 1]);
                    }
                    // debug stuff to be deleted later.
                    //GameObject cliffSprite = Resources.Load<GameObject>("Prefabs/Foliage/Cliffside/Cliffside 01");
                    ////Debug.Log("total cliff distance: " + distanceBetweenCliffPoints);
                    //// Debug.Log("size of cliff sprite.x: " + cliffSprite.GetComponent<SpriteRenderer>().bounds.size.x);
                    //int cliffSortingLayer = 0;
                    //for (int i = 0; i < cliffVertices.Length - 1; i++)
                    //{
                    //    float cliffSpriteDistance = Vector2.Distance(cliffVertices[i], cliffVertices[i + 1]) / (cliffSprite.GetComponent<SpriteRenderer>().bounds.size.x * 2.0f);
                    //    Debug.Log("cliffSpriteDistance: " + cliffSpriteDistance);
                    //    //int cliffSortingLayer = 0;
                    //    //Instantiate<GameObject>(cliffSprite, cliffVertices[i], Quaternion.identity, Cliffside.transform);
                    //    //Instantiate<GameObject>(cliffSprite, cliffVertices[i + 1], Quaternion.identity, Cliffside.transform);
                    //    for (float f = 0.0f; f < cliffSpriteDistance + cliffSprite.GetComponent<SpriteRenderer>().bounds.size.x * 2.0f; f += cliffSprite.GetComponent<SpriteRenderer>().bounds.size.x * 0.85f)
                    //    {
                    //        Vector3 point = LerpByDistance(cliffVertices[i], cliffVertices[i + 1], f);
                    //        point = new Vector3(point.x, point.y, Cliffside.transform.position.z);
                    //        GameObject cs = Instantiate<GameObject>(cliffSprite, point, Quaternion.identity, Cliffside.transform);
                    //        if (cliffVertices[i].y < cliffVertices[i + 1].y)
                    //        {
                    //            cliffSortingLayer -= 1;
                    //            cs.GetComponent<SpriteRenderer>().sortingOrder = cliffSortingLayer;
                    //        }
                    //        else if (cliffVertices[i].y > cliffVertices[i + 1].y)
                    //        {
                    //            cliffSortingLayer += 1;
                    //            cs.GetComponent<SpriteRenderer>().sortingOrder = cliffSortingLayer;
                    //        }

                    //    }
                        // for each i and i + 1, instantiate a cliff until next point is reached
                        // get distance between i and i + 1
                        //Vector2 point = LerpByDistance(cliffVertices[i], cliffVertices[i + 1], cliffSpriteDistance);
                        //Instantiate<GameObject>(cliffSprite, cliffVertices[i], Quaternion.identity, Cliffside.transform);
                        //Instantiate<GameObject>(cliffSprite, cliffVertices[i + 1], Quaternion.identity, Cliffside.transform);
                    }
                    // render a random cliff sprite
                    // combine meshes of those sprites
                }
            }
        }

        // Creates hills or mountains (only on cliffs). Includes all colliderstuff.
        void CreateMountains()
        {

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
