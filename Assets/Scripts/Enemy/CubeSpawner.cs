using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;
using UnityEngine.UI;
using TMPro;

public class CubeSpawner : MonoBehaviour
{
    public Transform playerTransform;
    public CubeComponentManager cubeComponentManager;
    public float xSpawnDistance = 10f;
    public float zSpawnDistance = 10f;
    public float ySpawnHeight = 0.65f;
    public float baseSpawnRate = 1f;
    public float spawnRate = 2f;
    public float baseSpawnRateWhenEmptySlots = 16f;
    public float spawnRateWhenEmptySlots = 4f;
    public float maxCubes = 200f;
    public GameObject cube;
    float timeElapsed = 0f;

    public float cubeHp = 10f;

    public Color stage1CubeColor = Color.red;
    public float stage1Gate = 10f;
    public Color stage2CubeColor = Color.yellow;
    public float stage2Gate = 20f;
    public Color stage3CubeColor = Color.green;
    public float stage3Gate = 50f;
    public Color stage4CubeColor = Color.blue;
    public float stage4Gate = 100f;
    public Color stage5CubeColor = Color.magenta;
    public float stage5Gate = 250f;

    public float xpMultiplier = 1f;

    public Material baseMaterial;

    public float difficulty = 1f;
    public float difficultyPerSecond = 0.01f;
    public float damageMultiplier = 1f;

    public float difficultyGateForHpScaling = 2f;

    public Image difficultyBar;
    public Image difficultyBackground;
    public TextMeshProUGUI difficultyText;
    public TextMeshProUGUI difficultyRampingText;
    public TextMeshProUGUI spawnRateText;
    public TextMeshProUGUI hitpointsText;
    public TextMeshProUGUI damageText;
    public TextMeshProUGUI xpMultiText;
    // Start is called before the first frame update
    void Start()
    {
        playerTransform = GameObject.Find("Player").transform;
        StartCoroutine(SpawnEnemies());
    }

    void Update()
    {
        difficulty += difficultyPerSecond * Time.deltaTime;
        UpdateDifficulty();
        UpdateUI();
    }

    IEnumerator SpawnEnemies()
    {
        while (true)
        {
            float tempSpawnRate;
            if (CheckAllEnemyCreatorsForEmptySlots())
            {
                tempSpawnRate = 1f / spawnRateWhenEmptySlots;
            }
            else
            {
                if (spawnRate == 0f)
                {
                    timeElapsed = 0f;
                    tempSpawnRate = 1f;
                }
                else
                {
                    tempSpawnRate = 1f / spawnRate;
                }
            }
            if (timeElapsed >= tempSpawnRate)
            {
                timeElapsed -= tempSpawnRate;
                if (GameObject.FindObjectsOfType<CubeComponent>().Length < maxCubes)
                {
                    SpawnEnemy();
                }
            }
            yield return new WaitForSeconds(0.05f);
            timeElapsed += 0.05f;
        }
    }

    private bool CheckAllEnemyCreatorsForEmptySlots()
    {
        GameObject[] enemyCreators = GameObject.FindGameObjectsWithTag("EnemyCreator");
        foreach (GameObject enemyCreator in enemyCreators)
        {
            EnemyCreator enemyCreatorScript = enemyCreator.GetComponent<EnemyCreator>();
            if (enemyCreatorScript.slotsTakenUp < enemyCreatorScript.slotsToInstantiate)
            {
                return true;
            }
        }
        return false;
    }

    private void SpawnEnemy()
    {
        string spawnAxis = "x";
        if (Random.Range(0, 2) >= 1)
        {
            spawnAxis = "z";
        }
        string spawnDirection = "positive";
        if (Random.Range(0, 2) >= 1)
        {
            spawnDirection = "negative";
        }
        float zAxis = 0f;
        float xAxis = 0f;
        float yAxis = ySpawnHeight;
        if (spawnAxis == "x")
        {
            xAxis += Random.Range(-1f, 1f) * xSpawnDistance;
            if (spawnDirection == "positive")
            {
                zAxis = zSpawnDistance;
            }
            else
            {
                zAxis = -zSpawnDistance;
            }
        }
        else if (spawnAxis == "z")
        {
            zAxis += Random.Range(-1f, 1f) * zSpawnDistance;
            if (spawnDirection == "positive")
            {
                xAxis = xSpawnDistance;
            }
            else
            {
                xAxis = -xSpawnDistance;
            }
        }
        Vector3 spawnLocation = new Vector3(xAxis, yAxis, zAxis);
        GameObject newCube = Instantiate(cube, spawnLocation, Quaternion.identity);
        
        newCube.transform.parent = transform;

        //set cube hp 
        CubeComponent cubeComponent = newCube.GetComponent<CubeComponent>();
        cubeComponent.maxHp = cubeHp;
        cubeComponent.xpDrop *= xpMultiplier;

        //set cube component manager
        cubeComponentManager.offensiveMaterial = GetMaterialForDifficulty();
    }

    public Color GetColorOfDifficulty(float amount)
    {
        // get the color of the cube based on amount, make it fade between colors
        if (amount <= stage1Gate)
        {
            return stage1CubeColor;
        }
        else if (amount <= stage2Gate)
        {
            return Color.Lerp(stage1CubeColor, stage2CubeColor, (amount - stage1Gate) / (stage2Gate - stage1Gate));
        }
        else if (amount <= stage3Gate)
        {
            return Color.Lerp(stage2CubeColor, stage3CubeColor, (amount - stage2Gate) / (stage3Gate - stage2Gate));
        }
        else if (amount <= stage4Gate)
        {
            return Color.Lerp(stage3CubeColor, stage4CubeColor, (amount - stage3Gate) / (stage4Gate - stage3Gate));
        }
        else if (amount <= stage5Gate)
        {
            return Color.Lerp(stage4CubeColor, stage5CubeColor, (amount - stage4Gate) / (stage5Gate - stage4Gate));
        }
        else
        {
            return stage5CubeColor;
        }
    }

    public Material GetMaterialForColor(Color color)
    {
        //make a copy of baseMaterial that is the correct color
        Material newMaterial = new Material(baseMaterial);
        newMaterial.color = color;
        //also set the albedo
        newMaterial.SetColor("_Color", color);
        newMaterial.SetColor("_EmissionColor", color);
        return newMaterial;
    }

    public Material GetMaterialForDifficulty()
    {
        Color color = GetColorOfDifficulty(difficulty);
        return GetMaterialForColor(color);
    }

    private void UpdateDifficulty()
    {
        float hpAndXpScaling = 1f;
        if (difficulty > difficultyGateForHpScaling)
        {
            hpAndXpScaling = 1 + difficulty - difficultyGateForHpScaling;
        }
        else
        {
            hpAndXpScaling = 1f;
        }
        cubeHp = 10f * Mathf.Pow(hpAndXpScaling, 2f);
        xpMultiplier = Mathf.Pow(hpAndXpScaling, 3f);
        spawnRateWhenEmptySlots = baseSpawnRateWhenEmptySlots * Mathf.Pow(difficulty, 0.75f);
        spawnRate = baseSpawnRate * Mathf.Pow(difficulty, 0.75f);
        damageMultiplier = 1f + (Mathf.Pow(hpAndXpScaling, 3f) - 1f) / 4f;
    }

    private void UpdateUI()
    {
        if (difficulty < stage1Gate)
        {
            difficultyBackground.color = Color.black;
            difficultyBar.color = stage1CubeColor;
        }
        else if (difficulty < stage2Gate)
        {
            difficultyBackground.color = stage1CubeColor;
            difficultyBar.color = stage2CubeColor;
        }
        else if (difficulty < stage3Gate)
        {
            difficultyBackground.color = stage2CubeColor;
            difficultyBar.color = stage3CubeColor;
        }
        else if (difficulty < stage4Gate)
        {
            difficultyBackground.color = stage3CubeColor;
            difficultyBar.color = stage4CubeColor;
        }
        else if (difficulty < stage5Gate)
        {
            difficultyBackground.color = stage4CubeColor;
            difficultyBar.color = stage5CubeColor;
        }
        else
        {
            difficultyBackground.color = stage5CubeColor;
            difficultyBar.color = stage5CubeColor;
        }
        difficultyBar.fillAmount = difficulty - Mathf.Floor(difficulty);
        difficultyText.text = $"diff={difficulty.ToString("F2")}";
        difficultyRampingText.text = $"[+{Math.Round(difficultyPerSecond, 2)}/s]";
        spawnRateText.text = $"spawn rate x{Math.Round(spawnRate / baseSpawnRate, 2)}";
        hitpointsText.text = $"segment hp x{Math.Round(cubeHp / 10f, 2)}";
        damageText.text = $"damage x{Math.Round(damageMultiplier, 2)}";
        xpMultiText.text = $"xp multi x{Math.Round(xpMultiplier, 2)}";
    }
}
