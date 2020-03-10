using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ZoomInToggle : MonoBehaviour
{
    // Start is called before the first frame update
    public GameObject ZoomInUI;
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.mousePresent)
        {
            if (Input.GetMouseButtonDown(1))
            {
                bool isActive = ZoomInUI.gameObject.activeSelf;
                ZoomInUI.gameObject.SetActive(!isActive);
            }
        }
    }
}
