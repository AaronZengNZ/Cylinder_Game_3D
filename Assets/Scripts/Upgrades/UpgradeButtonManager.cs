using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UpgradeButtonManager : MonoBehaviour
{
    public UpgradeButton[] upgradeButtons;
    public UpgradeListHolder upgradeListHolder;
    public GameObject upgradeParent;
    public float upgradesToInstantiate = 3f;
    public bool upgrading = false;
    // Start is called before the first frame update
    void Start()
    {
        FindButtons();
        InstantiateNewUpgrades("special");
    }

    // Update is called once per frame
    void Update()
    {
        CheckButtons();
    }

    public void GetUpgrade(UpgradeButton upgradeTarget){
        if(upgrading == false){
            upgrading = true;
            foreach(UpgradeButton button in upgradeButtons){
                if(button != upgradeTarget){
                    button.DisableButton();
                }
            }
            if(upgradeTarget.upgradeType == "special"){
                upgradeListHolder.AddSpecialUpgradeToList(upgradeTarget.selfPrefab);
            }
            upgradeListHolder.UpgradeLevelUp(upgradeTarget.selfPrefab);
            upgradeTarget.Upgrade();
        }
    }

    public void UpgradeFinished(){
        upgrading = false;
        InstantiateNewUpgrades("special");
    }

    private void InstantiateNewUpgrades(string type){
        GameObject[] newUpgrades = upgradeListHolder.GetRandomUpgrades(type, upgradesToInstantiate);
        for(int i = 0; i < newUpgrades.Length; i++){
            GameObject newUpgrade = Instantiate(newUpgrades[i], upgradeParent.transform);
        }
    }

    private void CheckButtons(){
        bool noNull = true;
        foreach(UpgradeButton button in upgradeButtons){
            if(button == null){
                noNull = false;
            }
        }
        if(noNull == false){
            FindButtons();
        }
    }

    private void FindButtons(){
        //find tag upgrade button
        GameObject[] buttons = GameObject.FindGameObjectsWithTag("UpgradeButton");
        upgradeButtons = new UpgradeButton[buttons.Length];
        for(int i = 0; i < buttons.Length; i++){
            upgradeButtons[i] = buttons[i].GetComponent<UpgradeButton>();
        }
    }
}
