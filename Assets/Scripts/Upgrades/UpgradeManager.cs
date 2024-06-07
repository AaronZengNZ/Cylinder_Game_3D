using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UpgradeManager : MonoBehaviour
{
    [Header("References")]
    public GameObject specialUpgradePrefab;
    public SpecialUpgradeSimulator[] upgradeSimulators;
    public StatsManager statsManager;
    [Header("Other (Gun)")]
    //firerate
    private float firerateAdditiveChanges = 0f;
    private float firerateMultiplicativeChanges = 1f;
    //damage
    private float damageAdditiveChanges = 0f;
    private float damageMultiplicativeChanges = 1f;
    //pierce
    private float pierceAdditiveChanges = 0f;
    private float pierceMultiplicativeChanges = 1f;
    //bulletSpeed
    private float bulletSpeedAdditiveChanges = 0f;
    private float bulletSpeedMultiplicativeChanges = 1f;
    //bulletMass
    private float bulletMassAdditiveChanges = 0f;
    private float bulletMassMultiplicativeChanges = 1f;
    //gunOffset
    private float gunOffsetSpeedChanges = 0f;
    public float gunOffsetQuantityBase = 10f;
    private float gunOffsetQuantityChanges = 0f;
    //special
    [Header ("Trig")]
    public float cosineTimeMult = 1f;
    public float cosineSpeedMult = 1f;
    public float tangentRequirement = 3f;
    public float tangentBuff = 0f;
    [Header ("Misc Stats")]
    public float defenseMultiplicativeChanges = 1f;
    public float defenseAdditiveChanges = 0f;
    [Header("Other (Player)")]
    public float playerSpeedAdditiveChanges = 0f;
    public float playerSpeedMultiplicativeChanges = 1f;
    public float playerMassAdditiveChanges = 0f;
    public float playerMassMultiplicativeChanges = 1f;

    // Start is called before the first frame update
    void Start()
    {
        ResetVariablesToZero();
    }

    public void NewUpgrade(string upgradeType, string upgradeName, string upgradeId, 
    string upgradeValue1Type, string upgradeValue1Equation, float upgradeValue1, 
    string upgradeValue2Type, string upgradeValue2Equation, float upgradeValue2){
        if(CheckUpgradeExists(upgradeName)){
            //find the upgrade and replace the values
            GameObject oldUpgrade = null;
            foreach(SpecialUpgradeSimulator upgrade in upgradeSimulators){
                if(upgrade.upgradeName == upgradeName){
                    UnityEngine.Debug.Log(upgrade.upgradeName);
                    oldUpgrade = upgrade.gameObject;
                }
            }
            if(oldUpgrade == null){return;}
            SpecialUpgradeSimulator UpgradeSimulator = oldUpgrade.GetComponent<SpecialUpgradeSimulator>();
            UpgradeSimulator.upgradeValue1 = upgradeValue1;
            UpgradeSimulator.upgradeValue2 = upgradeValue2;
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
            case "cos_time":
                cosineTimeMult += upgradeValue;
                break;
            case "cos_speed":
                cosineSpeedMult += upgradeValue;
                break;
            case "player_speed":
                playerSpeedAdditiveChanges += upgradeValue;
                break;
            case "player_mass":
                playerMassAdditiveChanges += upgradeValue;
                break;
            case "defense":
                defenseAdditiveChanges += upgradeValue;
                break;
            case "tangent":
                tangentBuff += upgradeValue;
                break;
            case "tangentReq":
                tangentRequirement += upgradeValue;
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
            case "gun_offset_speed":
                gunOffsetSpeedChanges += upgradeValue;
                break;
            case "gun_offset_quantity":
                gunOffsetQuantityChanges += upgradeValue;
                break;
            case "player_speed":
                playerSpeedMultiplicativeChanges *= upgradeValue;
                break;
            case "player_mass":
                playerMassMultiplicativeChanges *= upgradeValue;
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
        cosineTimeMult = 0f;
        cosineSpeedMult = 1f;
        playerMassAdditiveChanges = 0f;
        playerMassMultiplicativeChanges = 1f;
        playerSpeedAdditiveChanges = 0f;
        playerSpeedMultiplicativeChanges = 1f;
        defenseAdditiveChanges = 0f;
        defenseMultiplicativeChanges = 1f;
        tangentBuff = 0f;
        tangentRequirement = 0f;
    }

    // Update is called once per frame
    void Update()
    {
        SimulateUpgradeAllStats();
        float newFirerateMultiplicativeChanges = firerateMultiplicativeChanges;
        newFirerateMultiplicativeChanges = newFirerateMultiplicativeChanges * CosSpeed();
        if(TanCheck()){
            defenseAdditiveChanges += tangentBuff;
        }
        statsManager.UpdateStatCalculation("firerate", firerateAdditiveChanges, newFirerateMultiplicativeChanges);
        statsManager.UpdateStatCalculation("damage", damageAdditiveChanges, damageMultiplicativeChanges);
        statsManager.UpdateStatCalculation("pierce", pierceAdditiveChanges, pierceMultiplicativeChanges);
        statsManager.UpdateStatCalculation("bulletSpeed", bulletSpeedAdditiveChanges, bulletSpeedMultiplicativeChanges);
        statsManager.UpdateStatCalculation("bulletMass", bulletMassAdditiveChanges, bulletMassMultiplicativeChanges);
        statsManager.UpdateStatCalculation("gunOffsetSpeed", gunOffsetSpeedChanges);
        statsManager.UpdateStatCalculation("gunOffsetQuantity", gunOffsetQuantityChanges);
        statsManager.UpdateStatCalculation("playerSpeed", playerSpeedAdditiveChanges, playerSpeedMultiplicativeChanges);
        statsManager.UpdateStatCalculation("playerMass", playerMassAdditiveChanges, playerMassMultiplicativeChanges);
        statsManager.UpdateStatCalculation("defense", defenseAdditiveChanges, defenseMultiplicativeChanges);
    }

    private bool TanCheck(){
        float tangent = Mathf.Tan(Time.time);
        if(tangent > tangentRequirement){
            return true;
        }
        return false;
    }

    private float CosSpeed(){
        if(cosineTimeMult == 0f){
            return 1f;
        }
        float value = Mathf.Sin(Time.time) * cosineTimeMult + cosineSpeedMult;
        if(value <= 0.01){
            value = 0;
        }
        return value;
    }
}
