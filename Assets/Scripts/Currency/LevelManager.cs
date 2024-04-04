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
    [Header("Variables")]
    public float upgradePoints = 0f;
    public float xpPerUP = 100f;
    private float xp = 0f;
    private bool upgrading = false;
    public float upgradeCooldownTime = 1f;
    public float upgradeAnimationTime = 0.5f;
    public float barLerpSpeed = 0.2f;
    public float xpBarValue = 0f;

    void Start(){
        upgrading = false;
    }
    public void GetXp(float xpAmount){
        xp += xpAmount;
    }

    IEnumerator UpgradeCooldown(){
        upgrading = true;
        if(xp < xpPerUP * 3f){
            yield return new WaitForSeconds(0.2f);
        }
        else if(xp  < xpPerUP * 10f){
            yield return new WaitForSeconds(0.2f - (xp / (xpPerUP * 10f)) * 0.15f);
        }
        else{
            yield return new WaitForSeconds(0.05f);
        }
        levelUpParticles.Play();
        float tempAnimationTime = upgradeAnimationTime;
        if(xp < xpPerUP * 3f){
            yield return new WaitForSeconds(upgradeAnimationTime);
        }
        xpBarValue = 0f;
        xp -= xpPerUP;
        upgradePoints++;
        yield return new WaitForSeconds(xpPerUP / (xp + xpPerUP) * upgradeCooldownTime + 0.01f);
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

    // Update is called once per frame
    void Update()
    {
        GetUP();
        LerpSlider();
        GetXpBarValue();
        UpdateUpgradePointsText();
    }
}
