using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.U2D;
using UnityEngine.UI;

public class BountyUI : MonoBehaviour
{
    // Start is called before the first frame update
    public GameObject BountyPanel;
    public bool firstTimeLoaded = false;
    public Sprite humanSprite;
    public List<SpriteAtlas> humanFace;
    public string humanName;

    public void ToggleUI()
    {
        bool isActive = BountyPanel.activeSelf;
        BountyPanel.gameObject.SetActive(!isActive);
        if (BountyPanel.activeSelf && firstTimeLoaded == false)
        {
            GameObject.Find("Canvas/Bounty Panel/Name").GetComponent<TextMeshProUGUI>().text += humanName;
            GameObject.Find("Canvas/Bounty Panel/Sprite").GetComponent<Image>().sprite = humanSprite;
            Debug.Log("HumanSprite from BPtoggle: " + humanSprite);
            GameObject.Find("Canvas/Bounty Panel/Sprite").GetComponent<Image>().color = GameObject.Find("Humans").GetComponent<HumanGenerator>().ChosenWaldo.GetComponent<SpriteRenderer>().color;
            GameObject.Find("Canvas/Bounty Panel/Sprite/Nose").GetComponent<Image>().sprite = humanFace[0].GetSprite("Idle");
            GameObject.Find("Canvas/Bounty Panel/Sprite/Eyes").GetComponent<Image>().sprite = humanFace[1].GetSprite("Idle");
            GameObject.Find("Canvas/Bounty Panel/Sprite/Hair").GetComponent<Image>().sprite = humanFace[2].GetSprite("Idle");
            firstTimeLoaded = true;
        }
    }

    public void PopulateBountyUI()
    {
        // Get Sprite Data
        humanSprite = GameObject.Find("Humans").GetComponent<HumanGenerator>().ChosenWaldo.GetComponent<SpriteRenderer>().sprite;
        Debug.Log("bounty ui found humanSprite: " + humanSprite);
        humanFace = GameObject.Find("Humans").GetComponent<HumanGenerator>().ChosenWaldo.GetComponent<HumanInfo>().face;
        Debug.Log("bounty ui found humanFace: " + humanFace);
        humanName = GameObject.Find("Humans").GetComponent<HumanGenerator>().ChosenWaldo.name.Replace("\r", "");
    }
}
