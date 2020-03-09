using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BountyUI : MonoBehaviour
{
    // Start is called before the first frame update
    public GameObject BountyPanel;

public void ToggleUI()
    {
        bool isActive = BountyPanel.activeSelf;
        BountyPanel.gameObject.SetActive(!isActive);
    }
}
