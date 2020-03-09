using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HumanAI : MonoBehaviour
{
    Vector2 speed;
    bool moving;
    float waitTime;
    float moveTime;
    Vector2 randomDir;
    int posOrNeg;
    GameObject outline;
    GameObject outlineTwo;

    void Start()
    {
        float randSpeed = Random.Range(0.0f, 3.0f);
        speed = new Vector2(randSpeed, randSpeed);
        waitTime = Random.Range(1.0f, 5.0f);
        moveTime = 0.0f;
        randomDir = new Vector2(Random.Range(-1, 1), Random.Range(-1, 1));
        posOrNeg = Random.Range(0, 2);
    }

    private Vector2 movement;

    private void OnMouseEnter()
    {
        outline = new GameObject();
        outlineTwo = new GameObject();
        outlineTwo.AddComponent<SpriteRenderer>().sprite = Resources.Load<Sprite>("Glow");
        outlineTwo.transform.position = this.transform.position;
        outlineTwo.transform.localScale = new Vector3(0.25f, 0.25f, 1.0f);
        outline.transform.parent = GameObject.Find("Glow Effects").transform;
        outlineTwo.transform.parent = GameObject.Find("Glow Effects").transform;
        outline.name = "Hover Outline";
        outlineTwo.name = "Hover Glow";
        outline.AddComponent<SpriteRenderer>().sprite = this.GetComponent<SpriteRenderer>().sprite;
        outline.transform.localScale = new Vector3(1.5f, 1.5f, 1.0f);
        outline.GetComponent<SpriteRenderer>().color = new Color(1.0f, 0.75f, 0.0f);
        outline.transform.position = this.transform.position;
        outline.GetComponent<SpriteRenderer>().sortingOrder = 5;
        outlineTwo.GetComponent<SpriteRenderer>().sortingOrder = 5;
    }

    private void OnMouseExit()
    {
        if (outline != null)
        {
            Destroy(outline);
            Destroy(outlineTwo);
        }
    }

    void Update()
    {
        if (outline != null)
        {
            outlineTwo.transform.position = this.transform.position;
            outline.transform.position = this.transform.position;
        }
        while (moving == true)
        {
            if (moveTime > 0.0f)
            {
                movement = new Vector2(speed.x * randomDir.x, speed.y * randomDir.y);
                moveTime -= Time.deltaTime;
            }
            else
            {
                moveTime = Random.Range(1.0f, 5.0f);
                moving = false;
            }
        }
        if (waitTime > 0.0f)
        {
            waitTime -= Time.deltaTime;
        }
        else
        {
            waitTime = Random.Range(1.0f, 5.0f);
            randomDir = new Vector2(Random.Range(-1, 1), Random.Range(-1, 1));
            posOrNeg = Random.Range(0, 2);
            moving = true;
        }

    }

    void FixedUpdate()
    {
        if (posOrNeg == 0)
        {
            GetComponent<Rigidbody2D>().velocity = movement;
        }
        else
        {
            GetComponent<Rigidbody2D>().velocity = -movement;
        }
    }
}
