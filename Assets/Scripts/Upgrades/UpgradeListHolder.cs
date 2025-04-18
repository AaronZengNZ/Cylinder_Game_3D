using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UpgradeListHolder : MonoBehaviour
{
    public UpgradeList[] upgradeLists;
    public UpgradeList baseList;
    public UpgradeList selectedClass;
    public GameObject[] uniqueUpgrades;
    public GameObject[] specialUpgrades;
    public float specialUpgradeLevel = 0f;
    public GameObject[] upgrades;
    public float[] upgradeLevels;

    public float uniqueUpgradesGot = 0f;
    // Start is called before the first frame update
    void Start()
    {
        BaseListUniques();
        BaseListSetUpgrades();
        SelectClass("trigonometry");
        SelectedClassSpecials();
        //AddNewUpgradesToList(selectedClass);
    }

    public void UpgradeLevelUp(GameObject upgrade)
    {
        GameObject upgradeInArray = null;
        foreach (GameObject upgradeInList in upgrades)
        {
            UpgradeButton upgradeButton = upgradeInList.GetComponent<UpgradeButton>();
            UpgradeButton upgradeButtonUpgrade = upgrade.GetComponent<UpgradeButton>();
            if (upgradeButton.upgradeName == upgradeButtonUpgrade.upgradeName)
            {
                upgradeInArray = upgradeInList;
            }
        }

        if (upgradeInArray == null && upgrade.GetComponent<UpgradeButton>().upgradeType == "theory")
        {
            foreach (GameObject upgradeInList in uniqueUpgrades)
            {
                UpgradeButton upgradeButton = upgradeInList.GetComponent<UpgradeButton>();
                UpgradeButton upgradeButtonUpgrade = upgrade.GetComponent<UpgradeButton>();
                if (upgradeButton.upgradeName == upgradeButtonUpgrade.upgradeName)
                {
                    upgradeInArray = upgradeInList;
                }
            }
        }

        //if the upgrade is a unique upgrade, add it to the standard list.
        if (System.Array.IndexOf(uniqueUpgrades, upgradeInArray) != -1 &&
            System.Array.IndexOf(upgrades, upgradeInArray) == -1)
        {
            AddUniqueUpgradeToList(upgrade);
            uniqueUpgradesGot++;
            return;
        }

        int index = System.Array.IndexOf(upgrades, upgradeInArray);
        UnityEngine.Debug.Log("index = " + index);
        upgradeLevels[index]++;
    }

    private void BaseListSetUpgrades()
    {
        upgrades = baseList.upgrades;
        upgradeLevels = baseList.upgradeLevels;
    }

    public void SpecialUpgradeLevelUp(GameObject upgrade)
    {
        int index = System.Array.IndexOf(upgrades, upgrade);
        specialUpgradeLevel++;
    }

    private void BaseListUniques()
    {
        uniqueUpgrades = baseList.specialUpgrades;
    }

    private void SelectedClassSpecials()
    {
        specialUpgrades = selectedClass.specialUpgrades;
        specialUpgradeLevel = selectedClass.specialUpgradeLevel;
    }

    public void AddSpecialUpgradeToList(GameObject upgrade)
    {
        GameObject upgradeInSpecialUpgradeArray = null;
        foreach (GameObject specialUpgrade in specialUpgrades)
        {
            UpgradeButton upgradeButton = specialUpgrade.GetComponent<UpgradeButton>();
            UpgradeButton upgradeButtonUpgrade = upgrade.GetComponent<UpgradeButton>();
            if (upgradeButton.upgradeName == upgradeButtonUpgrade.upgradeName)
            {
                upgradeInSpecialUpgradeArray = specialUpgrade;
            }
        }
        // if(System.Array.IndexOf(specialUpgrades, upgrade) == -1){
        //     GameObject[] tempSpecialUpgrades = new GameObject[specialUpgrades.Length + 1];
        //     for(int i = 0; i < specialUpgrades.Length; i++){
        //         tempSpecialUpgrades[i] = specialUpgrades[i];
        //     }
        //     tempSpecialUpgrades[specialUpgrades.Length] = upgradeInSpecialUpgradeArray;
        //     specialUpgrades = tempSpecialUpgrades;
        // }
        GameObject[] tempUpgrades = new GameObject[upgrades.Length + 1];
        float[] tempUpgradeLevels = new float[upgradeLevels.Length + 1];
        for (int i = 0; i < upgrades.Length; i++)
        {
            tempUpgrades[i] = upgrades[i];
            tempUpgradeLevels[i] = upgradeLevels[i];
        }
        tempUpgrades[upgrades.Length] = upgradeInSpecialUpgradeArray;
        tempUpgradeLevels[upgradeLevels.Length] = 0f;
        upgrades = tempUpgrades;
        upgradeLevels = tempUpgradeLevels;
    }

    public void AddUniqueUpgradeToList(GameObject upgrade)
    {
        //finds the unique upgrade
        GameObject upgradeInSpecialUpgradeArray = null;
        foreach (GameObject specialUpgrade in uniqueUpgrades)
        {
            UpgradeButton upgradeButton = specialUpgrade.GetComponent<UpgradeButton>();
            UpgradeButton upgradeButtonUpgrade = upgrade.GetComponent<UpgradeButton>();
            if (upgradeButton.upgradeName == upgradeButtonUpgrade.upgradeName)
            {
                upgradeInSpecialUpgradeArray = specialUpgrade;
            }
        }
        //remove the unique upgrade from it's array
        GameObject[] tempUniqueUpgrades = new GameObject[uniqueUpgrades.Length - 1];
        for (int i = 0; i < uniqueUpgrades.Length; i++)
        {
            if (uniqueUpgrades[i] != upgradeInSpecialUpgradeArray)
            {
                tempUniqueUpgrades[i] = uniqueUpgrades[i];
            }
        }
        uniqueUpgrades = tempUniqueUpgrades;

        //if the upgrade is not in the list, add it to the list
        GameObject[] tempUpgrades = new GameObject[upgrades.Length + 1];
        float[] tempUpgradeLevels = new float[upgradeLevels.Length + 1];
        for (int i = 0; i < upgrades.Length; i++)
        {
            tempUpgrades[i] = upgrades[i];
            tempUpgradeLevels[i] = upgradeLevels[i];
        }
        tempUpgrades[upgrades.Length] = upgradeInSpecialUpgradeArray;
        tempUpgradeLevels[upgradeLevels.Length] = 1f;
        upgrades = tempUpgrades;
        upgradeLevels = tempUpgradeLevels;

        upgradeInSpecialUpgradeArray.GetComponent<UpgradeButton>().upgradeCostMultiplier *= Mathf.Pow(5f, uniqueUpgradesGot);
    }

    private void AddNewUpgradesToList(UpgradeList list)
    {
        int upgradeListLength = list.upgrades.Length + baseList.upgrades.Length;
        GameObject[] tempUpgrades = new GameObject[upgradeListLength];
        float[] tempUpgradeLevels = new float[upgradeListLength];
        for (int i = 0; i < baseList.upgrades.Length; i++)
        {
            tempUpgrades[i] = baseList.upgrades[i];
            tempUpgradeLevels[i] = baseList.upgradeLevels[i];
        }
        for (int i = 0; i < list.upgrades.Length; i++)
        {
            tempUpgrades[i] = list.upgrades[i];
            tempUpgradeLevels[i] = list.upgradeLevels[i];
        }
        list.upgrades = tempUpgrades;
        list.upgradeLevels = tempUpgradeLevels;
    }

    public void RemoveUpgradeFromList(GameObject Upgrade)
    {
        //remove the upgrade from the baselist.
        GameObject[] tempUpgrades = new GameObject[upgrades.Length - 1];
        float[] tempUpgradeLevels = new float[upgradeLevels.Length - 1];
    }

    public void SelectClass(string className)
    {
        foreach (UpgradeList list in upgradeLists)
        {
            if (list.className == className)
            {
                selectedClass = list;
            }
        }
    }

    public int GetLevelOfUpgrade(string upgradeType = "special", GameObject upgrade = null)
    {
        if (upgradeType == "special")
        {
            return (int)specialUpgradeLevel;
        }
        else
        {
            int index = System.Array.IndexOf(upgrades, upgrade);
            return (int)upgradeLevels[index];
        }
    }

    public GameObject[] GetRandomUpgrades(string extra = "none", float amount = 3f)
    {
        UnityEngine.Debug.Log("Got " + extra + " upgrades");
        if (extra == "special")
        {
            return specialUpgrades;
        }
        GameObject[] tempSelectedUpgrades;
        if (extra == "unique")
        {
            tempSelectedUpgrades = new GameObject[(int)amount];
            for (int i = 0; i < amount; i++)
            {
                tempSelectedUpgrades[i] = GetUniqueUpgrade(tempSelectedUpgrades);
            }
        }
        else
        {
            tempSelectedUpgrades = new GameObject[(int)amount];
            for (int i = 0; i < amount; i++)
            {
                tempSelectedUpgrades[i] = GetRandomUpgrade(tempSelectedUpgrades);
            }
        }
        return tempSelectedUpgrades;
    }

    private GameObject GetUniqueUpgrade(GameObject[] selectedUpgrades)
    {
        //get a random upgrade from the list of unique upgrades. repeat until it is not in the selected upgrades list   
        GameObject selectedUpgrade = uniqueUpgrades[Random.Range(0, uniqueUpgrades.Length)];
        int iterations = 0;
        while (System.Array.IndexOf(selectedUpgrades, selectedUpgrade) != -1)
        {
            selectedUpgrade = uniqueUpgrades[Random.Range(0, uniqueUpgrades.Length)];
            iterations++;
            if (iterations > 100)
            {
                return null;
                break;
            }
        }
        return selectedUpgrade;
    }

    private GameObject GetRandomUpgrade(GameObject[] selectedUpgrades)
    {
        //get a random upgrade from the list of upgrades. repeat until it is not in the selected upgrades list
        GameObject selectedUpgrade = upgrades[Random.Range(0, upgrades.Length)];
        int iterations = 0;
        bool whileCheck = true;
        while (whileCheck)
        {
            int index = Random.Range(0, upgrades.Length);
            selectedUpgrade = upgrades[index];
            iterations++;
            whileCheck = System.Array.IndexOf(selectedUpgrades, selectedUpgrade) != -1;
            UpgradeButton tempBtn = selectedUpgrade.GetComponent<UpgradeButton>();
            if (tempBtn.upgradeType == "special")
            {
                if (specialUpgradeLevel >= tempBtn.maxLevel)
                {
                    whileCheck = true;
                }
            }
            else if (upgradeLevels[index] >= tempBtn.maxLevel)
            {
                whileCheck = true;
            }
            if (iterations > 100)
            {
                return null;
                break;
            }
        }
        return selectedUpgrade;
    }
}
