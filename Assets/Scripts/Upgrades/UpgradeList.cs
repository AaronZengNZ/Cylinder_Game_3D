using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "UpgradeList", menuName = "UpgradeList", order = 0)]
public class UpgradeList : ScriptableObject {
    public string className = "trigonometry";
    public GameObject[] specialUpgrades;
    public float specialUpgradeLevel = 0f;
    public GameObject[] upgrades;
    public float[] upgradeLevels;
}
