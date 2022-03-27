using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PositionRendererSorter : MonoBehaviour
{
    [SerializeField]
    private int sortingOrderBase = 0;
    [SerializeField]
    private float offset = 0;
    [SerializeField]
    private bool runOnlyOnce = false;
    [SerializeField]
    private bool plusOneToSortingOrder = false;
    [SerializeField]
    private bool buildInterior = false;
    [SerializeField]
    private bool rowedFoliage = false;

    private float timer;
    private float timerMax = 0.1f;
    private Renderer myRenderer;

    private void Awake()
    {
        myRenderer = gameObject.GetComponent<Renderer>();
    }

    private void LateUpdate()
    {
        timer -= Time.deltaTime;
        if (timer <= 0.0f)
        {
            timer = timerMax;
            myRenderer.sortingOrder = Mathf.RoundToInt((transform.position.y - (myRenderer.bounds.extents.y / 2.0f) - offset) * 100.0f) * -1;
            if (plusOneToSortingOrder)
            {
                myRenderer.sortingOrder += 1;
            }
            if (buildInterior)
            {
                myRenderer.sortingOrder -= 1;
            }
            if (this.gameObject.tag == "Building")
            {
                if (Mathf.Sign(this.transform.position.y) == 0)
                {
                    myRenderer.sortingOrder += Mathf.RoundToInt(myRenderer.bounds.size.magnitude) * 100;
                }
                else
                {
                    myRenderer.sortingOrder += Mathf.RoundToInt(myRenderer.bounds.size.magnitude) * 100;
                }
            }
            if (runOnlyOnce)
            {
                Destroy(this);
            }
        }
    }
}
