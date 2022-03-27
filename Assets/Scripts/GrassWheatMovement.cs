using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GrassWheatMovement : MonoBehaviour
{
    // Start is called before the first frame update
    bool TestMovement = false;
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if(TestMovement)
        {
            Destroy(this.gameObject);
        }
    }
}
