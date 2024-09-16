using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IslandGenerationV2 : MonoBehaviour
{
    public int islandSeed;
    public Dictionary<string, bool> islandsGenerated= new Dictionary<string, bool>();

    // Create a random seed for all the island properties.
    void SetIslandSeed()
    {
        islandSeed = Random.Range(0, int.MaxValue);
        Random.InitState(islandSeed);
    }
    
    void Generate()
    {
        // Temporary hard coding! This will be replaced with a real grass generated function eventually.
        Material grassMaterial = Resources.Load("Materials/Grass") as Material;
        // Firstly set the Random.seed using the SetIslandSeed function.
        SetIslandSeed();
        // Create a top level gameObject called Island that holds some general island stats.
        GameObject Island = new GameObject();
        Island.name = "Island";
        Island.AddComponent<IslandInfoV2>();
        Island.GetComponent<IslandInfoV2>().IslandSize = Random.Range(50, 100);

        // Function to decide if it is one big island or a collection of islands.
        void IslandSizing()
        {
            int numberOfMasses = Random.Range(1, 5);
            // Add a gameObject to the IslandList equal to the numberOfMasses and add the IslandProperties component to each.
            for (int i = 0; i < numberOfMasses; i++)
            {
                GameObject newIsland = new GameObject();
                newIsland.name = "Island " + i;
                newIsland.AddComponent<IslandProperties>();
                Island.GetComponent<IslandInfoV2>().IslandObjects.Add(newIsland);
                // Make the newly created GameObject a child of the Island GameObject.
                Island.GetComponent<IslandInfoV2>().IslandObjects[i].transform.parent = Island.transform;
                // if index of the island is not 0, then set the IsSubIsland bool to true.
                if (i != 0)
                {
                    Island.GetComponent<IslandInfoV2>().IslandObjects[i].GetComponent<IslandProperties>().IsSubIsland = true;
                }
                islandsGenerated.Add(newIsland.name, false);
            }

        }
        
        // Generate the actual island mesh, will run this for each island in the IslandList.
        void GenerateIslandMesh(GameObject island)
        {
            // Add empty MeshFilter and MeshRenderer components to the island if it doesn't have one already.
            if (island.GetComponent<MeshFilter>() == null && island.GetComponent<MeshRenderer>() == null)
            {
                island.AddComponent<MeshFilter>();
                island.AddComponent<MeshRenderer>();
            }


            float islandScaleMultiplyer = 1.0f;
            // If the island is a sub island, then set the islandScaleMultiplyer to a random float between 0.25 and 0.75.
            if (island.GetComponent<IslandProperties>().IsSubIsland == true)
            {
                islandScaleMultiplyer = Random.Range(0.25f, 0.5f);
            }

            // Randomly set the bounds of the island.
            Vector2 islandBounds = new Vector2(Island.GetComponent<IslandInfoV2>().IslandSize * islandScaleMultiplyer, Island.GetComponent<IslandInfoV2>().IslandSize * islandScaleMultiplyer);
            // Randomly generate the number of vertex points the island will have.
            int islandVertexCount = Random.Range(5, 15);
            // List of island vertices.
            Vector2[] islandVertices = new Vector2[islandVertexCount + 1];
            // 3D Vector list of the island vertices.
            Vector3[] islandVertices3D = new Vector3[islandVertexCount + 1];
            // Set the rest of the vertices to be randomly generated within the islandBounds.
            for (int i = 0; i < islandVertexCount; i++)
            {
                Vector2 vertex = new Vector2(Random.Range(0, islandBounds.x), Random.Range(0, islandBounds.y));
                islandVertices[i] = vertex;
                islandVertices3D[i] = new Vector3(vertex.x, vertex.y, 0f);
            }

            // Create a new island mesh.
            Mesh islandMesh = new Mesh();
            islandMesh.vertices = islandVertices3D;
            islandMesh.RecalculateBounds();

            // Sort the mainIslandPoints clockwise.
            Vector2 islandCenterPoint = new Vector2(islandMesh.bounds.center.x, islandMesh.bounds.center.y);
            System.Array.Sort(islandVertices, new ClockwiseComparer(islandCenterPoint));

            // List of halfway points between the main island vertices.
            Vector2[] halfwayPoints = new Vector2[islandVertices.Length];

            // Get the halfway points between the main island vertices, put them in the halfwayPoints list.
            for (int i = 0; i < islandVertices.Length; i++)
            {
                if (i < islandVertices.Length - 2)
                {
                    float distanceBetweenVecs = Vector2.Distance(islandVertices[i], islandVertices[i + 1]);
                    Vector2 halfwayPoint = Vector2.Lerp(islandVertices[i], islandVertices[i + 1], (distanceBetweenVecs / 2) / (islandVertices[i] - islandVertices[i + 1]).magnitude);
                    halfwayPoints[i] = halfwayPoint;
                }
                else
                {
                    float distanceBetweenVecs = Vector2.Distance(islandVertices[i], islandVertices[0]);
                    Vector2 halfwayPoint = Vector2.Lerp(islandVertices[i], islandVertices[0], (distanceBetweenVecs / 2) / (islandVertices[i] - islandVertices[0]).magnitude);
                    halfwayPoints[i] = halfwayPoint;
                }
            }
            // Add the last halfway point (between the last and first vertex) to the halfwayPoints list.
            float distanceBetweenVecsLast = Vector2.Distance(islandVertices[islandVertices.Length - 1], islandVertices[0]);
            Vector2 lastHalfwayPoint = Vector2.Lerp(islandVertices[islandVertices.Length - 1], islandVertices[0], (distanceBetweenVecsLast / 2) / (islandVertices[islandVertices.Length - 1] - islandVertices[0]).magnitude);
            halfwayPoints[islandVertices.Length - 1] = lastHalfwayPoint;

            // int for the number of points between two main island vertices (makes the curve more smooth).
            int curvePoints = 15;
            // List of the curve points between the main island vertices. Using List<Vector2> because we don't know what the size of the list will be and C# be like that...
            List<Vector2> curvePointsList = new List<Vector2>();
            for (int i = 0; i < islandVertices.Length - 1; i++)
            {
                if (i < islandVertices.Length - 2)
                {
                    for (int x = 0; x < curvePoints; x++)
                    {
                        float t = x / (float)curvePoints;
                        Vector2 position = new IslandTools().CalculateQuadraticBezierPoint(t, halfwayPoints[i], islandVertices[i + 1], halfwayPoints[i + 1]);
                        curvePointsList.Add(position);
                    }
                }
                else
                {
                    for (int x = 0; x < curvePoints; x++)
                    {
                        float t = x / (float)curvePoints;
                        Vector2 position = new IslandTools().CalculateQuadraticBezierPoint(t, halfwayPoints[i], islandVertices[0], halfwayPoints[0]);
                        curvePointsList.Add(position);
                    }
                }
            }
            // curvePointsList.Add(curvePointsList[0]);
            // Convert the curve points list to an array.
            Vector2[] curvePointsArray = curvePointsList.ToArray();
            Vector2[] mainIslandPointsSorted = curvePointsArray;
            // Put the sorted main island points into a new Vector3 list.
            Vector3[] islandPointsSorted3D = new Vector3[mainIslandPointsSorted.Length];
            List<Vector3> mainIslandPointsSorted3DList = new List<Vector3>();
            for (int i = 0; i < mainIslandPointsSorted.Length; i++)
            {
                islandPointsSorted3D[i] = new Vector3(mainIslandPointsSorted[i].x, mainIslandPointsSorted[i].y, 0f);
                mainIslandPointsSorted3DList.Add(new Vector3(mainIslandPointsSorted[i].x, mainIslandPointsSorted[i].y, 0f));
            }

            // Rotate the curve points list by a random amount.
            new IslandTools().RotateIsland(islandCenterPoint, islandPointsSorted3D, new Vector3(0f, 0f, Random.Range(0, 360)));

            // EXPERIMENTAL //
            // First island point should have either the x value or y value of the second island point.
            if (Random.Range(0, 2) == 0)
            {
                islandPointsSorted3D[0] = new Vector3(islandPointsSorted3D[1].x, islandPointsSorted3D[0].y, 0f);
            }
            else
            {
                islandPointsSorted3D[0] = new Vector3(islandPointsSorted3D[0].x, islandPointsSorted3D[1].y, 0f);
            }
            // Every other point should randomly have the x value of y each halfway point be the same as the previous island point or the next island point.
            for (int i = 0; i < islandPointsSorted3D.Length - 1; i++)
            {
                Vector2 randomXY = new Vector2();
                if (Random.Range(0, 2) == 0)
                {
                    randomXY = new Vector2(0, 1);
                }
                else
                {
                    randomXY = new Vector2(1, 0);
                }
                if (i % 2 == 0 && i != 0)
                {
                    Vector3 newIslandPoint = new Vector3(islandPointsSorted3D[i].x, islandPointsSorted3D[i].y, 0f);
                    if (randomXY.x == 0)
                    {
                        newIslandPoint = new Vector3(islandPointsSorted3D[i - 1].x, islandPointsSorted3D[i + 1].y, 0f);
                    }
                    if (randomXY.y == 0)
                    {
                        newIslandPoint = new Vector3(islandPointsSorted3D[i + 1].x, islandPointsSorted3D[i - 1].y, 0f);
                    }
                    islandPointsSorted3D[i] = newIslandPoint;
                }
            }

            // The last point should have either the x value or y value of the previous island point or the next island point.
            if (Random.Range(0, 2) == 0)
            {
                islandPointsSorted3D[islandPointsSorted3D.Length - 1] = new Vector3(islandPointsSorted3D[islandPointsSorted3D.Length - 2].x, islandPointsSorted3D[0].y, 0f);
            }
            else
            {
                islandPointsSorted3D[islandPointsSorted3D.Length - 1] = new Vector3(islandPointsSorted3D[0].x, islandPointsSorted3D[islandPointsSorted3D.Length - 2].y, 0f);
            }
            // Put all the islandPointsSorted3D back into the mainIslandPointsSorted 2D array.
            for (int i = 0; i < islandPointsSorted3D.Length; i++)
            {
                mainIslandPointsSorted[i] = new Vector2(islandPointsSorted3D[i].x, islandPointsSorted3D[i].y);
            }

            // Bevel the corners of the islands.
            // EXPERIMENTAL //


            // Triangulate the new curve points.
            Triangulator triangulator = new Triangulator(mainIslandPointsSorted);
            int[] mainIslandIndices = triangulator.Triangulate();

            // Set the island mesh vertices to the mainIslandPoints list.
            islandMesh.vertices = islandPointsSorted3D;
            // Set the island mesh triangles to the mainIslandIndices list.
            islandMesh.triangles = mainIslandIndices;
            // Set the island mesh UVs.
            islandMesh.SetUVs(0, islandPointsSorted3D);
            // Set the island mesh normals.
            islandMesh.RecalculateNormals();
            // Set the island mesh bounds.
            islandMesh.RecalculateBounds();

            // Set the island mesh to the island gameobject.
            island.GetComponent<MeshFilter>().mesh = islandMesh;
            // TEMPORARY - set the island mest material to the default grass material.
            island.GetComponent<MeshRenderer>().material = grassMaterial;

            // Check if the island is missing a triangle with the TriangleScan function.
            TriangleScanAndEdgeCollider2D(island);

        }
        
        // Initial positioning of the island (at origin 0, 0, 0).
        void InitPositionIslands(List<GameObject> islandGameObjects)
        {
            // Start by putting all the islands at the same position.
            for (int i = 0; i < islandGameObjects.Count; i++)
            {
                islandGameObjects[i].transform.position = new Vector3(0f, 0f, 0f);
            }
        }
        
        // Get EdgeCollider2D from the edge points of the provided mesh.
        Vector2[] CreateEdgeCollider2DPoints(Mesh mesh)
        {
            // Convert the mesh vertices to Vector2s.
            Vector2[] edgePoints = new Vector2[mesh.vertices.Length];
            for (int x = 0; x < mesh.vertices.Length; x++)
            {
                edgePoints[x] = new Vector2(mesh.vertices[x].x, mesh.vertices[x].y);
            }
            return edgePoints;
        }

        // Function to scan for a missing triangle in the provided mesh and regenerate the mesh if it's missing.
        void TriangleScanAndEdgeCollider2D(GameObject gameObject)
        {
            // Check if the provided mesh has a missing triangle.
            if (gameObject.GetComponent<MeshFilter>().mesh.triangles.Length / 3 != gameObject.GetComponent<MeshFilter>().mesh.vertices.Length - 4)
            {
                GenerateIslandMesh(gameObject);
            }
            else
            {
                gameObject.AddComponent<EdgeCollider2D>();
                gameObject.GetComponent<EdgeCollider2D>().points = CreateEdgeCollider2DPoints(gameObject.GetComponent<MeshFilter>().mesh);
                // Mark the gameObject as finished being generated in the public islandGenerated dict.
                MarkIslandGenerated(gameObject);
            }
        }
        
        // Mark the islandMesh as generated in the islandsGenerated dictionary.
        void MarkIslandGenerated(GameObject island)
        {
            islandsGenerated[island.name] = true;
        }

        // Generate the island's name
        string GenerateName()
        {
            TextAsset islandNames = Resources.Load<TextAsset>("Names/Islands/IslandNames");
            string _islandName = "";
            string[] islandPrefixes = { "Isle of ", "Island of ", "Shore of " };
            string[] islandSuffixes = { " Isle", " Island", " Shores", " Bay", " Reef", " Rock", " Point" };
            string[] islandNameList = islandNames.text.Split('\n');
            int islandNameSyllables = Random.Range(2, 6);
            var islandPrefixSuffixBool = new System.Random();

            // Probability of adding a random prefix is 25%.
            if (islandPrefixSuffixBool.Next(100) < 25)
            {
                _islandName += islandPrefixes[Random.Range(0, islandPrefixes.Length)];
            }
            for (int i = 0; i < islandNameSyllables; i++)
            {
                var islandSyllable = islandNameList[Random.Range(0, islandNameList.Length)];
                if (i == 0)
                {
                    // Capitalize first syllable.
                    islandSyllable = islandSyllable.ToCharArray()[0].ToString().ToUpper() + islandSyllable.Substring(1);
                }
                _islandName += islandSyllable;
            }
            // Probability of adding a random suffix is 25%.
            if (islandPrefixSuffixBool.Next(100) < 25)
            {
                _islandName += islandSuffixes[Random.Range(0, islandSuffixes.Length)];
            }
            return _islandName;
        }

        // Generate the island's weather.
        string GenerateWeather()
        {
            List<string> weatherList = new List<string>{ "Sunny", "Rainy", "Windy", "Cloudy", "Foggy", "Stormy" };
            return weatherList[Random.Range(0, weatherList.Count)];
        }

        // Decide if the island has rivers.
        bool HasRivers()
        {
            if (Random.Range(0, 100) < 50)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        // Decide if the island has ponds.
        bool HasPonds()
        {
            if (Random.Range(0, 100) < 50)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        // Decide if the island has mountains.
        bool HasMountains()
        {
            if (Random.Range(0, 100) < 50)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        // Decide if the island has cliffs.
        bool HasCliffs()
        {
            if (Random.Range(0, 100) < 50)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        // Decide the island's tree level.
        string GenerateTreeLevel()
        {
            List<string> treeLevelList = new List<string> { "None", "Low", "Medium", "Thick" };
            return treeLevelList[Random.Range(0, treeLevelList.Count)];
        }

        // Decide the island's grass level.
        string GenerateGrassLevel()
        {
            List<string> grassLevelList = new List<string> { "None", "Low", "Medium", "Thick" };
            return grassLevelList[Random.Range(0, grassLevelList.Count)];
        }

        // Decide if the island has a volcano.
        bool HasVolcano()
        {
            if (Random.Range(0, 100) < 50)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        // Get the area of the island.
        float GetArea(Mesh islandMesh)
        {
            float area = 0f;
            Vector3[] islandVertices = islandMesh.vertices;
            area = new IslandTools().CalculateArea(islandVertices);
            return area;
        }
                
        // Generate the island's population.
        int GeneratePopulation()
        {
            // Get overall area of all of the island masses.
            float TotalIslandArea = 0f;
            for (int i = 0; i < Island.GetComponent<IslandInfoV2>().IslandObjects.Count; i++)
            {
                TotalIslandArea += Island.GetComponent<IslandInfoV2>().IslandObjects[i].GetComponent<IslandProperties>().IslandArea;
                
            }
            // Clamp between 50 and 300 people.
            return Mathf.Clamp((int)TotalIslandArea / 50, 50, 300);
        }

        // Check if all the islands have been generated.
        bool AllIslandsGenerated()
        {
            foreach (KeyValuePair<string, bool> island in islandsGenerated)
            {
                if (!island.Value)
                {
                    return false;
                }
            }
            return true;
        }

        // If all the islands have been generated, Add the IslandPlacement component to this gameObject.
        void AddIslandPlacement(GameObject island)
        {
            if (AllIslandsGenerated())
            {
                island.AddComponent<IslandPlacement>();
            }
        }

        // Set the island's name.
        Island.GetComponent<IslandInfoV2>().IslandName = GenerateName();
        
        // Set the island's weather.
        Island.GetComponent<IslandInfoV2>().Weather = GenerateWeather();
        
        // Decide the island sizing.
        IslandSizing();
        
        // For each island in the islandObjects list, run GenerateIslandMesh for that island and get the island's area,
        // also populate the island properties (except IsSubIsland bool).
        for (int i = 0; i < Island.GetComponent<IslandInfoV2>().IslandObjects.Count; i++)
        {
            GenerateIslandMesh(Island.GetComponent<IslandInfoV2>().IslandObjects[i]);
            Island.GetComponent<IslandInfoV2>().IslandObjects[i].GetComponent<IslandProperties>().IslandArea = GetArea(Island.GetComponent<IslandInfoV2>().IslandObjects[i].GetComponent<MeshFilter>().mesh);
            Island.GetComponent<IslandInfoV2>().IslandObjects[i].GetComponent<IslandProperties>().Rivers = HasRivers();
            Island.GetComponent<IslandInfoV2>().IslandObjects[i].GetComponent<IslandProperties>().Ponds = HasPonds();
            Island.GetComponent<IslandInfoV2>().IslandObjects[i].GetComponent<IslandProperties>().Mountains = HasMountains();
            Island.GetComponent<IslandInfoV2>().IslandObjects[i].GetComponent<IslandProperties>().Cliffs = HasCliffs();
            Island.GetComponent<IslandInfoV2>().IslandObjects[i].GetComponent<IslandProperties>().TreeLevel = GenerateTreeLevel();
            Island.GetComponent<IslandInfoV2>().IslandObjects[i].GetComponent<IslandProperties>().GrassLevel = GenerateGrassLevel();
            Island.GetComponent<IslandInfoV2>().IslandObjects[i].GetComponent<IslandProperties>().Volcano = HasVolcano();
        }
        
        // Generate the island's population.
        Island.GetComponent<IslandInfoV2>().Population = GeneratePopulation();
        
        // Position all the islands in the scene.
        InitPositionIslands(Island.GetComponent<IslandInfoV2>().IslandObjects);
        AddIslandPlacement(Island);

    }

    void Start()
    {
        Generate();
    }
}
