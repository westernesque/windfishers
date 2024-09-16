using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MagnifyCameraFollow : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        if (Input.mousePresent)
        {
            this.transform.position = Camera.main.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, z: 9.0f));
        }
    }
}
