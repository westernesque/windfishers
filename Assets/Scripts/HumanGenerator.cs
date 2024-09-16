using System.Collections;   
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using UnityEngine.U2D;

public class HumanGenerator : MonoBehaviour
{
    public int HumanCount = 50;
    public Dictionary<int, GameObject> HumanList;
    int HIndex = 0;
    public GameObject ChosenWaldo;
    public string ClickedHuman;
    public int SpawnInHousePercentage = 10;
    public TextAsset namesList;
    public TextAsset skinTonesList;
    SpriteAtlas[] eyeList;
    SpriteAtlas[] noseList;
    SpriteAtlas[] hairList;

    private void Awake()
    {
    }

    // Start is called before the first frame update
    void Start()
    {
        HumanList = new Dictionary<int, GameObject>();
        Debug.Log("HumanList size: " + HumanList.Count);
        GameObject human = Resources.Load<GameObject>("Prefabs/Body 01/Human");
        eyeList = Resources.LoadAll<SpriteAtlas>("Prefabs/Body 01/Faces/Eyes");
        noseList = Resources.LoadAll<SpriteAtlas>("Prefabs/Body 01/Faces/Nose");
        hairList = Resources.LoadAll<SpriteAtlas>("Prefabs/Body 01/Hair");
        SpawnHuman(human);

        ChosenWaldo = HumanList[Random.Range(0, HumanList.Count)];
        GameObject.Find("Canvas/Bounty Info Button").GetComponent<BountyUI>().PopulateBountyUI();
        MoveHumansInside();
    }

    string GenerateName()
    {
        List<string> wordlist = new List<string>();
        foreach (string line in namesList.text.Split('\n'))
        {
            wordlist.Add(line);
        }
        var firstNameSyllables = Random.Range(1, 4);
        var lastNameSyllables = Random.Range(1, 4);
        string firstName = "";
        string lastName = "";

        for (int i = 0; i < firstNameSyllables; i++)
        {
            firstName += wordlist[Random.Range(0, wordlist.Count)];
        }
        for (int i = 0; i < lastNameSyllables; i++)
        {
            lastName += wordlist[Random.Range(0, wordlist.Count)];
        }
        char[] fn = firstName.ToCharArray();
        fn[0] = char.ToUpper(fn[0]);
        firstName = new string(fn);

        char[] ln = lastName.ToCharArray();
        ln[0] = char.ToUpper(ln[0]);
        lastName = new string(ln);

        return firstName + " " + lastName;
    }

    public void SpawnHuman(GameObject human)
    {
        var triangleVerts = GameObject.Find("Island").GetComponent<IslandGenerator>().islandMesh.vertices;
        var triangleIndices = GameObject.Find("Island").GetComponent<IslandGenerator>().islandMesh.triangles;
        var goodTriangles = GameObject.Find("Island").GetComponent<IslandGenerator>().humanTriangles;
        for (int h = 0; h < HumanCount; h++)
        {
            int randomTriangle = Random.Range(0, goodTriangles.Count);
            bool spawned = false;
            List<List<Vector2>> triangleList = new List<List<Vector2>>();
            for (int i = 0; i < triangleIndices.Length; i++)
            {
                if (i % 3 == 0 && i < triangleIndices.Length)
                {
                    var vert1 = triangleIndices[i];
                    var vert2 = triangleIndices[i + 1];
                    var vert3 = triangleIndices[i + 2];
                    triangleList.Add(new List<Vector2> { triangleVerts[vert1], triangleVerts[vert2], triangleVerts[vert3] });
                }
            }
            var chosenTriangle = goodTriangles[randomTriangle];
            //var chosenTriangle = triangleList[randomTriangle];
            Vector3 humanSpawnPoint = GetRandomPoint(chosenTriangle[0], chosenTriangle[1], chosenTriangle[2]);
            while (spawned == false)
            {
                if (InTriangle(humanSpawnPoint, chosenTriangle[0], chosenTriangle[1], chosenTriangle[2]))
                {
                    var newHuman = Instantiate(human, humanSpawnPoint, transform.rotation);
                    newHuman.name = GenerateName();
                    //newHuman.AddComponent<PositionRendererSorter>();
                    newHuman.GetComponent<SpriteRenderer>().color = PickSkinTone();
                    newHuman.GetComponent<HumanInfo>().face = MakeFace(newHuman);
                    HumanList.Add(HIndex, newHuman);
                    newHuman.transform.parent = this.transform;
                    HIndex += 1;
                    spawned = true;
                }
                else
                {
                    humanSpawnPoint = GetRandomPoint(chosenTriangle[0], chosenTriangle[1], chosenTriangle[2]);
                }
            }
        }
    }

    void MoveHumansInside()
    {
        List<GameObject> HumansInside = new List<GameObject>();
        int NumberOfHumansIndoors = 100 / SpawnInHousePercentage;
        List<GameObject> Houses = GameObject.Find("Island").GetComponent<IslandGenerator>().houseList;
        Debug.Log("LENGTH OF HOUSELIST(HOUSE): " + Houses.Count);
        for (int i = 0; i < NumberOfHumansIndoors; i++)
        {
            GameObject chosenInsideHuman = HumanList[Random.Range(0, HumanList.Count)];
            bool HumanAdded = false;
            while (HumanAdded == false)
            {
                if (HumansInside.Contains(chosenInsideHuman))
                {
                    chosenInsideHuman = HumanList[Random.Range(0, HumanList.Count)];
                    Debug.Log(chosenInsideHuman.name);
                }
                else
                {
                    HumansInside.Add(chosenInsideHuman);
                    HumanAdded = true;
                }
            }
            HumansInside[i].transform.position = Houses[Random.Range(0, Houses.Count - 1)].transform.position;
        }  
    }

    public Color PickSkinTone()
    {
        //string[] skinTones = System.IO.File.ReadAllLines("Assets/Resources/skin_tones.txt");
        List<string> skinTones = new List<string>();
        foreach (string line in skinTonesList.text.Split('\n'))
        {
            skinTones.Add(line);
            //Debug.Log(line);
        }
        string chosenSkinTone = skinTones[Random.Range(0, skinTones.Count)];
        var skinToneValue = chosenSkinTone.Split(',');
        //return new Color(r: float.Parse("0.5"), g: float.Parse("0.5"), b: float.Parse("0.5"));
        return new Color(r: float.Parse(skinToneValue[0]) / 255, g: float.Parse(skinToneValue[1]) / 255, b: float.Parse(skinToneValue[2]) / 255);
    }

    public List<SpriteAtlas> MakeFace(GameObject human)
    {
        int randomEyes = Random.Range(0, eyeList.Length);
        int randomNose = Random.Range(0, noseList.Length);
        int randomHair = Random.Range(0, hairList.Length);
        SpriteAtlas chosenEyes = eyeList[randomEyes];
        SpriteAtlas chosenNose = noseList[randomNose];
        SpriteAtlas chosenHair = hairList[randomHair];
        List<SpriteAtlas> face = new List<SpriteAtlas> { chosenNose, chosenEyes, chosenHair};
        return face;
    }

    public Vector3 GetRandomPoint(Vector2 vec1, Vector2 vec2, Vector2 vec3)
    {
        Vector3 min = GetMinPoint(vec1, vec2, vec3);
        Vector3 max = GetMaxPoint(vec1, vec2, vec3);
        return new Vector3(Random.Range(min.x, max.x), Random.Range(min.y, max.y), -1.5f);
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

