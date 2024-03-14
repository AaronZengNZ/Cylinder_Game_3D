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
    private float livedTime = 0f;
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
        Destroy(gameObject);
    }

    private void DestroyCalculations(){
        livedTime += Time.deltaTime;
        if(livedTime > lifetime){
            Destroy(gameObject);
        }
    
    }

    private void Movement(){
        //rotate to change y direction
        if(yDirection != 0f){
            //set direction to yDirection
            transform.rotation = Quaternion.Euler(0f, yDirection+180, 0f);
        }
        rb.velocity = transform.forward * speed;
    }
}
