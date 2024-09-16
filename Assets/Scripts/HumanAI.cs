using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class HumanAI : MonoBehaviour
{
    public Vector2 speed;
    public bool moving;
    public bool hovered;
    public float waitTime;
    public float moveTime;
    Vector2 randomDir;
    int posOrNeg;
    public Animator humanAnimation;
    public Vector2 movement;
    string HumanName;
    public GameObject HumanInfoTMP;
    //Dictionary<Vector2, int> GrassPosList;

    void Start()
    {
        float randSpeed = Random.Range(0.2f, 1.0f);
        speed = new Vector2(randSpeed, randSpeed);
        waitTime = Random.Range(1.0f, 5.0f);
        moveTime = 0.0f;
        randomDir = new Vector2(Random.Range(-1, 1), Random.Range(-1, 1));
        posOrNeg = Random.Range(0, 2);
        //GrassPosList = GameObject.Find("Foliage").GetComponent<GrassFieldGenerator>().WheatPositions;
    }

    private void OnMouseEnter()
    {
        hovered = true;
        HumanName = this.gameObject.GetComponent<HumanInfo>().HumanName;
        HumanInfoTMP.transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = "name: " + HumanName.Replace("\r", "");
        HumanInfoTMP.SetActive(true);
    }

    private void OnMouseExit()
    {
        hovered = false;
        HumanInfoTMP.SetActive(false);
    }

/*    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.otherCollider.tag != "Grass - Wheat")
        {
            randomDir = -randomDir;
        }
        else
        {
            Debug.Log("Touching grass...");
        }
    }*/

    void Update()
    {
        if (hovered == false)
        {
            if (moving == true)
            {
                if (moveTime > 0.0f)
                {
                    movement = new Vector2(speed.x * randomDir.x, speed.y * randomDir.y);
                    moveTime -= Time.deltaTime;
                }
                else
                {
                    waitTime = Random.Range(1.0f, 5.0f);
                    moving = false;
                }
            }
            else
            {
                if (waitTime > 0.0f)
                {
                    movement = new Vector2(0.0f, 0.0f);
                    waitTime -= Time.deltaTime;
                }
                else
                {
                    moveTime = Random.Range(1.0f, 5.0f);
                    randomDir = new Vector2(Random.Range(-1, 1), Random.Range(-1, 1));
                    posOrNeg = Random.Range(0, 2);
                    moving = true;
                }
            }
        }
        else
        {
            movement = new Vector2(0.0f, 0.0f);
        }
        ChangeAnimation();
        //if (GrassPosList.ContainsKey(new Vector2(this.gameObject.transform.position.x, this.gameObject.transform.position.y)))
        //{
        //    int DestroyThis = GameObject.Find("Foliage").GetComponent<GrassFieldGenerator>().WheatPositions[new Vector2(this.gameObject.transform.position.x, this.gameObject.transform.position.y)];
        //    Destroy(GameObject.Find("Foliage").GetComponent<GrassFieldGenerator>().WheatList[DestroyThis]);
        //    Debug.Log("testinggrass");
        //}
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

    void ChangeAnimation()
    {
        if (Mathf.Sign(GetComponent<Rigidbody2D>().velocity.x) > 0 && GetComponent<Rigidbody2D>().velocity.x != 0.0f)
        {
            if (Mathf.Sign(GetComponent<Rigidbody2D>().velocity.y) > 0 && GetComponent<Rigidbody2D>().velocity.y != 0.0f)
            {
                humanAnimation.SetBool("Idle", false);
                humanAnimation.SetBool("Up", false);
                humanAnimation.SetBool("Down", false);
                humanAnimation.SetBool("UpRight", true);
                humanAnimation.SetBool("UpLeft", false);
                humanAnimation.SetBool("DownRight", false);
                humanAnimation.SetBool("DownLeft", false);
            }
            else if (Mathf.Sign(GetComponent<Rigidbody2D>().velocity.y) < 0 && GetComponent<Rigidbody2D>().velocity.y != 0.0f)
            {
                humanAnimation.SetBool("Idle", false);
                humanAnimation.SetBool("Up", false);
                humanAnimation.SetBool("Down", false);
                humanAnimation.SetBool("UpRight", false);
                humanAnimation.SetBool("UpLeft", false);
                humanAnimation.SetBool("DownRight", true);
                humanAnimation.SetBool("DownLeft", false);
            }
            else if (GetComponent<Rigidbody2D>().velocity.y == 0.0f)
            {
                humanAnimation.SetBool("Idle", false);
                humanAnimation.SetBool("Up", false);
                humanAnimation.SetBool("Down", false);
                humanAnimation.SetBool("UpRight", false);
                humanAnimation.SetBool("UpLeft", false);
                humanAnimation.SetBool("DownRight", true);
                humanAnimation.SetBool("DownLeft", false);
            }
        }
        else if (Mathf.Sign(GetComponent<Rigidbody2D>().velocity.x) < 0 && GetComponent<Rigidbody2D>().velocity.x != 0.0f)
        {
            if (Mathf.Sign(GetComponent<Rigidbody2D>().velocity.y) < 0 && GetComponent<Rigidbody2D>().velocity.y != 0.0f)
            {
                humanAnimation.SetBool("Idle", false);
                humanAnimation.SetBool("Up", false);
                humanAnimation.SetBool("Down", false);
                humanAnimation.SetBool("UpRight", false);
                humanAnimation.SetBool("UpLeft", false);
                humanAnimation.SetBool("DownRight", false);
                humanAnimation.SetBool("DownLeft", true);
            }
            else if (Mathf.Sign(GetComponent<Rigidbody2D>().velocity.y) > 0 && GetComponent<Rigidbody2D>().velocity.y != 0.0f)
            {
                humanAnimation.SetBool("Idle", false);
                humanAnimation.SetBool("Up", false);
                humanAnimation.SetBool("Down", false);
                humanAnimation.SetBool("UpRight", false);
                humanAnimation.SetBool("UpLeft", true);
                humanAnimation.SetBool("DownRight", false);
                humanAnimation.SetBool("DownLeft", false);
            }
            else if (GetComponent<Rigidbody2D>().velocity.y == 0.0f)
            {
                humanAnimation.SetBool("Idle", false);
                humanAnimation.SetBool("Up", false);
                humanAnimation.SetBool("Down", false);
                humanAnimation.SetBool("UpRight", false);
                humanAnimation.SetBool("UpLeft", false);
                humanAnimation.SetBool("DownRight", false);
                humanAnimation.SetBool("DownLeft", true);
            }
        }
        else
        {
            if (GetComponent<Rigidbody2D>().velocity.x == 0.0f && Mathf.Sign(GetComponent<Rigidbody2D>().velocity.y) < 0)
            {
                humanAnimation.SetBool("Idle", false);
                humanAnimation.SetBool("Up", false);
                humanAnimation.SetBool("Down", true);
                humanAnimation.SetBool("UpRight", false);
                humanAnimation.SetBool("UpLeft", false);
                humanAnimation.SetBool("DownRight", false);
                humanAnimation.SetBool("DownLeft", false);
            }
            else if (GetComponent<Rigidbody2D>().velocity.x == 0.0f && Mathf.Sign(GetComponent<Rigidbody2D>().velocity.y) > 0 && GetComponent<Rigidbody2D>().velocity.y != 0.0f)
            {
                humanAnimation.SetBool("Idle", false);
                humanAnimation.SetBool("Up", true);
                humanAnimation.SetBool("Down", false);
                humanAnimation.SetBool("UpRight", false);
                humanAnimation.SetBool("UpLeft", false);
                humanAnimation.SetBool("DownRight", false);
                humanAnimation.SetBool("DownLeft", false);
            }
            else if (GetComponent<Rigidbody2D>().velocity.x == 0.0f && GetComponent<Rigidbody2D>().velocity.y == 0.0f)
            {
                humanAnimation.SetBool("Idle", true);
                humanAnimation.SetBool("Up", false);
                humanAnimation.SetBool("Down", false);
                humanAnimation.SetBool("UpRight", false);
                humanAnimation.SetBool("UpLeft", false);
                humanAnimation.SetBool("DownRight", false);
                humanAnimation.SetBool("DownLeft", false);
            }
        }
        this.gameObject.GetComponent<HumanInfo>().CurrentDirection = this.gameObject.GetComponent<Animator>().GetCurrentAnimatorStateInfo(0).fullPathHash;
    }
}
