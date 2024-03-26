using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UpgradeManager : MonoBehaviour
{
    [Header("References")]
    public GameObject specialUpgradePrefab;
    public SpecialUpgradeSimulator[] upgradeSimulators;
    public StatsManager statsManager;
    [Header("Other")]
    //firerate
    public float firerateAdditiveChanges = 0f;
    public float firerateMultiplicativeChanges = 1f;
    //damage
    public float damageAdditiveChanges = 0f;
    public float damageMultiplicativeChanges = 1f;
    //pierce
    public float pierceAdditiveChanges = 0f;
    public float pierceMultiplicativeChanges = 1f;
    //bulletSpeed
    public float bulletSpeedAdditiveChanges = 0f;
    public float bulletSpeedMultiplicativeChanges = 1f;
    //bulletMass
    public float bulletMassAdditiveChanges = 0f;
    public float bulletMassMultiplicativeChanges = 1f;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    public void NewUpgrade(string upgradeType, string upgradeName, string upgradeId, 
    string upgradeValue1Type, string upgradeValue1Equation, float upgradeValue1, 
    string upgradeValue2Type, string upgradeValue2Equation, float upgradeValue2){
        if(CheckUpgradeExists(upgradeName)){
            return;
        }
        else{
            GameObject newUpgrade = Instantiate(specialUpgradePrefab, transform.position, Quaternion.identity);
            newUpgrade.transform.parent = transform;
            SpecialUpgradeSimulator newUpgradeSimulator = newUpgrade.GetComponent<SpecialUpgradeSimulator>();
            newUpgradeSimulator.upgradeType = upgradeType;
            newUpgradeSimulator.upgradeName = upgradeName;
            newUpgradeSimulator.upgradeId = upgradeId;
            newUpgradeSimulator.upgradeValue1Type = upgradeValue1Type;
            newUpgradeSimulator.upgradeValue1Equation = upgradeValue1Equation;
            newUpgradeSimulator.upgradeValue1 = upgradeValue1;
            newUpgradeSimulator.upgradeValue2Type = upgradeValue2Type;
            newUpgradeSimulator.upgradeValue2Equation = upgradeValue2Equation;
            newUpgradeSimulator.upgradeValue2 = upgradeValue2;
            AddUpgradeToArray(newUpgradeSimulator);
        }
    }

    private void AddUpgradeToArray(SpecialUpgradeSimulator upgrade){
        List<SpecialUpgradeSimulator> upgradeList = new List<SpecialUpgradeSimulator>(upgradeSimulators);
        upgradeList.Add(upgrade);
        upgradeSimulators = upgradeList.ToArray();
    }

    private bool CheckUpgradeExists(string upgradeName){
        foreach(SpecialUpgradeSimulator upgrade in upgradeSimulators){
            if(upgrade.upgradeName == upgradeName){
                return true;
            }
        }
        return false;
    }

    private void SimulateUpgradeStatByUpgrade(SpecialUpgradeSimulator target){
        switch (target.upgradeValue1Equation){
            case "add":
                switch (target.upgradeValue1Type){
                    case "firerate":
                        firerateAdditiveChanges += target.upgradeValue1;
                        break;
                    case "damage":
                        damageAdditiveChanges += target.upgradeValue1;
                        break;
                    case "pierce":
                        pierceAdditiveChanges += target.upgradeValue1;
                        break;
                    case "bulletSpeed":
                        bulletSpeedAdditiveChanges += target.upgradeValue1;
                        break;
                    case "bulletMass":
                        bulletMassAdditiveChanges += target.upgradeValue1;
                        break;
                }
            break;
            case "multiply":
                switch (target.upgradeValue1Type){
                    case "firerate":
                        firerateMultiplicativeChanges += target.upgradeValue1;
                        break;
                    case "damage":
                        damageMultiplicativeChanges += target.upgradeValue1;
                        break;
                    case "pierce":
                        pierceMultiplicativeChanges += target.upgradeValue1;
                        break;
                    case "bulletSpeed":
                        bulletSpeedMultiplicativeChanges += target.upgradeValue1;
                        break;
                    case "bulletMass":
                        bulletMassMultiplicativeChanges += target.upgradeValue1;
                        break;
                }
            break;
        }
        switch (target.upgradeValue2Equation){
            case "add":
                switch (target.upgradeValue2Type){
                    case "firerate":
                        firerateAdditiveChanges += target.upgradeValue2;
                        break;
                    case "damage":
                        damageAdditiveChanges += target.upgradeValue2;
                        break;
                    case "pierce":
                        pierceAdditiveChanges += target.upgradeValue2;
                        break;
                    case "bulletSpeed":
                        bulletSpeedAdditiveChanges += target.upgradeValue2;
                        break;
                    case "bulletMass":
                        bulletMassAdditiveChanges += target.upgradeValue2;
                        break;
                }
            break;
            case "multiply":
                switch (target.upgradeValue2Type){
                    case "firerate":
                        firerateMultiplicativeChanges += target.upgradeValue2;
                        break;
                    case "damage":
                        damageMultiplicativeChanges += target.upgradeValue2;
                        break;
                    case "pierce":
                        pierceMultiplicativeChanges += target.upgradeValue2;
                        break;
                    case "bulletSpeed":
                        bulletSpeedMultiplicativeChanges += target.upgradeValue2;
                        break;
                    case "bulletMass":
                        bulletMassMultiplicativeChanges += target.upgradeValue2;
                        break;
                }
            break;  
        }      
    }

    private void SimulateUpgradeAllStats(){
        foreach(SpecialUpgradeSimulator upgrade in upgradeSimulators){
            SimulateUpgradeStatByUpgrade(upgrade);
        }
    }

    // Update is called once per frame
    void Update()
    {
        SimulateUpgradeAllStats();
        statsManager.UpgradeManagerStatTool(firerateAdditiveChanges, firerateMultiplicativeChanges,
        damageAdditiveChanges, damageMultiplicativeChanges,
        pierceAdditiveChanges, pierceMultiplicativeChanges,
        bulletSpeedAdditiveChanges, bulletSpeedMultiplicativeChanges,
        bulletMassAdditiveChanges, bulletMassMultiplicativeChanges);
    }
}
