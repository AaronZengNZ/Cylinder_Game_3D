using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyUpdateManager : MonoBehaviour
{
    public Transform player;
    public GameObject[] enemies = new GameObject[10000];
    private Rigidbody[] rbs = new Rigidbody[10000];
    private bool[] deadBooleans = new bool[10000];
    private bool[] activatedBooleans = new bool[10000];
    private float[] movementSpeeds = new float[10000];
    private string[] movementTypes = new string[10000];
    void Start(){
        player = GameObject.Find("Player").transform;
    }

    void Update()
    {
        FindEnemies();
        UpdateEnemies();
    }

    public void SetVariables(Rigidbody rb, bool dead, bool activated, float moveSpeed, string movementType, float index){
        rbs[(int)index] = rb;
        deadBooleans[(int)index] = dead;
        activatedBooleans[(int)index] = activated;
        movementSpeeds[(int)index] = moveSpeed;
        movementTypes[(int)index] = movementType;
    }

    private void UpdateEnemies(){
        for(int i = 0; i < enemies.Length; i++){
            if(enemies[i] != null){
                MoveEnemy(enemies[i].GetComponent<Enemy>().index);
            }
        }
    }
    // NOTE TO SELF: TO REDUCE LAG, THE ENEMY SCRIPT'S VARIABLES CANNOT BE ACCESSED SINCE THIS ALSO CAUSES LAG. FIX LATER
    private void MoveEnemy(float index){
        int realIndex = (int)index;
        if(deadBooleans[realIndex] && enemies[realIndex] != null && rbs[realIndex] != null){
            rbs[realIndex].velocity = Vector3.zero;
            return;
        }
        else if(activatedBooleans[realIndex] && rbs[realIndex] != null){
            if(movementTypes[realIndex] == "chase"){
                Vector3 direction = player.position - enemies[realIndex].transform.position;
                rbs[realIndex].velocity = direction.normalized * movementSpeeds[realIndex];
            }
        }
    }

    private void FindEnemies(){
        GameObject[] tempEnemies = GameObject.FindGameObjectsWithTag("Enemy");
        //int enemiesLooped = 0;
        //use a for loop to check enemies array for nulls and replace with new ones
        for(int i = 0; i < tempEnemies.Length; i++){
            if(tempEnemies[i].GetComponent<Enemy>().index == 0){
                tempEnemies[i].GetComponent<Enemy>().index = FindFirstEmptySlotAndReturnIndex(tempEnemies[i]);
                tempEnemies[i].GetComponent<Enemy>().enemyUpdateManager = this;
            }
        }
    }

    private int FindFirstEmptySlotAndReturnIndex(GameObject target){
        for(int i = 0; i < enemies.Length; i++){
            if(enemies[i] == null){
                enemies[i] = target;
                return i;
            }
        }
        return -1;
    }

    public void EnemyDestroyed(float index){
        enemies[(int)index] = null;
    }
}
