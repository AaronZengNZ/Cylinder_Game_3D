using System.Linq.Expressions;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UpgradeButtonManager : MonoBehaviour
{
    public UpgradeButton[] upgradeButtons;
    private UpgradeButton[] savedUpgradeButtons;
    public LevelManager levelManager;
    public UpgradeListHolder upgradeListHolder;
    public NotificationHandler notificationHandler;
    public GameObject upgradeParent;
    public float upgradesToInstantiate = 3f;
    public bool upgrading = false;
    public bool doingUnique = false;
    // Start is called before the first frame update
    void Start()
    {
        levelManager = GameObject.Find("LevelManager").GetComponent<LevelManager>();
        notificationHandler = GameObject.Find("NotificationHandler").GetComponent<NotificationHandler>();
        FindButtons();
        //StartCoroutine(InstantiateNewUpgrades("special"));
        StartCoroutine(InstantiateNewUpgrades("special"));
    }

    // Update is called once per frame
    void Update()
    {
        FindButtons();
        CheckButtons();
    }

    public void GetUpgrade(UpgradeButton upgradeTarget)
    {
        if (upgrading == false)
        {
            float upgradePrice = upgradeTarget.currentCost;
            if (levelManager.CheckPrice(upgradePrice))
            {
                levelManager.SpendCurrency(upgradePrice);
            }
            else
            {
                if (!Input.GetKeyDown(KeyCode.Space))
                {
                    notificationHandler.NewNotification("Not enough upgrade points!");
                }
                return;
            }
            upgrading = true;
            foreach (UpgradeButton button in upgradeButtons)
            {
                if (button != upgradeTarget)
                {
                    button.DisableButton();
                }
            }
            if (upgradeTarget.upgradeType == "special")
            {
                upgradeListHolder.AddSpecialUpgradeToList(upgradeTarget.selfPrefab);
                upgradeListHolder.SpecialUpgradeLevelUp(upgradeTarget.selfPrefab);
            }
            else
            {
                upgradeListHolder.UpgradeLevelUp(upgradeTarget.selfPrefab);
            }
            upgradeTarget.Upgrade();
        }
    }

    public void UpgradeFinished()
    {
        upgrading = false;
        StartCoroutine(InstantiateNewUpgrades("normal", doingUnique));
        doingUnique = false;
    }

    IEnumerator InstantiateNewUpgrades(string type, bool fromUnique = false)
    {
        yield return new WaitForSecondsRealtime(0.5f);
        if (fromUnique)
        {
            upgradeButtons = new UpgradeButton[savedUpgradeButtons.Length];
            for (int i = 0; i < savedUpgradeButtons.Length; i++)
            {
                upgradeButtons[i] = savedUpgradeButtons[i];
                upgradeButtons[i].gameObject.SetActive(true);
            }
        }
        else
        {
            UnityEngine.Debug.Log("Instantiating " + type + " upgrades");
            GameObject[] newUpgrades = upgradeListHolder.GetRandomUpgrades(type, upgradesToInstantiate);
            for (int i = 0; i < newUpgrades.Length; i++)
            {
                GameObject newUpgrade = Instantiate(newUpgrades[i], upgradeParent.transform);
                UpgradeButton newUpgradeButton = newUpgrade.GetComponent<UpgradeButton>();
                newUpgradeButton.level = upgradeListHolder.GetLevelOfUpgrade(newUpgradeButton.upgradeType, newUpgrades[i]);
                newUpgradeButton.UpdateCost(levelManager.GetMultiplier());
            }
        }
    }

    private void CheckButtons()
    {
        bool noNull = true;
        foreach (UpgradeButton button in upgradeButtons)
        {
            if (button == null)
            {
                noNull = false;
            }
        }
        if (noNull == false)
        {
            FindButtons();
        }
    }

    private void FindButtons()
    {
        //find tag upgrade button
        GameObject[] buttons = GameObject.FindGameObjectsWithTag("UpgradeButton");
        upgradeButtons = new UpgradeButton[buttons.Length];
        for (int i = 0; i < buttons.Length; i++)
        {
            upgradeButtons[i] = buttons[i].GetComponent<UpgradeButton>();
        }
    }

    public void GetMinibossUpgrade()
    {
        //clear instantiated upgrades
        savedUpgradeButtons = new UpgradeButton[upgradeButtons.Length];
        for (int i = 0; i < upgradeButtons.Length; i++)
        {
            savedUpgradeButtons[i] = upgradeButtons[i];
            upgradeButtons[i].gameObject.SetActive(false);
        }
        upgradeButtons = new UpgradeButton[0];
        //instantiate new upgrades
        doingUnique = true;
        StartCoroutine(InstantiateNewUpgrades("unique"));
    }
}
