using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    [Header("Animation")]
    public Animator anim;
    public GameObject deathParticles;
    public Material redMaterial;
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

    // OTHER
    public bool dead = false;

    [Header("Attack")]
    public float attackDamage = 10f;
    public GameObject rangedWeapon;
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
            blocks[i].GetComponent<Renderer>().material = redMaterial;
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
                blocks[i].GetComponent<Renderer>().material = redMaterial;
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

    private void DropParticles(){
        for(int i = 0; i < particlesDropped; i++){
            GameObject newParticle = Instantiate(xpParticle, transform.position, Quaternion.identity);
            ExperienceParticle particleScript = newParticle.GetComponent<ExperienceParticle>();
            particleScript.player = player;
            particleScript.experienceValue = xpDrop / particlesDropped;
        }
    }
}
