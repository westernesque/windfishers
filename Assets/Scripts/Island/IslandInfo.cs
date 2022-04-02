using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IslandInfo : MonoBehaviour
{
    // Randomly generated, pulls from the IslandNames.txt file.
    public string IslandName;
    
    // Overall island size, including sub-islands.
    public int IslandSize;

    // Randomly generated, pulls from WeatherOptions list.
    public string Weather;

    // Random in range, dependent on the IslandSize variable.
    public int Population;
    
    public bool Rivers;
    public bool Ponds;
    public bool Mountains;
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
            Population = Random.Range(IslandSize / 4, IslandSize / 2);
        }

        GenerateIslandName();
        GenerateIslandSize();
        GenerateWeather();
        GeneratePopulation();
    }

    void Start()
    {
        PopulateIslandInfo();
    }
}
