using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public float Countdown = 120.0f;
    public bool PickupsEnabled;
    public bool levelEnd = false;

    void Update()
    {
        if (levelEnd == false)
        {
            Countdown -= Time.deltaTime;
            if (GameObject.Find("Humans").GetComponent<HumanGenerator>().ChosenWaldo.name == GameObject.Find("Humans").GetComponent<HumanGenerator>().ClickedHuman)
            {
                SceneManager.LoadScene("Win", LoadSceneMode.Additive);
                GameObject.Find("Main Camera/Night").GetComponent<Nighttime>().timerStopped = true;
                levelEnd = true;
            }
            if (Countdown <= 0.0f)
            {
                SceneManager.LoadScene("Lose", LoadSceneMode.Additive);
                GameObject.Find("Main Camera/Night").GetComponent<Nighttime>().timerStopped = true;
                levelEnd = true;
            }
        }
    }

}