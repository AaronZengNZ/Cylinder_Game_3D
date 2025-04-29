using System.Numerics;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Vector2 = UnityEngine.Vector2;
using Vector3 = UnityEngine.Vector3;
using Quaternion = UnityEngine.Quaternion;

public class Player : MonoBehaviour
{
    [Header("References")]
    public Rigidbody rb;
    public Transform cursorLocation;
    public Transform finalCursorLocation;
    public Animator anim;
    public GameObject bulletPrefab;
    public UiTextManager uiTextManager;
    public StatsManager statsManager;
    public UpgradeManager upgradeManager;
    public TextMeshProUGUI velocityText;
    public PlayerHealth playerHealth;
    public GameEnd gameEnd;
    public GameObject deathParticles;
    [Header("Movement")]
    public float speed = 5.0f;
    public bool moving = false;
    public bool movingActive = false;
    public Vector3 mousePosInWorld;
    [Header("Cylinder")]
    public bool dead = false;
    public GameObject cylinder;
    public Vector3 cylinderRotationalOffset;
    public CapsuleCollider capsuleCollider;
    // other
    public float prevYrotation = 0f;
    private float accelerationValue = 0f;
    public float acceleration = 0.2f;
    public float velocity = 0f;
    public float maxVel = 1.5f;
    public float mass = 10f;
    public GameObject auras;

    float stasisWindup = 0.3f;
    bool hitWall = false;

    [Header("Special Upgrades & References")]
    public float pythagorasLimitationAngles = 0f;

    public GameObject projectile;
    public float goldenRatioVelocityCorrespondence = 0f;
    public float goldenRatioPower = 0f;
    public bool goldenRatioMaxed = false;
    private float goldenRatioProjectileCd = 0f;
    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody>();
        anim = GetComponent<Animator>();
        capsuleCollider = GetComponent<CapsuleCollider>();
        statsManager = GameObject.Find("StatsManager").GetComponent<StatsManager>();
        upgradeManager = GameObject.Find("UpgradeManager").GetComponent<UpgradeManager>();
        playerHealth = GetComponent<PlayerHealth>();
        gameEnd = GameObject.Find("GameEnd").GetComponent<GameEnd>();
    }

    void Update()
    {
        if (dead)
        {
            rb.velocity = Vector3.zero;
            return;
        }
        CheckForToggleMovement();
        GetVariables();
        GetUpgrades();
        Movement();
        Rotate();
        UpdateUI();
        SpecialUpgrades();
    }

    public void SpawnDeathParticles()
    {
        GameObject deathParticlesInstance = Instantiate(deathParticles, transform.position, Quaternion.identity);
    }

    public void GameOverScreen()
    {
        gameEnd.GameOver();
    }

    private void SpecialUpgrades()
    {
        goldenRatioVelocityCorrespondence = upgradeManager.goldenRatioVelocityCorrespondence;
        goldenRatioPower = upgradeManager.goldenRatioPower;
        goldenRatioMaxed = upgradeManager.goldenRatioMaxed;
        GoldenRatio();
    }

    private void GoldenRatio()
    {
        if (goldenRatioVelocityCorrespondence > 0f && velocity > 0f)
        {
            goldenRatioProjectileCd += Time.deltaTime;
            if (goldenRatioProjectileCd > (1 / goldenRatioVelocityCorrespondence / velocity))
            {
                float streams = 1f;
                if(goldenRatioMaxed)
                {
                    streams = 2f;
                }
                for (int i = 0; i < streams; i++)
                {
                    goldenRatioProjectileCd = 0f;
                    //make the projectiles spawn at the player and shoot in circles
                    GameObject projectileInstance = Instantiate(projectile, transform.position, Quaternion.identity);
                    BulletScript bulletScript = projectileInstance.GetComponent<BulletScript>();
                    projectileInstance.transform.rotation = Quaternion.Euler(0, (360 / streams) * i + Time.time * 90 * goldenRatioVelocityCorrespondence + 180, 0);
                    projectileInstance.GetComponent<Rigidbody>().velocity = rb.velocity + projectileInstance.transform.forward * (velocity + 5f) * 2f;
                    bulletScript.speed = velocity * 2f + 5f;
                    bulletScript.percentageDamage = goldenRatioPower;
                    bulletScript.damageElites = goldenRatioMaxed;
                    bulletScript.noControl = true;
                }
            }
        }
    }

    private void UpdateUI()
    {
        velocityText.text = $"v={velocity.ToString("F2")}u/s";
    }

    private void GetVariables()
    {
        //get mouse pos in world with ground layermask raycast
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;
        bool hitVoid = false;
        if (Physics.Raycast(ray, out hit, 1000, LayerMask.GetMask("Ground")))
        {
            mousePosInWorld = hit.point;
            if (hit.collider.tag == "Void")
            {
                hitVoid = true;
            }
        }
        cursorLocation.position = mousePosInWorld;
        hitWall = false;
        //case another ray from the player's position to the mouse position
        if (hitVoid)
        {
            Vector3 tempMousePosInWorld = mousePosInWorld;
            tempMousePosInWorld.y = transform.position.y;
            mousePosInWorld = tempMousePosInWorld;
            cursorLocation.position = mousePosInWorld;
            //cast a raycast from playerPosition to mousePosInWorld
            if (Physics.Raycast(transform.position, mousePosInWorld - transform.position, out hit, 1000, LayerMask.GetMask("Wall")))
            {
                mousePosInWorld = hit.point;
                if (GetVector2Distance(transform.position, mousePosInWorld) < 0.5f)
                {
                    hitWall = true;
                    return;
                }
            }
        }
        finalCursorLocation.position = mousePosInWorld;
    }

    private void GetUpgrades()
    {
        maxVel = statsManager.GetStatFloat("playerSpeed");
        pythagorasLimitationAngles = upgradeManager.pythagorasLimitationAngles;
    }

    void OnTriggerEnter(Collider other){
        if(other.tag == "Obstacle"){
            moving = false;
        }
    }

    private void Movement(){
        if(movingActive){
            if (moving)
            {
                if (GetVector2Distance(transform.position, mousePosInWorld) < 1f)
                {
                    stasisWindup -= Time.deltaTime;
                    if (hitWall)
                    {
                        stasisWindup = 0;
                    }
                }
                else
                {
                    stasisWindup = 0.3f;
                }
                if (stasisWindup <= 0)
                {
                    moving = false;
                    rb.velocity = Vector3.zero;
                    return;
                }

                Vector3 moveDirection = new Vector3(Mathf.Sin(prevYrotation * Mathf.Deg2Rad), 0, Mathf.Cos(prevYrotation * Mathf.Deg2Rad));
                rb.velocity = -moveDirection * speed * (accelerationValue);
                velocity = speed * (1 + (accelerationValue - 1) * 2f) / 2.4f;
            }
            else
            {
                rb.velocity = Vector3.zero;
                velocity = 0f;
            }
        }
        else{
            rb.velocity = Vector3.zero;
        }
    }

    public float GetCollisionDamage()
    {
        bool tangentMaxed = upgradeManager.tangentMaxed;
        float defence = playerHealth.defence;
        if (tangentMaxed && moving == false)
        {
            return 5f * defence;
        }
        else
        {
            return velocity * defence;
        }
    }

    private void Rotate()
    {
        //do not rotate if timescale is 0
        if (Time.timeScale <= 0.1f) { return; }
        if (movingActive == false) { accelerationValue = 1; }
        //point transform at lookpos
        transform.LookAt(new Vector3(mousePosInWorld.x, transform.position.y, mousePosInWorld.z));
        prevYrotation = transform.eulerAngles.y - 180;
        //float newYrotation = transform.eulerAngles.y;
        //prevYrotation = ChangeTowardsValue(prevYrotation, newYrotation, tempRotSpeed);

        if (pythagorasLimitationAngles > 0f && moving)
        {
            prevYrotation = Mathf.Round(prevYrotation / pythagorasLimitationAngles) * pythagorasLimitationAngles;
        }

        transform.eulerAngles = new Vector3(transform.eulerAngles.x, prevYrotation, transform.eulerAngles.z);

        //rotate the auras to make them not rotate with the player (make them face the same direction)
        auras.transform.eulerAngles = new Vector3(0, 0, 0);
    }

    private float CalculateAngle(Vector2 t1, Vector2 t2){
        float angle = Mathf.Atan2(t2.y - t1.y, t2.x - t1.x) * Mathf.Rad2Deg;
        if(angle < 0){
            angle += 360;
        }
        return angle;
    }

    private float ChangeTowardsValue(float value1, float value2, float changeSpeed){
        //lerp the values
        value1 = value2 - 180;
        return value1;
    }

    private void CheckForToggleMovement(){
        accelerationValue += acceleration * Time.deltaTime;
        if(accelerationValue > 1 * maxVel){
            accelerationValue = 1 * maxVel;
        }
        if(Input.GetMouseButtonDown(1)){
            moving = !moving;
            if(GetVector2Distance(transform.position, mousePosInWorld) < 0.5f){
                moving = false;
                return;
            }
        }
        anim.SetBool("Moving", moving);
    }

    public void SetActiveMovingTrue(){
        movingActive = true;
        capsuleCollider.direction = 0;
        capsuleCollider.center = new Vector3(0, -0.125f, 0);
    }

    public void SetActiveMovingFalse(){
        movingActive = false;
        capsuleCollider.direction = 1;
        capsuleCollider.center = new Vector3(0, 0, 0);
    }

    private float GetVector2Distance(Vector3 v1, Vector3 v2)
    {
        //get distance between x and z
        return Vector2.Distance(new Vector2(v1.x, v1.z), new Vector2(v2.x, v2.z));
    }
}
