using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CameraMovement : MonoBehaviour
{
    float Speed = 10.0f;
    float islandBoundsX;
    float islandBoundsY;
    // Start is called before the first frame update
    void Start()
    {
        islandBoundsX = GameObject.Find("Island").GetComponent<IslandGenerator>().IslandBounds.x;
        islandBoundsY = GameObject.Find("Island").GetComponent<IslandGenerator>().IslandBounds.y;
    }

    // Update is called once per frame
    void Update()
    {
        float xAxisValue = Input.GetAxis("Horizontal") * Time.deltaTime * Speed;
        float yAxisValue = Input.GetAxis("Vertical") * Time.deltaTime * Speed;
        //float zAxisValue = Camera.current.transform.position.z;
        if (Camera.current != null)
        {
            if (Camera.current.transform.position.x < -islandBoundsX - 5.0f)
            {
                Camera.current.transform.position = new Vector3(-islandBoundsX - 5.0f, Camera.current.transform.position.y, Camera.current.transform.position.z);
            }
            if (Camera.current.transform.position.x > islandBoundsX + 5.0f)
            {
                Camera.current.transform.position = new Vector3(islandBoundsX + 5.0f, Camera.current.transform.position.y, Camera.current.transform.position.z);
            }
            if (Camera.current.transform.position.y < -islandBoundsY - 5.0f)
            {
                Camera.current.transform.position = new Vector3(Camera.current.transform.position.x, -islandBoundsY - 5.0f, Camera.current.transform.position.z);
            }
            if (Camera.current.transform.position.y > islandBoundsY + 5.0f)
            {
                Camera.current.transform.position = new Vector3(Camera.current.transform.position.x, islandBoundsY + 5.0f, Camera.current.transform.position.z);
            }
            //Camera.main.transform.Translate(new Vector3(xAxisValue, yAxisValue, 0.0f));
            Camera.main.transform.Translate(new Vector3(xAxisValue, yAxisValue, 0.0f));
        }
    }
}