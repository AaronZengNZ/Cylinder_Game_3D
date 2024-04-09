using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class UpgradeButton : MonoBehaviour
{
    [Header("References")]
    public UpgradeManager upgradeManager;
    public UpgradeButtonManager upgradeButtonManager;
    public Outline hoverOutline;
    public Animator animator;
    public GameObject selfPrefab;
    [Header("Text References")]
    public TextMeshProUGUI TitleText;
    public string title;
    //string that is text that follows the title
    public string titleType = "<color=#FF5555>Special</color>";
    public TextMeshProUGUI DescText;
    public string[] descriptions;
    public TextMeshProUGUI FunctionText;
    public string[] functions;
    public TextMeshProUGUI SecondaryFunctionText;
    public string[] secondaryFunctions;
    public TextMeshProUGUI CostText;
    public float[] costs;
    [Header("Variables")]
    public string upgradeType = "special";
    public string upgradeName = "power_of_sine";
    public string upgradeId = "sin_offset";
    public string upgradeValue1Type = "gun_power";
    public string upgradeValue1Equation = "add";
    public float[] upgradeValue1;
    public string upgradeValue2Type = "gun_offset_quantity";
    public string upgradeValue2Equation = "add";
    public float[] upgradeValue2;
    public int level = 0;
    public bool upgrading = false;
    
    void Start(){
        upgradeManager = GameObject.Find("UpgradeManager").GetComponent<UpgradeManager>();
        upgradeButtonManager = GameObject.Find("UpgradeButtonManager").GetComponent<UpgradeButtonManager>();
        animator = GetComponent<Animator>();
        hoverOutline.enabled = false;
        UpdateTexts();
    }

    public void TryUpgrade(){
        upgradeButtonManager.GetUpgrade(this);
    }

    public void Hover(){
        if(upgrading){return;}
        hoverOutline.enabled = true;
    }

    public void Unhover(){
        if(upgrading){return;}
        hoverOutline.enabled = false;
    }

    public void DisableButton(){
        upgrading = true;
        animator.SetInteger("FocusInt", -1);
    }

    public void Upgrade(){
        upgrading = true;
        animator.SetInteger("FocusInt", 1);
    }

    public void CancelUpgrade(){
        Destroy(gameObject);
    }

    public void UpgradeSuccess(){
        upgradeManager.NewUpgrade(upgradeType, upgradeName, upgradeId, upgradeValue1Type, 
        upgradeValue1Equation, upgradeValue1[level], upgradeValue2Type, upgradeValue2Equation, upgradeValue2[level]);
        upgradeButtonManager.UpgradeFinished();
        CancelUpgrade();
    }

    private void UpdateTexts(){
        TitleText.text = title + " " + level + " - " + titleType;
        DescText.text = descriptions[level];
        FunctionText.text = functions[level];
        SecondaryFunctionText.text = secondaryFunctions[level];
        CostText.text = "Cost: " + costs[level].ToString() + " UP";
    }
}
