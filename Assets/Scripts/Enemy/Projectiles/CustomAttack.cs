using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CustomAttack : MonoBehaviour
{
    public string AttackType = "Lazer";
    public ParticleSystem attack;
    public Slider attackIndicator;
    public Collider attackCollider;
    public float indicationDuration = 1f;
    public float postAttackDuration = 0.2f;
    public float damage = 10f;
    public float yPos = 1f;
    public Transform player;
    // Start is called before the first frame update
    void Start()
    {
        transform.position = new Vector3(transform.position.x, yPos, transform.position.z);
        player = FindObjectOfType<Player>().gameObject.GetComponent<Transform>();
        attackCollider.enabled = false;
        if(AttackType == "Lazer"){
            StartCoroutine(Lazer());
        }
    }

    private void AimAtPlayer(){
        transform.LookAt(player);
        //y rotation 0. keep everything else
        transform.eulerAngles = new Vector3(0, transform.eulerAngles.y, transform.eulerAngles.z);
    }

    IEnumerator Lazer(){
        AimAtPlayer();
        attackIndicator.gameObject.SetActive(true);
        //slide the slider from 0 to 1
        float time = 0;
        while(time < indicationDuration / 2){
            time += Time.deltaTime;
            attackIndicator.value = time / (indicationDuration / 2);
            yield return null;
        }
        yield return new WaitForSeconds(indicationDuration / 2);
        Attack();
        //slide the slider back
        time = 0;
        while(time < postAttackDuration){
            time += Time.deltaTime;
            attackIndicator.value = 1 - time / postAttackDuration;
            yield return null;
        }
        //delete gameobject
        Destroy(gameObject);
    }

    public void Attack(){
        attack.Play();
        attackCollider.enabled = true;
    }

    private void OnTriggerEnter(Collider other) {
        if(other.tag == "Player"){
            //other.GetComponent<PlayerHealth>().TakeDamage(damage);
        }
    }
}
