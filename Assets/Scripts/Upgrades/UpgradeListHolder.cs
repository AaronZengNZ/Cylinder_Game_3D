using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UpgradeListHolder : MonoBehaviour
{
    public UpgradeList[] upgradeLists;
    public UpgradeList baseList;
    public UpgradeList selectedClass;
    public GameObject[] uniqueUpgrades;
    public float[] uniqueUpgradeLevels;
    public GameObject[] specialUpgrades;
    public float specialUpgradeLevel = 0f;
    public GameObject[] upgrades;
    public float[] upgradeLevels;
    // Start is called before the first frame update
    void Start()
    {
        BaseListUniques();
        SelectClass("trigonometry");
        SelectedClassSpecials();
        AddNewUpgradesToList(selectedClass);
    }

    public void UpgradeLevelUp(GameObject upgrade){
        int index = System.Array.IndexOf(upgrades, upgrade);
        upgradeLevels[index]++;
    }

    private void BaseListUniques(){
        uniqueUpgrades = baseList.specialUpgrades;
    }

    private void SelectedClassSpecials(){
        specialUpgrades = selectedClass.specialUpgrades;
        specialUpgradeLevel = selectedClass.specialUpgradeLevel;
    }

    public void AddSpecialUpgradeToList(GameObject upgrade){
        if(System.Array.IndexOf(specialUpgrades, upgrade) == -1){
            GameObject[] tempSpecialUpgrades = new GameObject[specialUpgrades.Length + 1];
            for(int i = 0; i < specialUpgrades.Length; i++){
                tempSpecialUpgrades[i] = specialUpgrades[i];
            }
            tempSpecialUpgrades[specialUpgrades.Length] = upgrade;
            specialUpgrades = tempSpecialUpgrades;
        }
        GameObject[] tempUpgrades = new GameObject[upgrades.Length + 1];
        float[] tempUpgradeLevels = new float[upgradeLevels.Length + 1];
        for(int i = 0; i < upgrades.Length; i++){
            tempUpgrades[i] = upgrades[i];
            tempUpgradeLevels[i] = upgradeLevels[i];
        }
        tempUpgrades[upgrades.Length] = upgrade;
        tempUpgradeLevels[upgradeLevels.Length] = 0f;
        upgrades = tempUpgrades;
        upgradeLevels = tempUpgradeLevels;
    }

    private void AddNewUpgradesToList(UpgradeList list){
        int upgradeListLength = list.upgrades.Length + baseList.upgrades.Length;
        GameObject[] tempUpgrades = new GameObject[upgradeListLength];
        float[] tempUpgradeLevels = new float[upgradeListLength];
        for(int i = 0; i < baseList.upgrades.Length; i++){
            tempUpgrades[i] = baseList.upgrades[i];
            tempUpgradeLevels[i] = baseList.upgradeLevels[i];
        }
        for(int i = 0; i < list.upgrades.Length; i++){
            tempUpgrades[i] = list.upgrades[i];
            tempUpgradeLevels[i] = list.upgradeLevels[i];
        }
        list.upgrades = tempUpgrades;
        list.upgradeLevels = tempUpgradeLevels;
    }

    public void SelectClass(string className){
        foreach(UpgradeList list in upgradeLists){
            if(list.className == className){
                selectedClass = list;
            }
        }
    }

    public GameObject[] GetRandomUpgrades(string extra = "none", float amount = 3f){
        if(extra == "special"){
            return specialUpgrades;
        }
        GameObject[] tempSelectedUpgrades;
        if(extra == "unique"){
            tempSelectedUpgrades = new GameObject[(int)amount];
            for(int i = 0; i < amount; i++){
                tempSelectedUpgrades[i] = GetUniqueUpgrade(tempSelectedUpgrades);
            }
        }
        else{
            tempSelectedUpgrades = new GameObject[(int)amount];
            for(int i = 0; i < amount; i++){
                tempSelectedUpgrades[i] = GetRandomUpgrade(tempSelectedUpgrades);
            }
        }
        return tempSelectedUpgrades;
    }

    private GameObject GetUniqueUpgrade(GameObject[] selectedUpgrades){
        //get a random upgrade from the list of unique upgrades. repeat until it is not in the selected upgrades list   
        GameObject selectedUpgrade = uniqueUpgrades[Random.Range(0, uniqueUpgrades.Length)];
        int iterations = 0;
        while(System.Array.IndexOf(selectedUpgrades, selectedUpgrade) != -1){
            selectedUpgrade = uniqueUpgrades[Random.Range(0, uniqueUpgrades.Length)];
            iterations++;
            if(iterations > 100){
                return null;
                break;
            }
        }
        return selectedUpgrade;
    }

    private GameObject GetRandomUpgrade(GameObject[] selectedUpgrades){
        //get a random upgrade from the list of upgrades. repeat until it is not in the selected upgrades list
        GameObject selectedUpgrade = upgrades[Random.Range(0, upgrades.Length)];
        int iterations = 0;
        while(System.Array.IndexOf(selectedUpgrades, selectedUpgrade) != -1){
            selectedUpgrade = upgrades[Random.Range(0, upgrades.Length)];
            iterations++;
            if(iterations > 100){
                return null;
                break;
            }
        }
        return selectedUpgrade;
    }
}
