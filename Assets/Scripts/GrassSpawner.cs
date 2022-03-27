using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Timeline;

public class SpriteData
{
    public Vector3 pos;
    public Vector3 scale;
    public Quaternion rot;

    public SpriteData(Vector3 pos, Vector3 scale, Quaternion rot)
    {
        this.pos = pos;
        this.scale = scale;
        this.rot = rot;
    }
}

public class GrassSpawner : MonoBehaviour
{
    public int instances;
    public Vector3 maxPos;
    public Sprite sprite;
    public Material spriteMat;
    private List<List<SpriteData>> batches = new List<List<SpriteData>>();

    // Start is called before the first frame update
    void Start()
    {
        Debug.Log("TEST GRASS SPAWNER");
        int batchIndexNum = 0;
        List<SpriteData> currentBatch = new List<SpriteData>();
        for (int i = 0; i < instances; i++)
        {
            AddSprite(currentBatch, i);
            batchIndexNum++;
            if (batchIndexNum >= 1000)
            {
                batches.Add(currentBatch);
                currentBatch = BuildNewBatch();
                batchIndexNum = 0;
            }
        }
    }

    // Update is called once per frame
    void Update()
    {
        RenderBatches();
    }

    private void AddSprite(List<SpriteData> currentBatch, int i)
    {
        Vector3 position = new Vector3(Random.Range(-maxPos.x, maxPos.x), Random.Range(-maxPos.y, maxPos.y), 0.0f);
        currentBatch.Add(new SpriteData(position, new Vector3(1.0f, 1.0f, 1.0f), Quaternion.identity));
    }

    private List<SpriteData> BuildNewBatch()
    {
        return new List<SpriteData>();
    }

    private void RenderBatches()
    {
        foreach (var batch in batches)
        {
            Debug.Log("draw texture called");
            Graphics.DrawTexture(screenRect: Camera.main.rect, texture:sprite.texture, mat: spriteMat);
        }
    }
}
