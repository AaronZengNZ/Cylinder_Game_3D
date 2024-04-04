using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpecialUpgradeSimulator : MonoBehaviour
{
    public string upgradeType = "trig";
    public string upgradeName = "power_of_sine";
    public string upgradeId = "sin_offset";
    public string upgradeValue1Type = "gun_power";
    public string upgradeValue1Equation = "add";
    public float upgradeValue1 = 5f;
    public string upgradeValue2Type = "gun_offset_quantity";
    public string upgradeValue2Equation = "add";
    public float upgradeValue2 = 10f;
}
