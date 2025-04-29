using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class PlayerHealth : MonoBehaviour
{
    public float maxHitpoints = 100f;
    public float defence = 0f;
    public float armor = 0f;
    public float regeneration = 2f;
    public StatsManager statsManager;
    public Player player;
    [SerializeField] private float hitpoints = 100f;

    //references
    public Image healthBar;
    public TextMeshProUGUI healthText;
    public TextMeshProUGUI regenerationText;
    public TextMeshProUGUI defenceText;
    public TextMeshProUGUI armorText;

    public bool dead = false;
    // Start is called before the first frame update
    void Start()
    {
        hitpoints = maxHitpoints;
        StartCoroutine(RegenerationTimer());
        player = GetComponent<Player>();
    }

    void Update()
    {
        GetVariables();
        healthBar.fillAmount = hitpoints / maxHitpoints;
        healthText.text = $"<color=#FF5555>{Mathf.Ceil(hitpoints)}/{maxHitpoints}";
        regenerationText.text = $"[+r({Mathf.Ceil(regeneration)})/s]";
        defenceText.text = $"d={Mathf.Ceil(defence)}";
        armorText.text = $"a={Mathf.Ceil(armor)}%";
    }

    public void GetVariables()
    {
        defence = statsManager.GetStatFloat("defence");
        armor = statsManager.GetStatFloat("armor");
        maxHitpoints = statsManager.GetStatFloat("playerHp");
    }

    public void TakeDamage(float amount){
        if(dead){ hitpoints = 0; return; }
        float armorMulti = (1 - armor / 100);
        if(armorMulti < 0.1f){
            armorMulti = 0.1f;
        }
        float trueDamage = (amount - defence);
        if(trueDamage < 10f){
            trueDamage = 10f * armorMulti; //minimum amount of damage
            //blocked.
        }
        else{
            trueDamage *= armorMulti;
            hitpoints -= trueDamage;
            if(hitpoints <= 0){
                hitpoints = 0;
                Die();
            }
        }
    }

    public void Heal(float amount)
    {
        hitpoints += amount / 100f * maxHitpoints;
        if(hitpoints > maxHitpoints){
            hitpoints = maxHitpoints;
        }
    }

    private void Die()
    {
        if(dead){ return; }
        //die
        player.dead = true;
        dead = true;
        player.anim.SetBool("Dead", true);
    }

    IEnumerator RegenerationTimer(){
        while(true){
            yield return new WaitForSeconds(1);
            Regenerate(regeneration);
        }
    }

    public void Regenerate(float amount){
        if(dead){ return; }
        hitpoints += amount;
        if(hitpoints > maxHitpoints){
            hitpoints = maxHitpoints;
        }
    }
}
