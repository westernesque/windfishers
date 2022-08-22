using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DebugScene : MonoBehaviour
{
    // Update is called once per frame
    void Update()
    {
        // Reload the scene if the user presses the R key.
        if (Input.GetKeyDown(KeyCode.R))
        {
            UnityEngine.SceneManagement.SceneManager.LoadScene(UnityEngine.SceneManagement.SceneManager.GetActiveScene().name);
        }
    }
}
