using System.Numerics;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Vector2 = UnityEngine.Vector2;
using Vector3 = UnityEngine.Vector3;
using Quaternion = UnityEngine.Quaternion;

public class ExperienceParticle : MonoBehaviour
{
    [Header("References")]
    public Rigidbody rb;
    public GameObject player;
    public LevelManager levelManager;
    [Header("Particle Logic")]
    public float flingSpeed = 5f;
    public float verticalFlingSpeed = 5f;
    public float gravity = 10f;
    public float speed = 2f;
    public float deceleration = 10f;
    public float baseSpeed = 5f;
    public float acceleration = 10f;
    public float pickupRadius = 5f;
    public float directionAngle = 1f;
    public float yPos = 0.8f;
    private bool idle = true;
    [Header("Particle Transform")]
    public float maxSize = 0.3f;
    public float minSize = 0.1f;
    public Vector3 rigidbodyVelocity;
    [Header("Variables")]
    public float experienceValue = 5f;
    public float pickupCooldown = 0.5f;
    private bool canPickup = false;
    private bool collected = false;
    private float randomScale;

    public float healValue = 0f;
    public bool minibossUpgrade = false;

    public float randomSizeModifier = 0f;
    // Start is called before the first frame update
    void Start()
    {
        player = GameObject.Find("Player");
        rb = GetComponent<Rigidbody>();
        levelManager = GameObject.Find("LevelManager").GetComponent<LevelManager>();
        idle = true;
        speed = flingSpeed * Random.Range(0.5f, 1.5f);
        transform.position = new Vector3(transform.position.x, yPos, transform.position.z);
        //scale

        RandomDirection();
        RandomTransform();
        UpFling();
        StartCoroutine(Cooldown());
    }

    IEnumerator Cooldown(){
        canPickup = false;
        yield return new WaitForSeconds(pickupCooldown);
        canPickup = true;
    }

    private void RandomTransform(){
        //set random scale
        randomScale = Random.Range(minSize, maxSize);
       //increase randomScale by the square root of experience value / 10
        randomScale += (Mathf.Sqrt((Mathf.Pow(experienceValue, 0.4f) / 10 - 0.1f) / 2.5f + 1) - 1) * Random.Range(0.7f, 1.1f);
        //set random scale
        randomScale += Random.Range(randomSizeModifier * 0.8f, randomSizeModifier * 1.2f);
        //add this randomsacle to transform scale
        transform.localScale = new Vector3(randomScale, randomScale, randomScale);
        //set random rotation
        transform.rotation = Quaternion.Euler(Random.Range(0f, 360f), Random.Range(0f, 360f), Random.Range(0f, 360f));
    }

    private void UpFling(){
        float randomUpFling = verticalFlingSpeed * Random.Range(0.5f, 1f);
        rb.velocity = new Vector3(rb.velocity.x, randomUpFling, rb.velocity.z);
    }

    private void RandomDirection(){
        directionAngle = Random.Range(0f, 360f);
    }

    private void PointTowardsPlayer(){
        if(!idle){
            //Finish tommorow. Particle will start at stasis and fling, then chase down the player with acceleration.
            Vector3 tempDirection = (player.transform.position - transform.position).normalized;
            Vector3 direction = new Vector3(tempDirection.x, 0, tempDirection.z);
            directionAngle = Mathf.Atan2(direction.z, direction.x) * Mathf.Rad2Deg;
        }
    }

    private void Fling(){
        idle = true;
    }

    private void Accelerate(){
        if(!idle && !collected){
            speed += acceleration * Time.deltaTime;
            if(speed < baseSpeed){
                speed = baseSpeed;
            }
            ChasePlayer();
        }
        else{
            speed -= deceleration * Time.deltaTime;
            if(speed <= 0){
                speed = 0;
            }
        }
        rb.velocity = new Vector3(rb.velocity.x, rb.velocity.y - gravity * Time.deltaTime, rb.velocity.z);
    }

    private void ChasePlayer(){
        gravity = 0f;
        rb.velocity = new Vector3(rb.velocity.x, yPos - transform.position.y, rb.velocity.z);
        //make rigidbody spin
        float spinSpeed = 36f / randomScale;
        rb.angularVelocity = new Vector3(spinSpeed, spinSpeed, spinSpeed);
    }

    private void Move(){
        if(speed <= 0 || collected){
            rb.velocity = new Vector3(0, rb.velocity.y, 0);
            return;
        }
        rb.velocity = new Vector3(Mathf.Cos(directionAngle * Mathf.Deg2Rad) * speed, rb.velocity.y, Mathf.Sin(directionAngle * Mathf.Deg2Rad) * speed);
    }

    private void CheckPickupRange()
    {
        if (canPickup == false) { return; }
        float distance = 0f;
        Vector2 playerPos = new Vector2(player.transform.position.x, player.transform.position.z);
        Vector2 particlePos = new Vector2(transform.position.x, transform.position.z);
        distance = Vector2.Distance(playerPos, particlePos);
        if (distance <= pickupRadius)
        {
            idle = false;
        }
        if (pickupRadius == 0f)
        {
            idle = false;
        }
    }

    private void CheckCollect(){
        if(!idle && !collected){
            float distance = 0f;
            Vector2 playerPos = new Vector2(player.transform.position.x, player.transform.position.z);
            Vector2 particlePos = new Vector2(transform.position.x, transform.position.z);
            distance = Vector2.Distance(playerPos, particlePos);
            if(distance <= speed * Time.deltaTime * 2f){
                levelManager.GetXp(experienceValue);

                if (healValue > 0)
                {
                    player.GetComponent<PlayerHealth>().Heal(healValue);
                }

                if (minibossUpgrade)
                {
                    UpgradeButtonManager upgradeButtonManager = GameObject.Find("UpgradeButtonManager").GetComponent<UpgradeButtonManager>();
                    upgradeButtonManager.GetMinibossUpgrade();
                    LevelManager levelManager = GameObject.Find("LevelManager").GetComponent<LevelManager>();
                    levelManager.ShowUpgradeMenu();
                }

                collected = true;
                StartCoroutine(CollectAndDestroy());
            }
        }
    }

    IEnumerator CollectAndDestroy(){
        //hide mesh, wait .5 seccs, destroy
        GetComponent<MeshRenderer>().enabled = false;
        yield return new WaitForSeconds(0.5f);
        Destroy(gameObject);
    }

    // Update is called once per frame
    void Update()
    {
        PointTowardsPlayer();
        CheckCollect();
        Move();
        Accelerate();
        CheckPickupRange();
        rigidbodyVelocity = rb.velocity;
    }
}
