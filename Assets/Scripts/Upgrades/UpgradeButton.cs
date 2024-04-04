using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UpgradeButton : MonoBehaviour
{
    [Header("References")]
    public UpgradeManager upgradeManager;
    [Header("Variables")]
    public string upgradeType = "trig";
    public string upgradeName = "power_of_sine";
    public string upgradeId = "sin_offset";
    public string upgradeValue1Type = "gun_power";
    public string upgradeValue1Equation = "add";
    public float upgradeValue1 = 5f;
    public string upgradeValue2Type = "gun_offset_quantity";
    public string upgradeValue2Equation = "add";
    public float upgradeValue2 = 10f;
    
    void Start(){
        upgradeManager = GameObject.Find("UpgradeManager").GetComponent<UpgradeManager>();
    }

    public void TryUpgrade(){
        //do calculations here (check for cost, etc)
        UpgradeSuccess();
    }

    private void UpgradeSuccess(){
        upgradeManager.NewUpgrade(upgradeType, upgradeName, upgradeId, upgradeValue1Type, 
        upgradeValue1Equation, upgradeValue1, upgradeValue2Type, upgradeValue2Equation, upgradeValue2);
    }
}
