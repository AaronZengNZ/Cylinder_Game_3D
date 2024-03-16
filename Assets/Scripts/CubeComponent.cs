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
    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "PlayerBullet" && hasTarget == false)
        {
            BulletScript bulletScript = other.gameObject.GetComponent<BulletScript>();
            TakeDamage(bulletScript.damage);
            bulletScript.HitEnemy();
        }
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
        cubeComponentManager.ComponentDestroyed(index);
    }
}
