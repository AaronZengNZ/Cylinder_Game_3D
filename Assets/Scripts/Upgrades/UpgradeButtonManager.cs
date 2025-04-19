using System.Linq.Expressions;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class UpgradeButtonManager : MonoBehaviour
{
    public UpgradeButton[] upgradeButtons;
    private UpgradeButton[] savedUpgradeButtons;
    private string savedUpgradeType = "normal";
    public LevelManager levelManager;
    public UpgradeListHolder upgradeListHolder;
    public NotificationHandler notificationHandler;
    public GameObject upgradeParent;
    public float upgradesToInstantiate = 3f;
    public bool upgrading = false;
    public bool doingUnique = false;
    private string currentUpgradeType = "normal";
    public TextMeshProUGUI upgradeTitle;
    public string normalUpgradeTitle = "Enhance the formula?";
    public string uniqueUpgradeTitle = "Discover a theory?";
    public string specialUpgradeTitle = "Select a fundamental.";
    public GameObject rerollButton;
    public TextMeshProUGUI rerollCostText;
    public float rerollCost = 1f;
    private bool rerollingEnabled = true;
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
        currentUpgradeType = type;
        if (fromUnique)
        {
            currentUpgradeType = savedUpgradeType;
        }
        //over the span of 0.25 seconds remove characters
        string currentUpgradeTitleText = upgradeTitle.text;
        string nextUpgradeTitleText = "";

        if (currentUpgradeType == "special")
        {
            nextUpgradeTitleText = specialUpgradeTitle;
            rerollingEnabled = false;
            rerollCostText.text = "Cannot Reroll.";
        }
        else if (currentUpgradeType == "unique")
        {
            nextUpgradeTitleText = uniqueUpgradeTitle;
            rerollingEnabled = true;
            rerollCostText.text = "Reroll - <color=#00FFFF> 1 free";
        }
        else
        {
            nextUpgradeTitleText = normalUpgradeTitle;
            rerollingEnabled = true;
            rerollCostText.text = "Reroll - <color=#00FFFF>" + Mathf.RoundToInt(rerollCost) + " UP";
        }

        if (currentUpgradeTitleText == nextUpgradeTitleText)
        {
            yield return new WaitForSecondsRealtime(0.5f);
        }
        else
        {
            if (currentUpgradeTitleText.Length > 0)
            {
                for (int i = 0; i < currentUpgradeTitleText.Length; i++)
                {
                    upgradeTitle.text = currentUpgradeTitleText.Substring(0, currentUpgradeTitleText.Length - i);
                    yield return new WaitForSecondsRealtime(0.5f / currentUpgradeTitleText.Length);
                }
            }
            for (int i = 0; i < nextUpgradeTitleText.Length; i++)
            {
                upgradeTitle.text = nextUpgradeTitleText.Substring(0, i + 1);
                yield return new WaitForSecondsRealtime(0.5f / nextUpgradeTitleText.Length);
            }
        }

        if (fromUnique)
        {
            upgradeButtons = new UpgradeButton[savedUpgradeButtons.Length];
            for (int i = 0; i < savedUpgradeButtons.Length; i++)
            {
                upgradeButtons[i] = savedUpgradeButtons[i];
                upgradeButtons[i].gameObject.SetActive(true);
            }
            currentUpgradeType = savedUpgradeType;
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
        savedUpgradeType = currentUpgradeType;
        //instantiate new upgrades
        doingUnique = true;
        StartCoroutine(InstantiateNewUpgrades("unique"));
    }

    public void TryReroll()
    {
        StartCoroutine(TryRerollCoroutine());
    }

    IEnumerator TryRerollCoroutine()
    {
        if(rerollingEnabled == false)
        {
            if(currentUpgradeType == "unique")
            {
                notificationHandler.NewNotification("You can only reroll theory upgrades once.");
            }
            else
            {
                notificationHandler.NewNotification("Cannot reroll special upgrades.");
            }
            yield break;
        }

        float tempRerollCost = rerollCost;
        if (currentUpgradeType == "unique")
        {
            tempRerollCost = 0;
        }
        yield return null;
        if (levelManager.CheckPrice(tempRerollCost))
        {
            //clear upgrades first
            for (int i = 0; i < upgradeButtons.Length; i++)
            {
                upgradeButtons[i].DisableButton();
            }

            yield return new WaitForSecondsRealtime(2f);

            for (int i = 0; i < upgradeButtons.Length; i++)
            {
                Destroy(upgradeButtons[i].gameObject);
            }

            StartCoroutine(InstantiateNewUpgrades(currentUpgradeType));

            if (currentUpgradeType == "unique")
            {
                rerollingEnabled = false;
                rerollCostText.text = "Cannot Reroll.";
                yield break;
            }
            levelManager.SpendCurrency(tempRerollCost);
            //double reroll cost
            rerollCost *= 2;
            rerollCostText.text = "Reroll - <color=#00FFFF>" + Mathf.RoundToInt(rerollCost) + " UP";
        }
        else
        {
            notificationHandler.NewNotification("Not enough upgrade points!");
        }
    }
}
