using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.U2D;

public class HumanInfo : MonoBehaviour
{
    public string HumanName;
    public List<SpriteAtlas> face;
    public SpriteAtlas nose;
    public SpriteAtlas eyes;
    public SpriteAtlas hair;
    GameObject humanNose;
    GameObject humanEyes;
    GameObject humanHair;
    int DirectionChanged;
    public int CurrentDirection;
    void Start()
    {
        HumanName = gameObject.name;
        humanNose = this.transform.GetChild(1).gameObject;
        humanEyes = this.transform.GetChild(0).gameObject;
        humanHair = this.transform.GetChild(2).gameObject;
        nose = face[0];
        eyes = face[1];
        hair = face[2];
    }

    // Update is called once per frame
    void Update()
    {
        if (DirectionChanged != CurrentDirection)
        {
            ChangeFaceDirection();
            DirectionChanged = CurrentDirection;
        }
    }

    void ChangeFaceDirection()
    {
        if (this.gameObject.GetComponent<Animator>().GetBool("Down"))
        {
            humanNose.GetComponent<SpriteRenderer>().sprite = nose.GetSprite("Down");
            humanEyes.GetComponent<SpriteRenderer>().sprite = eyes.GetSprite("Down");
            humanHair.GetComponent<SpriteRenderer>().sprite = hair.GetSprite("Down");
        }
        else if (this.gameObject.GetComponent<Animator>().GetBool("Idle"))
        {
            humanNose.GetComponent<SpriteRenderer>().sprite = nose.GetSprite("Idle");
            humanEyes.GetComponent<SpriteRenderer>().sprite = eyes.GetSprite("Idle");
            humanHair.GetComponent<SpriteRenderer>().sprite = hair.GetSprite("Idle");
        }
        else if (this.gameObject.GetComponent<Animator>().GetBool("DownRight"))
        {
            humanNose.GetComponent<SpriteRenderer>().sprite = nose.GetSprite("Right");
            humanEyes.GetComponent<SpriteRenderer>().sprite = eyes.GetSprite("Right");
            humanHair.GetComponent<SpriteRenderer>().sprite = hair.GetSprite("DownRight");
        }
        else if (this.gameObject.GetComponent<Animator>().GetBool("DownLeft"))
        {
            humanNose.GetComponent<SpriteRenderer>().sprite = nose.GetSprite("Left");
            humanEyes.GetComponent<SpriteRenderer>().sprite = eyes.GetSprite("Left");
            humanHair.GetComponent<SpriteRenderer>().sprite = hair.GetSprite("DownLeft");
        }
        else if (this.gameObject.GetComponent<Animator>().GetBool("UpRight"))
        {
            humanNose.GetComponent<SpriteRenderer>().sprite = null;
            humanEyes.GetComponent<SpriteRenderer>().sprite = null;
            humanHair.GetComponent<SpriteRenderer>().sprite = hair.GetSprite("UpRight");
        }
        else if (this.gameObject.GetComponent<Animator>().GetBool("UpLeft"))
        {
            humanNose.GetComponent<SpriteRenderer>().sprite = null;
            humanEyes.GetComponent<SpriteRenderer>().sprite = null;
            humanHair.GetComponent<SpriteRenderer>().sprite = hair.GetSprite("UpLeft");
        }
        else if (this.gameObject.GetComponent<Animator>().GetBool("Up"))
        {
            humanNose.GetComponent<SpriteRenderer>().sprite = null;
            humanEyes.GetComponent<SpriteRenderer>().sprite = null;
            humanHair.GetComponent<SpriteRenderer>().sprite = hair.GetSprite("Up");
        }
    }

    private void OnMouseDown()
    {
        Debug.Log("Clicked: " + HumanName);
        GetComponentInParent<HumanGenerator>().ClickedHuman = HumanName;
        if (GetComponentInParent<HumanGenerator>().ClickedHuman == GetComponentInParent<HumanGenerator>().ChosenWaldo.name)
        {
            Debug.Log("WALDO FOUND");
        }
    }
}
