using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
public class UpgradeTextDisplay : MonoBehaviour
{
    public TextMeshProUGUI text;

    public string upgradeName;
    public float upgradeLevel;
    public string upgradeType;
    public float upgradePointsSpent = 0f;
    // Update is called once per frame
    void Update()
    {
        string tempText = "";
        tempText += "<color=white>" + upgradeName + "</color> - Level " + upgradeLevel + " |";
        switch (upgradeType.ToLower())
        {
            case "basic":
                tempText += "<color=#DDDDDD>Basic Upgrade</color> | ";
                break;
            case "special":
                tempText += "<color=#FF5555>Special Upgrade</color> | ";
                break;
            case "theory":
                tempText += "<color=#FFF555>Theory Upgrade</color> | ";
                break;
        }
        tempText += "<color=#00FFFF>" + upgradePointsSpent + " UP spent</color>";
        text.text = tempText;
    }
}
