using System.Net.Mime;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class GameEnd : MonoBehaviour
{
    public Animator gameOverScreenAnimator;
    private bool gameOver = false;

    public GameObject upgradeTextDisplayPrefab;
    public GameObject upgradeTextDisplayParent;
    public UpgradeListHolder upgradeListHolder;

    public LevelManager levelManager;
    public CubeSpawner cubeSpawner;

    public TextMeshProUGUI finalPointsSpentText;
    public TextMeshProUGUI finalSegmentsDestroyedText;
    public TextMeshProUGUI finalDifficultyText;
    public TextMeshProUGUI finalScoreText;

    public float finalScore = 0f;
    // Start is called before the first frame update
    void Start()
    {
        gameOverScreenAnimator.SetBool("Over", false);
        upgradeListHolder = GameObject.Find("UpgradeListHolder").GetComponent<UpgradeListHolder>();
        levelManager = GameObject.Find("LevelManager").GetComponent<LevelManager>();
        cubeSpawner = GameObject.Find("CubeSpawner").GetComponent<CubeSpawner>();
        finalScore = 0f;
    }

    public void GameOver()
    {
        if (gameOver) return;
        gameOver = true;
        gameOverScreenAnimator.SetBool("Over", true);
        GetFinalUpgrades();
        float totalPointsSpent = levelManager.pointsSpent;
        float finalDifficulty = cubeSpawner.difficulty;
        float totalSegmentsDestroyed = cubeSpawner.segmentsDestroyed;

        finalPointsSpentText.text = $"Score for total <color=#00FFFF>UP spent</color>: {totalPointsSpent * 5} ({totalPointsSpent}x5)";
        finalScore += (totalPointsSpent * 5);
        finalSegmentsDestroyedText.text = $"Score for total <color=#FF0000>Segments Eliminated</color>: {Mathf.Pow(totalSegmentsDestroyed, 1.2f).ToString("F0")} ({totalSegmentsDestroyed}^1.2)";
        finalScore += Mathf.Floor(Mathf.Pow(totalSegmentsDestroyed, 1.2f));
        finalDifficultyText.text = $"Score for final <color=#FF00FF>Difficulty Reached</color>: {(10 * Mathf.Pow(finalDifficulty, 5)).ToString("F0")} (10x{finalDifficulty.ToString("F2")}^5)";
        finalScore += Mathf.Round((10 * Mathf.Pow(finalDifficulty, 5)));
        finalScoreText.text = $"Final Score: {finalScore.ToString("F0")}";
        StartCoroutine(WaitAndPauseTime());
    }

    IEnumerator WaitAndPauseTime()
    {
        Time.timeScale = 1f;
        yield return new WaitForSecondsRealtime(1f);
        //over 1 second slow down time to 0
        float timeScale = Time.timeScale;
        while (timeScale > 0f)
        {
            timeScale -= Time.unscaledDeltaTime;
            if(timeScale < 0f)
            {
                timeScale = 0f;
            }
            Time.timeScale = timeScale;
            yield return null;
        }
    }

    private void GetFinalUpgrades()
    {
        foreach (GameObject upgrade in upgradeListHolder.upgrades)
        {
            if (upgradeListHolder.GetLevelOfUpgrade(upgrade) > 0)
            {
                UpgradeButton upgradeButton = upgrade.GetComponent<UpgradeButton>();
                GameObject upgradeTextDisplay = Instantiate(upgradeTextDisplayPrefab, upgradeTextDisplayParent.transform);
                UpgradeTextDisplay upgradeTextDisplayComponent = upgradeTextDisplay.GetComponent<UpgradeTextDisplay>();
                upgradeTextDisplayComponent.upgradeName = upgradeButton.title;
                upgradeTextDisplayComponent.upgradeLevel = upgradeListHolder.GetLevelOfUpgrade(upgrade);
                upgradeTextDisplayComponent.upgradeType = upgradeButton.upgradeType;
                upgradeTextDisplayComponent.upgradePointsSpent = upgrade.GetComponent<UpgradeButton>().upgradePointsSpent;
            }
            else if (upgradeListHolder.GetLevelOfUpgrade("special", upgrade) > 0)
            {
                UpgradeButton upgradeButton = upgrade.GetComponent<UpgradeButton>();
                GameObject upgradeTextDisplay = Instantiate(upgradeTextDisplayPrefab, upgradeTextDisplayParent.transform);
                UpgradeTextDisplay upgradeTextDisplayComponent = upgradeTextDisplay.GetComponent<UpgradeTextDisplay>();
                upgradeTextDisplayComponent.upgradeName = upgradeButton.title;
                upgradeTextDisplayComponent.upgradeLevel = upgradeListHolder.GetLevelOfUpgrade("special", upgrade);
                upgradeTextDisplayComponent.upgradeType = "Special";
                upgradeTextDisplayComponent.upgradePointsSpent = upgrade.GetComponent<UpgradeButton>().upgradePointsSpent;
            }
        }
    }
}
