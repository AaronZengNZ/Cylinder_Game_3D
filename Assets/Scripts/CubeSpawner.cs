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
    public GameObject cube;
    // Start is called before the first frame update
    void Start()
    {
        playerTransform = GameObject.Find("Player").transform;
        StartCoroutine(SpawnEnemies());
    }

    IEnumerator SpawnEnemies(){
        while(true){
            SpawnEnemy();
            yield return new WaitForSeconds(1f / spawnRate);
        }
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
        float xAxis = playerTransform.position.x;
        float zAxis = playerTransform.position.z;
        float yAxis = ySpawnHeight;
        if(spawnAxis == "x"){
            xAxis += Random.Range(-1,1)*xSpawnDistance;
            if(spawnDirection == "positive"){
                zAxis += zSpawnDistance;
            }
            else{
                zAxis -= zSpawnDistance;
            }
        }
        else if(spawnAxis == "z"){
            zAxis += Random.Range(-1,1)*zSpawnDistance;
            if(spawnDirection == "positive"){
                xAxis += xSpawnDistance;
            }
            else{
                xAxis -= xSpawnDistance;
            }
        }
        Vector3 spawnLocation = new Vector3(xAxis, yAxis, zAxis);
        Instantiate(cube, spawnLocation, Quaternion.identity);
    }
}