using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HumanInfo : MonoBehaviour
{
    string HumanName;
    void Start()
    {
        HumanName = gameObject.name;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnMouseDown()
    {
        Debug.Log("Clicked: " + HumanName);
        GetComponentInParent<HumanGenerator>().ClickedHuman = HumanName;
        if (GetComponentInParent<HumanGenerator>().ClickedHuman == GetComponentInParent<HumanGenerator>().ChosenWaldo.name)
        {
            Debug.Log("WALDO FOUND");
        }
    }
}
