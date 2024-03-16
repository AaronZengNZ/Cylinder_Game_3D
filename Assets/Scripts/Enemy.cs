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
    private bool activated = false;
    [Header("Hitpoints")]
    public Transform[] blocks;
    public float hitpointsPerBlock = 10f;
    public float blocksRemaining = 0f;
    public float currentBlockHitpoints = 10f;
    public float blockDefaultSize = 0.25f;
    public Transform[] secondaryBlockObjects;
    public float[] blockObjectHideThresholds;

    // OTHER
    private bool dead = false;
    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody>();
        player = GameObject.Find("Player");
        anim = GetComponent<Animator>();
        blocksRemaining = blocks.Length;
        currentBlockHitpoints = hitpointsPerBlock;
        StartCoroutine(Activate());
    }

    IEnumerator Activate(){
        activated = false;
        yield return new WaitForSeconds(activationTime);
        activated = true;
    }

    private void OnTriggerEnter(Collider other)
    {
        if(other.gameObject.tag == "PlayerBullet"){
            BulletScript bulletScript = other.gameObject.GetComponent<BulletScript>();
            TakeDamage(bulletScript.damage);
            bulletScript.HitEnemy();
        }
    }

    public void TakeDamage(float damage){
        if(dead || !activated){return;}
        currentBlockHitpoints -= damage;
        if(currentBlockHitpoints <= 0){
            blocksRemaining--;
            if(blocksRemaining <= 0){
                dead = true;
                anim.SetTrigger("Dead");
            }
            else{
                currentBlockHitpoints = hitpointsPerBlock;
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
    void Update()
    {
        Movement();
    }

    private void Movement(){
        if(dead){
            rb.velocity = Vector3.zero;
            return;
        }
        if(activated){
            Vector3 direction = player.transform.position - transform.position;
            rb.velocity = direction.normalized * moveSpeed;
        }
    }

    public void DieAndDestroy(){
        Instantiate(deathParticles, transform.position, Quaternion.identity);
        Destroy(gameObject);
    }
}
