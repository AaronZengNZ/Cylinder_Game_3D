using System.Runtime;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class Enemy : MonoBehaviour
{
    [Header("Animation")]
    public Animator anim;
    public GameObject deathParticles;
    public Material aliveMaterial;
    public Material grayMaterial;
    [Header("Movement")]
    public Rigidbody rb;
    public float moveSpeed = 5f;
    public float activationTime = 1f;
    public GameObject player;
    public bool activated = false;
    public bool canMove = true;
    [Header("Hitpoints")]
    public Transform[] blocks;
    public float hitpointsPerBlock = 10f;
    public float blocksRemaining = 0f;
    public float currentBlockHitpoints = 10f;
    public float blockDefaultSize = 0.25f;
    [Header("UpdateAssistors")]
    public float index = 0f;
    public string movementType = "chase";
    public EnemyUpdateManager enemyUpdateManager;
    public float customVar = 0f;
    public float customVar2 = 0f;
    [Header("Loot")]
    public float xpDrop = 60f;
    public float particlesDropped = 6f;
    public GameObject xpParticle;
    public float healDrop = 10f;
    public GameObject healingParticle;
    // OTHER
    public bool dead = false;

    [Header("Attack")]
    public string attackType = "melee";
    public float attackDamage = 10f;
    public float attackCooldown = 1f;
    public GameObject rangedWeapon;
    private bool canAttack = true;

    public bool collision = false;
    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody>();
        player = GameObject.Find("Player");
        anim = GetComponent<Animator>();
        blocksRemaining = blocks.Length;
        currentBlockHitpoints = hitpointsPerBlock;
        enemyUpdateManager = GameObject.Find("EnemyUpdateManager").GetComponent<EnemyUpdateManager>();
        enemyUpdateManager.SetVariables(rb, dead, activated, moveSpeed, movementType, index, customVar, customVar2);
        SetActivationOfColliders(false);
        StartCoroutine(Activate());
    }

    public void RangedAttack(){
        GameObject newRangedWeapon = Instantiate(rangedWeapon, transform.position, Quaternion.identity);
        CustomAttack attackScript = newRangedWeapon.GetComponent<CustomAttack>();
        attackScript.player = player.transform;
        attackScript.damage = attackDamage;
        attackScript.parent = this.gameObject;
    }

    IEnumerator Activate(){
        //grey all blocks
        for(int i = 0; i < blocks.Length; i++){
            blocks[i].GetComponent<Renderer>().material = grayMaterial;
        }
        activated = false;
        enemyUpdateManager.SetVariables(rb, dead, activated, moveSpeed, movementType, index, customVar, customVar2);
        yield return new WaitForSeconds(activationTime);
        for(int i = 0; i < blocks.Length; i++){
            blocks[i].GetComponent<Renderer>().material = aliveMaterial;
        }
        SetActivationOfColliders(true);
        activated = true;
        enemyUpdateManager.SetVariables(rb, dead, activated, moveSpeed, movementType, index, customVar, customVar2);
    }

    private void StopMoving(){
        enemyUpdateManager.SetVariables(rb, dead, activated, 0f, movementType, index, customVar, customVar2);
        rb.velocity = Vector3.zero;
        canMove = false;
    }

    private void StartMoving(){
        enemyUpdateManager.SetVariables(rb, dead, activated, moveSpeed, movementType, index, customVar, customVar2);
        canMove = true;
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
                enemyUpdateManager.SetVariables(rb, dead, activated, 0f, movementType, index, customVar, customVar2);
                anim.SetTrigger("Dead");
                SetActivationOfColliders(false);
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
        //    blocks[blocks.Length - 1].localScale = new Vector3(blockDefaultSize, blockDefaultSize, blockDefaultSize);
        }
        for(int i = 0; i < blocks.Length; i++){
            if(i > blocksRemaining - 1){
                blocks[i].GetComponent<Renderer>().material = grayMaterial;
            }
            else if(i < blocksRemaining - 1){
                blocks[i].GetComponent<Renderer>().material = aliveMaterial;
            }
        }
    }

    public void DieAndDestroy(){
        DropParticles();
        enemyUpdateManager.EnemyDestroyed(index);
        Instantiate(deathParticles, transform.position, Quaternion.identity);
        Destroy(gameObject);
    }

    private void SetActivationOfColliders(bool value){
        foreach(Collider col in GetComponents<Collider>()){
            col.enabled = value;
        }
    }

    private void DropParticles()
    {
        for (int i = 0; i < particlesDropped; i++)
        {
            GameObject newParticle = Instantiate(xpParticle, transform.position, Quaternion.identity);
            ExperienceParticle particleScript = newParticle.GetComponent<ExperienceParticle>();
            particleScript.player = player;
            particleScript.experienceValue = xpDrop / particlesDropped;
        }
        for (int i = 0; i < 1 + Math.Floor(healDrop / 5); i++)
        {
            GameObject newParticle = Instantiate(healingParticle, transform.position, Quaternion.identity);
            ExperienceParticle particleScript = newParticle.GetComponent<ExperienceParticle>();
            particleScript.player = player;
            particleScript.healValue = healDrop / (1 + Mathf.Floor((healDrop / 5)));
        }
    }

    private void OnTriggerEnter(Collider other) {
        if(other.tag == "Player"){
            if(!collision){
                if(attackType == "melee"){
                    other.GetComponent<PlayerHealth>().TakeDamage(attackDamage);
                }
                if(other.GetComponent<Player>().moving){
                    TakeDamage(other.GetComponent<PlayerHealth>().defence * 10f);
                }
                collision = true;
                StartCoroutine(CollisionCooldown());
            }
        }
    }

    IEnumerator CollisionCooldown(){
        //apply force to self, away from player, on the x and z axis
        Vector3 direction = (transform.position - player.transform.position).normalized;
        rb.velocity = new Vector3(direction.x * 40f, rb.velocity.y, direction.z * 40f);
        SetActivationOfColliders(false);
        yield return new WaitForSeconds(0.2f);
        SetActivationOfColliders(true);
        yield return new WaitForSeconds(0.3f);
        collision = false;
    }
}
