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
    public float gunOffsetSpeedChanges = 0f;
    public float gunOffsetQuantityChanges = 0f;

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
                UpgradeValueHandlerAdditive(target.upgradeValue1Type, target.upgradeValue1);
            break;
            case "multiply":
                UpgradeValueHandlerMultiplicative(target.upgradeValue1Type, target.upgradeValue1);
            break;
        }
        switch (target.upgradeValue2Equation){
            case "add":
                UpgradeValueHandlerAdditive(target.upgradeValue2Type, target.upgradeValue2);
            break;
            case "multiply":
                UpgradeValueHandlerMultiplicative(target.upgradeValue2Type, target.upgradeValue2);
            break;  
        }      
    }

    private void UpgradeValueHandlerAdditive(string upgradeType, float upgradeValue){
        switch (upgradeType){
            case "gun_firerate":
                firerateAdditiveChanges += upgradeValue;
                break;
            case "gun_power":
                damageAdditiveChanges += upgradeValue;
                break;
            case "gun_pierce":
                pierceAdditiveChanges += upgradeValue;
                break;
            case "gun_bullet_speed":
                bulletSpeedAdditiveChanges += upgradeValue;
                break;
            case "gun_bullet_mass":
                bulletMassAdditiveChanges += upgradeValue;
                break;
            case "gun_offset_quantity":
                gunOffsetQuantityChanges += upgradeValue;
                break;
        }
    }

    private void UpgradeValueHandlerMultiplicative(string upgradeType, float upgradeValue){
        switch (upgradeType){
            case "gun_firerate":
                firerateMultiplicativeChanges *= upgradeValue;
                break;
            case "gun_power":
                damageMultiplicativeChanges *= upgradeValue;
                break;
            case "gun_pierce":
                pierceMultiplicativeChanges *= upgradeValue;
                break;
            case "gun_bullet_speed":
                bulletSpeedMultiplicativeChanges *= upgradeValue;
                break;
            case "gun_bullet_mass":
                bulletMassMultiplicativeChanges *= upgradeValue;
                break;
        }
    }

    private void SimulateUpgradeAllStats(){
        ResetVariablesToZero();
        foreach(SpecialUpgradeSimulator upgrade in upgradeSimulators){
            SimulateUpgradeStatByUpgrade(upgrade);
        }
    }

    private void ResetVariablesToZero(){
        //TRY TO FIND SOME WAY TO DO THIS WITH A LIST OR SOMETHING
        firerateAdditiveChanges = 0f;
        firerateMultiplicativeChanges = 1f;
        damageAdditiveChanges = 0f;
        damageMultiplicativeChanges = 1f;
        pierceAdditiveChanges = 0f;
        pierceMultiplicativeChanges = 1f;
        bulletSpeedAdditiveChanges = 0f;
        bulletSpeedMultiplicativeChanges = 1f;
        bulletMassAdditiveChanges = 0f;
        bulletMassMultiplicativeChanges = 1f;
        gunOffsetSpeedChanges = 0f;
        gunOffsetQuantityChanges = 0f;
    }

    // Update is called once per frame
    void Update()
    {
        SimulateUpgradeAllStats();
        statsManager.UpgradeManagerStatTool(firerateAdditiveChanges, firerateMultiplicativeChanges,
        damageAdditiveChanges, damageMultiplicativeChanges,
        pierceAdditiveChanges, pierceMultiplicativeChanges,
        bulletSpeedAdditiveChanges, bulletSpeedMultiplicativeChanges,
        bulletMassAdditiveChanges, bulletMassMultiplicativeChanges,
        gunOffsetSpeedChanges, gunOffsetQuantityChanges);
    }
}
