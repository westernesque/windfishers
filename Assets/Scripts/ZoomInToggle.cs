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
        if (Input.mousePresent && Camera.main.orthographicSize == 5.0f)
        {
            if (Input.GetMouseButtonDown(1))
            {
                bool isActive = ZoomInUI.gameObject.activeSelf;
                ZoomInUI.gameObject.SetActive(!isActive);
            }
        }
        if (Input.GetKey(KeyCode.Z) && Camera.main.orthographicSize > 1.0f)
        {
            Camera.main.orthographicSize -= 0.1f;
        }
        if (Input.GetKey(KeyCode.X) && Camera.main.orthographicSize < 5.0f)
        {
            Camera.main.orthographicSize += 0.1f;
        }
    }
}
