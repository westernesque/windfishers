using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneLoader : MonoBehaviour
{
    public void ReloadMainScene()
    {
        SceneManager.LoadScene("Main");
    }
}
