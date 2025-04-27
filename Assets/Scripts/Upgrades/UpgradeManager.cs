using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

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
    [Header("Trig")]
    public float cosineTimeMult = 1f;
    public float cosineSpeedMult = 1f;
    public float tangentRequirement = 3f;
    public float tangentBuff = 0f;
    public GameObject tangentAura;
    [Header("Misc Stats")]
    public float defenceMultiplicativeChanges = 1f;
    public float defenceAdditiveChanges = 0f;
    public float armorMultiplicativeChanges = 1f;
    public float armorAdditiveChanges = 0f;
    [Header("Other (Player)")]
    public float playerSpeedAdditiveChanges = 0f;
    public float playerSpeedMultiplicativeChanges = 1f;
    public float playerHpAdditiveChanges = 0f;
    public float playerHpMultiplicativeChanges = 1f;

    [Header("Special Upgrades")]
    public float theoryUpgradesGot = 0f;
    public bool tangentMaxed = false;
    public float pythagorasLimitationAngles = 0f;
    public bool sinMaxed = false;

    public GameObject tangentDefenceText;
    public GameObject cosineAspdText;
    public GameObject sinPowerText;
    public GameObject summationText;

    public float goldenRatioVelocityCorrespondence = 0f;
    public float goldenRatioPower = 0f;
    public bool goldenRatioMaxed = false;



    public float summationFirerateRampMax = 0f;
    public float summationFirerateReduction = 0f;
    public float summationFirerateMulti = 0f;
    public bool summationMaxed = false;
    // Start is called before the first frame update
    void Start()
    {
        ResetVariablesToZero();
        tangentMaxed = false;
        sinMaxed = false;
        goldenRatioMaxed = false;
        summationMaxed = false;
        pythagorasLimitationAngles = 0f;
    }

    public void NewUpgrade(string upgradeType, string upgradeName, string upgradeId,
    string upgradeValue1Type, string upgradeValue1Equation, float upgradeValue1,
    string upgradeValue2Type, string upgradeValue2Equation, float upgradeValue2)
    {
        if (CheckUpgradeExists(upgradeName))
        {
            //find the upgrade and replace the values
            GameObject oldUpgrade = null;
            foreach (SpecialUpgradeSimulator upgrade in upgradeSimulators)
            {
                if (upgrade.upgradeName == upgradeName)
                {
                    UnityEngine.Debug.Log(upgrade.upgradeName);
                    oldUpgrade = upgrade.gameObject;
                }
            }
            if (oldUpgrade == null) { return; }
            SpecialUpgradeSimulator UpgradeSimulator = oldUpgrade.GetComponent<SpecialUpgradeSimulator>();
            UpgradeSimulator.upgradeValue1 = upgradeValue1;
            UpgradeSimulator.upgradeValue2 = upgradeValue2;
            return;
        }
        else
        {
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

    public void AddSpecialUpgradeValue(string upgradeName, string check)
    {
        switch (upgradeName)
        {
            case "pythagoras":
                if (check == "one")
                {
                    pythagorasLimitationAngles = 90f;
                }
                else if (check == "max")
                {
                    pythagorasLimitationAngles = 45f;
                }
                break;
            case "tangent":
                if (check == "max")
                {
                    tangentMaxed = true;
                }
                break;
            case "sine":
                if (check == "max")
                {
                    sinMaxed = true;
                }
                break;
            case "golden_ratio":
                if (check == "max")
                {
                    goldenRatioMaxed = true;
                }
                break;
            case "summation":
                if (check == "max")
                {
                    summationMaxed = true;
                }
                break;
        }
    }

    private void AddUpgradeToArray(SpecialUpgradeSimulator upgrade)
    {
        List<SpecialUpgradeSimulator> upgradeList = new List<SpecialUpgradeSimulator>(upgradeSimulators);
        upgradeList.Add(upgrade);
        upgradeSimulators = upgradeList.ToArray();
    }

    private bool CheckUpgradeExists(string upgradeName)
    {
        foreach (SpecialUpgradeSimulator upgrade in upgradeSimulators)
        {
            if (upgrade.upgradeName == upgradeName)
            {
                return true;
            }
        }
        return false;
    }

    private void SimulateUpgradeStatByUpgrade(SpecialUpgradeSimulator target)
    {
        switch (target.upgradeValue1Equation)
        {
            case "add":
                UpgradeValueHandlerAdditive(target.upgradeValue1Type, target.upgradeValue1);
                break;
            case "multiply":
                UpgradeValueHandlerMultiplicative(target.upgradeValue1Type, target.upgradeValue1);
                break;
        }
        switch (target.upgradeValue2Equation)
        {
            case "add":
                UpgradeValueHandlerAdditive(target.upgradeValue2Type, target.upgradeValue2);
                break;
            case "multiply":
                UpgradeValueHandlerMultiplicative(target.upgradeValue2Type, target.upgradeValue2);
                break;
        }
    }

    private void UpgradeValueHandlerAdditive(string upgradeType, float upgradeValue)
    {
        switch (upgradeType)
        {
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
            case "player_hitpoints":
                playerHpAdditiveChanges += upgradeValue;
                break;
            case "defence":
                defenceAdditiveChanges += upgradeValue;
                break;
            case "armor":
                armorAdditiveChanges += upgradeValue;
                break;
            case "tangent":
                tangentBuff += upgradeValue;
                break;
            case "tangentReq":
                tangentRequirement += upgradeValue;
                break;
            case "movement_angle":
                pythagorasLimitationAngles += upgradeValue;
                break;
            case "golden_ratio_velocity":
                goldenRatioVelocityCorrespondence += upgradeValue;
                break;
            case "golden_ratio_power":
                goldenRatioPower += upgradeValue;
                break;
            case "summation_max":
                summationFirerateRampMax += upgradeValue;
                break;
            case "summation_level":
                switch (upgradeValue)
                {
                    case 1:
                        summationFirerateReduction = 0.1f;
                        firerateMultiplicativeChanges *= 0.8f;
                        break;
                    case 2:
                        summationFirerateReduction = 0.125f;
                        firerateMultiplicativeChanges *= 0.7f;
                        break;
                    case 3:
                        summationFirerateReduction = 0.15f;
                        firerateMultiplicativeChanges *= 0.65f;
                        break;
                    case 4:
                        summationFirerateReduction = 0.18f;
                        firerateMultiplicativeChanges *= 0.6f;
                        break;
                    case 5:
                        summationFirerateReduction = 0.05f;
                        firerateMultiplicativeChanges *= 0.6f;
                        break;
                }
                break;
        }
    }

    private void UpgradeValueHandlerMultiplicative(string upgradeType, float upgradeValue)
    {
        switch (upgradeType)
        {
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
            case "player_hitpoints":
                playerHpMultiplicativeChanges *= upgradeValue;
                break;
        }
    }

    private void SimulateUpgradeAllStats()
    {
        ResetVariablesToZero();
        foreach (SpecialUpgradeSimulator upgrade in upgradeSimulators)
        {
            SimulateUpgradeStatByUpgrade(upgrade);
        }
    }

    private void ResetVariablesToZero()
    {
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
        playerHpAdditiveChanges = 0f;
        playerHpMultiplicativeChanges = 1f;
        playerSpeedAdditiveChanges = 0f;
        playerSpeedMultiplicativeChanges = 1f;
        defenceAdditiveChanges = 0f;
        defenceMultiplicativeChanges = 1f;
        armorAdditiveChanges = 0f;
        armorMultiplicativeChanges = 1f;
        tangentBuff = 0f;
        tangentRequirement = 0f;
        goldenRatioVelocityCorrespondence = 0f;
        goldenRatioPower = 0f;
        summationFirerateRampMax = 0f;
        summationFirerateReduction = 0f;
    }

    // Update is called once per frame
    void Update()
    {
        UpdateSummationText();
        SimulateUpgradeAllStats();
        float newFirerateMultiplicativeChanges = firerateMultiplicativeChanges;
        newFirerateMultiplicativeChanges *= CosSpeed();
        newFirerateMultiplicativeChanges *= 1 + summationFirerateMulti;
        float newDamageMultiplicativeChanges = damageMultiplicativeChanges;
        newDamageMultiplicativeChanges *= SinPower();
        TangentUpgradeCheck();
        statsManager.UpdateStatCalculation("firerate", firerateAdditiveChanges, newFirerateMultiplicativeChanges);
        statsManager.UpdateStatCalculation("damage", damageAdditiveChanges, newDamageMultiplicativeChanges);
        statsManager.UpdateStatCalculation("pierce", pierceAdditiveChanges, pierceMultiplicativeChanges);
        statsManager.UpdateStatCalculation("bulletSpeed", bulletSpeedAdditiveChanges, bulletSpeedMultiplicativeChanges);
        statsManager.UpdateStatCalculation("bulletMass", bulletMassAdditiveChanges, bulletMassMultiplicativeChanges);
        statsManager.UpdateStatCalculation("gunOffsetSpeed", gunOffsetSpeedChanges);
        statsManager.UpdateStatCalculation("gunOffsetQuantity", gunOffsetQuantityChanges);
        statsManager.UpdateStatCalculation("playerSpeed", playerSpeedAdditiveChanges, playerSpeedMultiplicativeChanges);
        statsManager.UpdateStatCalculation("playerHp", playerHpAdditiveChanges, playerHpMultiplicativeChanges);
        statsManager.UpdateStatCalculation("defence", defenceAdditiveChanges, defenceMultiplicativeChanges);
        statsManager.UpdateStatCalculation("armor", armorAdditiveChanges, armorMultiplicativeChanges);
    }

    private void TangentUpgradeCheck()
    {
        if (tangentBuff > 0)
        {
            tangentDefenceText.SetActive(true);
            tangentDefenceText.GetComponent<TextMeshProUGUI>().text = "<color=#FF5555>Case[Tan[t]>" + tangentRequirement + "]<color=\"white\">{d=d+" + tangentBuff + "}(" + TanCheck() + ")";
            if (TanCheck())
            {
                defenceAdditiveChanges += tangentBuff;
                tangentAura.GetComponent<Animator>().SetBool("Show", true);
            }
            else
            {
                tangentAura.GetComponent<Animator>().SetBool("Show", false);
            }
        }
        else
        {
            tangentDefenceText.SetActive(false);
        }
    }

    private bool TanCheck()
    {
        float tangent = Mathf.Tan(Time.time / 2f);
        if (tangent > tangentRequirement)
        {
            return true;
        }
        return false;
    }

    private float SinPower()
    {
        if (!sinMaxed)
        {
            sinPowerText.SetActive(false);
            return 1f;
        }

        float value = Mathf.Abs(Mathf.Sin(Time.time * statsManager.GetStatFloat("gunOffsetSpeed"))) * 0.5f + 1f;
        sinPowerText.SetActive(true);
        sinPowerText.GetComponent<TextMeshProUGUI>().text = "p*=<color=#FF5555>[Abs(Sin(t))*0.5+1]<color=\"white\">(" + value.ToString("F1") + ")";
        return value;
    }

    private float CosSpeed()
    {
        if (cosineTimeMult == 0f)
        {
            cosineAspdText.SetActive(false);
            return 1f;
        }
        float value = Mathf.Sin(Time.time) * cosineTimeMult + cosineSpeedMult;
        if (value <= 0.01)
        {
            value = 0;
        }
        cosineAspdText.SetActive(true);
        cosineAspdText.GetComponent<TextMeshProUGUI>().text = "c=c/<color=#FF5555>[Cos[t]x" + cosineTimeMult + (cosineSpeedMult - 1).ToString("+0;-#") + "]<color=\"white\">(" + value.ToString("F2") + ")";
        return value;
    }

    public void NotFiringCalculations()
    {
        if (summationFirerateRampMax > 0)
        {
            summationFirerateMulti += summationFirerateRampMax * Time.deltaTime / (1.5f + summationFirerateRampMax / 2f);
            if (summationFirerateMulti > summationFirerateRampMax)
            {
                summationFirerateMulti = summationFirerateRampMax;
            }
        }
        else
        {
            summationFirerateMulti = 0;
        }
    }

    public void FireCalculations()
    {
        if (summationMaxed || summationFirerateMulti > 0)
        {
            summationFirerateMulti -= summationFirerateReduction;
            if (summationFirerateMulti < 0)
            {
                if (!summationMaxed)
                {
                    summationFirerateMulti = 0;
                }
                else if (summationFirerateMulti < -1)
                {
                    summationFirerateMulti = -1;
                }
            }
        }
    }

    private void UpdateSummationText()
    {
        if (summationFirerateRampMax > 0)
        {
            summationText.SetActive(true);
            summationText.GetComponent<TextMeshProUGUI>().text = "Case[notFiring]{<color=#FF5555>[fBuff<color=\"white\">=" + (100 * summationFirerateRampMax).ToString() + "%x(Δt/" + (1.5f + summationFirerateRampMax / 2).ToString() + ")]}(" + (summationFirerateMulti * 100).ToString("F0") + "%)";
        }
        else
        {
            summationText.SetActive(false);
        }
    }
}
