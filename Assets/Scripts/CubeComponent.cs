using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CubeComponent : MonoBehaviour
{
    public CubeComponentManager cubeComponentManager;
    public float maxHp = 10f;
    public GameObject deathParticles;
    float hp = 10f;
    public bool hasTarget = false;
    public float index = 0;

    void Start(){
        //disable mesh renderer
        GetComponent<MeshRenderer>().enabled = false;
    }
    public void HitByBullet(float damage)
    {
        TakeDamage(damage);
    }

    private void TakeDamage(float damage)
    {
        if(hasTarget == true){return;}
        hp -= damage;
        if (hp <= 0)
        {
            Die();
        }
    }

    private void Die(){
        Instantiate(deathParticles, transform.position, Quaternion.identity);
        Destroy(gameObject);
    }

    private void OnDestroy(){
        if(cubeComponentManager != null){
            cubeComponentManager.ComponentDestroyed(index);
        }
    }
}
