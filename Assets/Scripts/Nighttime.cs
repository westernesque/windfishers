using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Nighttime : MonoBehaviour
{
    SpriteRenderer spriteRenderer;
    Color spriteRendererColor;
    Image timer;
    float redGreenValue = 1.0f;
    float CountdownRate;
    public bool timerStopped = false;

    void Awake()
    {
        AdjustNightLayer();
    }

    void AdjustNightLayer()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        timer = GameObject.Find("Canvas/Timer/Background").GetComponent<Image>();
        CountdownRate = 0.5f / GameObject.Find("Game Manager").GetComponent<GameManager>().Countdown;
        Debug.Log("Countdown rate: " + CountdownRate);
        spriteRendererColor = spriteRenderer.color;

        float cameraHeight = Camera.main.orthographicSize * 2;
        Vector2 cameraSize = new Vector2(Camera.main.aspect * cameraHeight, cameraHeight);
        Vector2 spriteSize = spriteRenderer.sprite.bounds.size;
        Vector2 scale = transform.localScale;
        if (cameraSize.x >= cameraSize.y)
        { // Landscape (or equal)
            scale *= cameraSize.x / spriteSize.x;
        }
        else
        { // Portrait
            scale *= cameraSize.y / spriteSize.y;
        }
        transform.position = Vector2.zero; // Optional
        transform.localScale = scale;
    }
    // Start is called before the first frame update
    void Start()
    {
        
    }

    void UpdateTimer()
    {
        if (timerStopped == false)
        {
            spriteRendererColor.a += Time.deltaTime * CountdownRate;
            spriteRenderer.color = spriteRendererColor;
            timer.color = new Color(redGreenValue, redGreenValue, timer.color.b);
            redGreenValue -= Time.deltaTime * CountdownRate;
        }
    }


    // Update is called once per frame
    void Update()
    {
        UpdateTimer();
    }
}
