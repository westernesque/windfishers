using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CloudMovement : MonoBehaviour
{
    public int CloudCount = 0;
    public Vector2 WindDirection;
    public float WindSpeed;
    public GameObject CloudPrefab;
    public bool firstCloudsGenerated = false;
    public Vector2 IslandBounds;
    
    // Start is called before the first frame update
    void Start()
    {
        IslandBounds = GameObject.Find("Island").GetComponent<IslandGenerator>().IslandBounds;
        if (GameObject.Find("Island").GetComponent<IslandGenerator>().CloudCoverage.ToString() == "Low")
        {
            CloudCount = Random.Range(30, 50);
        }
        else if (GameObject.Find("Island").GetComponent<IslandGenerator>().CloudCoverage.ToString() == "Normal")
        {
            CloudCount = Random.Range(51, 80);
        }
        else if (GameObject.Find("Island").GetComponent<IslandGenerator>().CloudCoverage.ToString() == "Heavy")
        {
            CloudCount = Random.Range(81, 100);
        }
        else if (GameObject.Find("Island").GetComponent<IslandGenerator>().CloudCoverage.ToString() == "Extreme")
        {
            CloudCount = Random.Range(101, 150);
        }
        for (int i = 0; i < CloudCount; i++)
        {
            GenerateCloud();
        }
        firstCloudsGenerated = true;
    }

    // Update is called once per frame
    void Update()
    {
        foreach (Transform child in this.transform)
        {
            child.transform.position += new Vector3(WindSpeed * WindDirection.x * Time.deltaTime, WindSpeed * WindDirection.y * Time.deltaTime, 0.0f);
            if (child.transform.position.x > IslandBounds.x + 10.0f)
            {
                Destroy(child.gameObject);
            }
        }
        if (this.transform.childCount < CloudCount)
        {
            GenerateCloud();
        }
    }

    void GenerateCloud()
    {
        GameObject cloud = CloudPrefab;
        GameObject NewCloud;
        if (firstCloudsGenerated == false)
        {
            NewCloud = Instantiate(cloud, new Vector3(Random.Range(-IslandBounds.x - 15.0f, IslandBounds.x + 15.0f), Random.Range(-IslandBounds.y, IslandBounds.y), 0.0f), transform.rotation);
        }
        else
        {
            NewCloud = Instantiate(cloud, new Vector3((-IslandBounds.x * WindDirection.x) - 15.0f, Random.Range(-IslandBounds.y - 15.0f, IslandBounds.y + 15.0f), 0.0f), transform.rotation);
        }
        NewCloud.transform.parent = this.transform;
        NewCloud.GetComponent<SpriteRenderer>().sortingOrder = 5;
        NewCloud.GetComponent<SpriteRenderer>().color = new Color(255.0f, 255.0f, 255.0f, 0.5f);
    }
}
