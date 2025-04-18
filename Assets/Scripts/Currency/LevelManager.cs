using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class LevelManager : MonoBehaviour
{
    [Header("References")]
    public Slider xpSlider;
    public ParticleSystem levelUpParticles;
    public TextMeshProUGUI upgradePointsText;
    public Animator upgradeMenuAnimator;
    [Header("Variables")]
    public float upgradePoints = 0f;
    public float xpPerUP = 100f;
    private float xp = 0f;
    private bool upgrading = false;
    public float upgradeCooldownTime = 1f;
    public float upgradeAnimationTime = 0.5f;
    public float barLerpSpeed = 0.2f;
    public float xpBarValue = 0f;
    private bool upgradeMenuShowing = false;
    private bool menuTransition = false;
    [Header("Levels & Logic")]
    public float pointsSpent = 0f;
    public float costMultiplier = 1f;
    public float logarithmIncrease = 5f;

    void Start(){
        upgrading = false;
    }
    public void GetXp(float xpAmount){
        xp += xpAmount;
    }

    IEnumerator UpgradeCooldown(){
        upgrading = true;
        if(xp < xpPerUP * 3f){
            yield return new WaitForSecondsRealtime(0.2f);
        }
        else if(xp < xpPerUP * 20f){
            yield return new WaitForSecondsRealtime(0.2f - (xp / (xpPerUP * 20f)) * 0.18f);
        }
        else{
            yield return new WaitForSecondsRealtime(0.001f);
        }
        levelUpParticles.Play();
        float tempAnimationTime = upgradeAnimationTime;
        if(xp < xpPerUP * 3f){
            yield return new WaitForSecondsRealtime(upgradeAnimationTime);
        }
        xpBarValue = 0f;
        xp -= xpPerUP;
        upgradePoints++;
        if(xp > xpPerUP * 50f){
            int levelsToAdd = Mathf.FloorToInt(xp / xpPerUP / 50);
            xp -= xpPerUP * levelsToAdd;
            upgradePoints += levelsToAdd;
        }
        yield return new WaitForSecondsRealtime(xpPerUP / (xp + xpPerUP) * upgradeCooldownTime + 0.001f);
        upgrading = false;
    }

    private void GetUP(){
        if(xp >= xpPerUP && upgrading == false){
            StartCoroutine(UpgradeCooldown());
        }
    }

    private void LerpSlider(){
        xpSlider.value = Mathf.Lerp(xpSlider.value, xp / xpPerUP, barLerpSpeed);
    }

    private void GetXpBarValue(){
        if(!upgrading){
            xpBarValue = xp / xpPerUP;
            if(xpBarValue > 1){
                xpBarValue = 1;
            }
        }
    }

    private void UpdateUpgradePointsText(){
        upgradePointsText.text = upgradePoints.ToString() + " Upgrade Points";
    }

    private void UpgradeMenu(){
        upgradeMenuAnimator.SetBool("Showing", upgradeMenuShowing);
        //check 'e' key down
        if(Input.GetKeyDown(KeyCode.Space) && !menuTransition){
            upgradeMenuShowing = !upgradeMenuShowing;
            menuTransition = true;
            StartCoroutine(MenuTransition());
        }
    }

    public void ShowUpgradeMenu(){
        if(!menuTransition){
            upgradeMenuShowing = true;
            menuTransition = true;
            StartCoroutine(MenuTransition());
        }
    }

    IEnumerator MenuTransition(){
        Time.timeScale = 0.75f;
        yield return new WaitForSecondsRealtime(0.25f);
        if(upgradeMenuShowing){
            Time.timeScale = 0f;
            yield return new WaitForSecondsRealtime(0.9f);
        }
        else{
            yield return new WaitForSecondsRealtime(0.9f);
            Time.timeScale = 1f;
        }
        menuTransition = false;
    }

    public bool CheckPrice(float amount){
        if(upgradePoints >= amount){
            return true;
        }
        return false;
    }

    public void SpendCurrency(float amount){
        upgradePoints -= amount;
        pointsSpent += amount;
    }

    public float GetMultiplier(){
        CalculateMultiplier();
        return costMultiplier;
    }

    private void CalculateMultiplier(){
        if(pointsSpent <= 0){
            costMultiplier = 1f;
            return;
        }
        costMultiplier = Mathf.Log(pointsSpent, logarithmIncrease) + 2;
    }

    // Update is called once per frame
    void Update()
    {
        GetUP();
        LerpSlider();
        GetXpBarValue();
        UpdateUpgradePointsText();
        UpgradeMenu();
        CalculateMultiplier();
    }
}
