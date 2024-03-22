using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    [Header("Animation")]
    public Animator anim;
    public GameObject deathParticles;
    [Header("Movement")]
    public Rigidbody rb;
    public float moveSpeed = 5f;
    public float activationTime = 1f;
    public GameObject player;
    public bool activated = false;
    [Header("Hitpoints")]
    public Transform[] blocks;
    public float hitpointsPerBlock = 10f;
    public float blocksRemaining = 0f;
    public float currentBlockHitpoints = 10f;
    public float blockDefaultSize = 0.25f;
    public Transform[] secondaryBlockObjects;
    public float[] blockObjectHideThresholds;

    [Header("UpdateAssistors")]
    public float index = 0f;
    public string movementType = "chase";
    public EnemyUpdateManager enemyUpdateManager;

    // OTHER
    public bool dead = false;
    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody>();
        player = GameObject.Find("Player");
        anim = GetComponent<Animator>();
        blocksRemaining = blocks.Length;
        currentBlockHitpoints = hitpointsPerBlock;
        enemyUpdateManager = GameObject.Find("EnemyUpdateManager").GetComponent<EnemyUpdateManager>();
        enemyUpdateManager.SetVariables(rb, dead, activated, moveSpeed, movementType, index);
        StartCoroutine(Activate());
    }

    IEnumerator Activate(){
        activated = false;
        enemyUpdateManager.SetVariables(rb, dead, activated, moveSpeed, movementType, index);
        yield return new WaitForSeconds(activationTime);
        activated = true;
        enemyUpdateManager.SetVariables(rb, dead, activated, moveSpeed, movementType, index);
    }

    public void HitByBullet(float damage)
    {
        TakeDamage(damage);
    }

    public void TakeDamage(float damage){
        if(dead || !activated){return;}
        float damageOverflow = damage-currentBlockHitpoints;
        currentBlockHitpoints -= damage;
        if(currentBlockHitpoints <= 0){
            blocksRemaining--;
            if(blocksRemaining <= 0){
                dead = true;
                enemyUpdateManager.SetVariables(rb, dead, activated, moveSpeed, movementType, index);
                anim.SetTrigger("Dead");
            }
            else{
                currentBlockHitpoints = hitpointsPerBlock;
                TakeDamage(damageOverflow);
            }
        }
        ScaleBlockSizes();
    }

    private void ScaleBlockSizes(){
        if(currentBlockHitpoints > 0){
            float scale = currentBlockHitpoints / hitpointsPerBlock * blockDefaultSize;
            blocks[blocks.Length - 1].localScale = new Vector3(scale, scale, scale);
        }
        for(int i = 0; i < blocks.Length; i++){
            if(i > blocksRemaining - 1){
                blocks[i].localScale = new Vector3(0f, 0f, 0f);
            }
            else if(i < blocksRemaining - 1){
                blocks[i].localScale = new Vector3(blockDefaultSize, blockDefaultSize, blockDefaultSize);
            }
        }
        for(int i = 0; i < secondaryBlockObjects.Length; i++){
            if(blocksRemaining <= blockObjectHideThresholds[i]){
                secondaryBlockObjects[i].gameObject.SetActive(false);
            }
            else{
                secondaryBlockObjects[i].gameObject.SetActive(true);
            }
        }
    }

    // Update is called once per frame
    // void Update()
    // {
    //     //also make a script to assist with movement (reduce lag)
    //     Movement();
    // }

    // private void Movement(){
    //     if(dead){
    //         rb.velocity = Vector3.zero;
    //         return;
    //     }
    //     if(activated){
    //         Vector3 direction = player.transform.position - transform.position;
    //         rb.velocity = direction.normalized * moveSpeed;
    //     }
    // }

    public void DieAndDestroy(){
        enemyUpdateManager.EnemyDestroyed(index);
        Instantiate(deathParticles, transform.position, Quaternion.identity);
        Destroy(gameObject);
    }
}
