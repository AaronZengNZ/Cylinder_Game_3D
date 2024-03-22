using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class CubeSpawner : MonoBehaviour
{
    public Transform playerTransform;
    public float xSpawnDistance = 10f;
    public float zSpawnDistance = 10f;
    public float ySpawnHeight = 0.65f;
    public float spawnRate = 2f;
    public float spawnRateWhenEmptySlots = 4f;
    public float maxCubes = 200f;
    public GameObject cube;
    float timeElapsed = 0f;
    // Start is called before the first frame update
    void Start()
    {
        playerTransform = GameObject.Find("Player").transform;
        StartCoroutine(SpawnEnemies());
    }

    IEnumerator SpawnEnemies(){
        while(true){
            float tempSpawnRate;
            if(CheckAllEnemyCreatorsForEmptySlots()){
                Debug.Log("Found Empty Spawner");
                tempSpawnRate = 1f / spawnRateWhenEmptySlots;
            }
            else{
                Debug.Log("Did Not Find Empty Spawner");
                if(spawnRate == 0f){
                    timeElapsed = 0f;
                    tempSpawnRate = 1f;
                }
                else{
                    tempSpawnRate = 1f / spawnRate;
                }
            }
            if(timeElapsed >= tempSpawnRate){
                timeElapsed -= tempSpawnRate;
                if(GameObject.FindObjectsOfType<CubeComponent>().Length < maxCubes){
                    Debug.Log("Spawning Enemy");
                    SpawnEnemy();
                }
            }
            yield return new WaitForSeconds(0.05f);
            timeElapsed += 0.05f;
        }
    }

    private bool CheckAllEnemyCreatorsForEmptySlots(){
        GameObject[] enemyCreators = GameObject.FindGameObjectsWithTag("EnemyCreator");
        foreach(GameObject enemyCreator in enemyCreators){
            EnemyCreator enemyCreatorScript = enemyCreator.GetComponent<EnemyCreator>();
            if(enemyCreatorScript.slotsTakenUp < enemyCreatorScript.slotsToInstantiate){
                return true;
            }
        }
        return false;
    }

    private void SpawnEnemy(){
        string spawnAxis = "x";
        if(Random.Range(0, 2) >= 1){
            spawnAxis = "z";
        }
        string spawnDirection = "positive";
        if(Random.Range(0, 2) >= 1){
            spawnDirection = "negative";
        }
        float zAxis = 0f;
        float xAxis = 0f;
        float yAxis = ySpawnHeight;
        if(spawnAxis == "x"){
            xAxis += Random.Range(-1f,1f) * xSpawnDistance;
            Debug.Log(xAxis);
            if(spawnDirection == "positive"){
                zAxis = zSpawnDistance;
            }
            else{
                zAxis = -zSpawnDistance;
            }
        }
        else if(spawnAxis == "z"){
            zAxis += Random.Range(-1f,1f)*zSpawnDistance;
            Debug.Log(zAxis);
            if(spawnDirection == "positive"){
                xAxis = xSpawnDistance;
            }
            else{
                xAxis = -xSpawnDistance;
            }
        }
        Vector3 spawnLocation = new Vector3(xAxis, yAxis, zAxis);
        Instantiate(cube, spawnLocation, Quaternion.identity);
    }
}
