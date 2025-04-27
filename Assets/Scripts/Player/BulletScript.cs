using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BulletScript : MonoBehaviour
{
    public Rigidbody rb;
    public float speed = 10f;
    public float damage = 10f;
    public float lifetime = 5f;
    public float yDirection = 0f;
    public float pierce = 1f;
    private float livedTime = 0f;
    public float percentageDamage = 0f;
    public bool damageElites = false;
    public bool noControl = false;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        Movement();
        DestroyCalculations();
    }

    public void HitEnemy(){
        pierce -= 1f;
        if(pierce <= 1f){
            Destroy(gameObject);
        }
    }

    private void DestroyCalculations(){
        livedTime += Time.deltaTime;
        if(livedTime > lifetime){
            Destroy(gameObject);
        }
    
    }

    private void Movement(){
        if(noControl){ return; }
        //rotate to change y direction
        if (yDirection != 0f)
        {
            //set direction to yDirection
            transform.rotation = Quaternion.Euler(0f, yDirection + 180, 0f);
        }
        rb.velocity = transform.forward * speed;
    }

    //use this to call enemy collide. i sleep now
    private void OnTriggerEnter(Collider other){
        CubeComponent cubeComponent = other.GetComponent<CubeComponent>();
        Enemy enemy = other.GetComponent<Enemy>();
        if(cubeComponent != null){
            cubeComponent.TakeDamage(damage, percentageDamage, damageElites);
            HitEnemy();
        }
        else if(enemy != null){
            enemy.TakeDamage(damage, percentageDamage, damageElites);
            HitEnemy();
        }
        else if(other.tag == "Wall"){
            Destroy(gameObject);
        }
    }
}
