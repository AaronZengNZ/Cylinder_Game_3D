using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DamageHitbox : MonoBehaviour
{
    public float damage = 5f;
    public float cooldown = 1f;
    bool canAttack = true;
    void OnTriggerEnter(Collider other){
        if(other.gameObject.tag == "Player" && canAttack){
            other.gameObject.GetComponent<PlayerHealth>().TakeDamage(damage);
            StartCoroutine(AttackCooldown());
        }
    }

    IEnumerator AttackCooldown(){
        canAttack = false;
        yield return new WaitForSeconds(cooldown);
        canAttack = true;
    }
}
