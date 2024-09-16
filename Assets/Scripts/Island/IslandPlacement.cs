using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IslandPlacement : MonoBehaviour
{
    // Repositions the islands relative to the first island in the island list (IslandInfo.IslandObjects[0]), if the edge collider is touching another island.
    void PositionIslands()
    {
        bool islandsTouching = true;
        List<GameObject> islands = this.GetComponent<IslandInfoV2>().IslandObjects;

        if (islandsTouching == false)
        {
            Destroy(this);
        }
        for (int i = 1; i < islands.Count; i++)
        {
            for (int x = 0; x < islands.Count; x++)
            {
                if (i != x)
                {
                    if (islands[i].GetComponent<EdgeCollider2D>().bounds.Intersects(islands[x].GetComponent<EdgeCollider2D>().bounds))
                    {
                        islands[i].transform.position = new Vector3(islands[i].transform.position.x + Random.Range(-10f, 10f), islands[i].transform.position.y + Random.Range(-10f, 10f), 0f);
                    }
                }
            }
        }
    }

    // Update is called once per frame
    void Update()
    {
        PositionIslands();
    }
}
