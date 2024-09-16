using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoatMovement : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        float boatDirection = Input.GetAxis("Horizontal");
        if (boatDirection < 0.0f)
        {
            GetComponent<SpriteRenderer>().sprite = Resources.Load<Sprite>("Prefabs/Boat - Large 02");
        }
        if (boatDirection > 0.0f)
        {
            GetComponent<SpriteRenderer>().sprite = Resources.Load<Sprite>("Prefabs/Boat - Large");

        }
    }
}
