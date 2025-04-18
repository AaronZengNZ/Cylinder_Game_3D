using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CustomAttack : MonoBehaviour
{
    public string AttackType = "Lazer";
    public ParticleSystem attack;
    public Image attackIndicator;
    public Collider attackCollider;
    public float indicationDuration = 1f;
    public float postAttackDuration = 0.2f;
    public float damage = 10f;
    public float yPos = 1f;
    public Transform player;
    public GameObject parent;
    private bool hit = false;

    public bool instantiateAtPlayer = false;
    public float randomOffset = 2f;
    // Start is called before the first frame update
    void Start()
    {
        transform.position = new Vector3(transform.position.x, yPos, transform.position.z);
        if (instantiateAtPlayer)
        {
            transform.position = new Vector3(player.position.x + Random.Range(-randomOffset, randomOffset), yPos, player.position.z + Random.Range(-randomOffset, randomOffset));
        }
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
        //slide the slider from 0 to 1
        float time = 0;
        while(time < indicationDuration){
            time += Time.deltaTime;
            if (time > indicationDuration / 2) { attackIndicator.fillAmount = 1; }
            else { attackIndicator.fillAmount = time / (indicationDuration / 2); }
            if(parent == null){Destroy(gameObject);}
            yield return null;
        }
        Attack();
        //slide the slider back
        time = 0;
        while(time < postAttackDuration){
            time += Time.deltaTime;
            attackIndicator.fillAmount = 1 - time / postAttackDuration;
            yield return null;
        }
        if (postAttackDuration < 0)
        {
            yield return new WaitForSeconds(-postAttackDuration);
        }
        //delete gameobject
        Destroy(gameObject);
    }

    public void Attack(){
        attack.Play();
        attackCollider.enabled = true;
    }

    private void OnTriggerEnter(Collider other) {
        if(hit){return;}
        if(other.tag == "Player"){
            other.GetComponent<PlayerHealth>().TakeDamage(damage);
            hit = true;
        }
    }
}
