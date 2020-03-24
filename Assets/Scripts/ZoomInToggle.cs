using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ZoomInToggle : MonoBehaviour
{
    // Start is called before the first frame update
    public GameObject Xray;
    public GameObject ScopeOverlay;
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetMouseButtonDown(1))
        {
            bool isActive = Xray.gameObject.activeSelf;
            Xray.gameObject.SetActive(!isActive);
        }
        if (Input.GetKey(KeyCode.Z) && Camera.main.orthographicSize > 1.0f)
        {
            Camera.main.orthographicSize -= 0.1f;
            var currentScale = ScopeOverlay.transform.localScale;
            currentScale.x -= 0.1f;
            currentScale.y -= 0.1f;
            currentScale = new Vector3(Mathf.Clamp(currentScale.x, 3.0f, 6.0f), Mathf.Clamp(currentScale.y, 3.0f, 6.0f), 1.0f);
            ScopeOverlay.transform.localScale = currentScale;
        }
        if (Input.GetKey(KeyCode.X) && Camera.main.orthographicSize < 5.0f)
        {
            Camera.main.orthographicSize += 0.1f;
            var currentScale = ScopeOverlay.transform.localScale;
            currentScale.x += 0.1f;
            currentScale.y += 0.1f;
            currentScale = new Vector3(Mathf.Clamp(currentScale.x, 3.0f, 6.0f), Mathf.Clamp(currentScale.y, 3.0f, 6.0f), 1.0f);
            ScopeOverlay.transform.localScale = currentScale;
        }
    }
}
