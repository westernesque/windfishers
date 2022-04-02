using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IslandInfo : MonoBehaviour
{
    public string IslandName;
    public int IslandSize;
    public string Weather;
    public int Population;
    public bool Rivers; 
    public bool Ponds;
    public bool Mountains;
    public bool Cliffs;
    public int SubIslandCount;
    public string CloudLevel;
    public string TreeLevel;
    public string GrassLevel;
    public bool Volcano = false;

    void PopulateIslandInfo()
    {
        // Randomly generates the IslandName.
        void GenerateIslandName()
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
            IslandName = _islandName;
        }

        // Generate the IslandSize.
        void GenerateIslandSize()
        {
            IslandSize = Random.Range(500, 1000);
        }

        // Generate the Weather.
        void GenerateWeather()
        {
            string[] weatherList = { "Sunny", "Rainy", "Windy", "Cloudy", "Foggy", "Stormy" };
            Weather = weatherList[Random.Range(0, weatherList.Length)];
        }

        // Generate the Population.
        void GeneratePopulation()
        {
            Population = Random.Range(IslandSize / 5, IslandSize / 3);
        }

        // Generate if the island has rivers.
        void GenerateRivers()
        {
            if (Random.Range(0, 100) < 50)
            {
                Rivers = true;
            }
            else
            {
                Rivers = false;
            }
        }

        // Generate if the island has ponds.
        void GeneratePonds()
        {
            if (Random.Range(0, 100) < 50)
            {
                Ponds = true;
            }
            else
            {
                Ponds = false;
            }
        }

        // Generate if the island has mountains.
        void GenerateMountains()
        {
            if (Random.Range(0, 100) < 50)
            {
                Mountains = true;
            }
            else
            {
                Mountains = false;
            }
        }

        // Generate if the island has cliffs.
        void GenerateCliffs()
        {
            if (Random.Range(0, 100) < 50)
            {
                Cliffs = true;
            }
            else
            {
                Cliffs = false;
            }
        }

        // Generate if the island has sub-islands and how many.
        void GenerateSubIslands()
        {
            if (Random.Range(0, 100) < 50)
            {
                SubIslandCount = Random.Range(1, 5);
            }
            else
            {
                SubIslandCount = 0;
            }
        }

        // Generate the CloudLevel.
        void GenerateCloudLevel()
        {
            string[] cloudLevelList = { "Clear", "Cloudy", "Overcast", "Stormy" };
            CloudLevel = cloudLevelList[Random.Range(0, cloudLevelList.Length)];
        }

        // Generate the TreeLevel.
        void GenerateTreeLevel()
        {
            string[] treeLevelList = { "None", "Low", "Medium", "Thick" };
            TreeLevel = treeLevelList[Random.Range(0, treeLevelList.Length)];
        }

        // Generate the GrassLevel.
        void GenerateGrassLevel()
        {
            string[] grassLevelList = { "None", "Low", "Medium", "Thick" };
            GrassLevel = grassLevelList[Random.Range(0, grassLevelList.Length)];
        }

        // Generate if the island has a volcano.
        void GenerateVolcano()
        {
            if (Random.Range(0, 100) < 50)
            {
                Volcano = true;
            }
            else
            {
                Volcano = false;
            }
        }


        // Run all the island generation functions.
        GenerateIslandName();
        GenerateIslandSize();
        GenerateWeather();
        GeneratePopulation();
        GenerateRivers();
        GeneratePonds();
        GenerateMountains();
        GenerateCliffs();
        GenerateSubIslands();
        GenerateCloudLevel();
        GenerateTreeLevel();
        GenerateGrassLevel();
        GenerateVolcano();
    }

    void Start()
    {
        PopulateIslandInfo();
    }
}
