using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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
    [Header("Movement")]
    public float speed = 5.0f;
    public bool moving = false;
    public bool movingActive = false;
    public Vector3 mousePosInWorld;
    [Header("Cylinder")]
    public GameObject cylinder;
    public Vector3 cylinderRotationalOffset;
    public CapsuleCollider capsuleCollider;
    // other
    public float prevYrotation = 0f;
    private float accelerationValue = 0f;
    public float acceleration = 0.2f;
    public float velocity = 0f;
    public float maxVelMulti = 1f;
    public float mass = 10f;
    public GameObject auras;

    float stasisWindup = 0.3f;
    bool hitWall = false;

    public float pythagorasLimitationAngles = 0f;

    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody>();
        anim = GetComponent<Animator>();
        capsuleCollider = GetComponent<CapsuleCollider>();
        statsManager = GameObject.Find("StatsManager").GetComponent<StatsManager>();
        upgradeManager = GameObject.Find("UpgradeManager").GetComponent<UpgradeManager>();
    }

    void Update(){
        CheckForToggleMovement(); 
        GetVariables();
        GetUpgrades();
        Movement();
        Rotate();  
    }
    private void GetVariables(){
        //get mouse pos in world with ground layermask raycast
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;
        bool hitVoid = false;
        if(Physics.Raycast(ray, out hit, 1000, LayerMask.GetMask("Ground"))){
            mousePosInWorld = hit.point;
            if(hit.collider.tag == "Void"){
                hitVoid = true;
            }
        }
        cursorLocation.position = mousePosInWorld;
        hitWall = false;
        //case another ray from the player's position to the mouse position
        if(hitVoid){
            Vector3 tempMousePosInWorld = mousePosInWorld;
            tempMousePosInWorld.y = transform.position.y;
            mousePosInWorld = tempMousePosInWorld;
            cursorLocation.position = mousePosInWorld;
            //cast a raycast from playerPosition to mousePosInWorld
            if(Physics.Raycast(transform.position, mousePosInWorld - transform.position, out hit, 1000, LayerMask.GetMask("Wall"))){
                mousePosInWorld = hit.point;
                if(GetVector2Distance(transform.position, mousePosInWorld) < 0.5f){
                    hitWall = true;
                    return;
                }
            }
        }
        finalCursorLocation.position = mousePosInWorld;
    }

    private void GetUpgrades()
    {
        maxVelMulti = statsManager.GetStatFloat("playerSpeed");
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
                rb.velocity = -moveDirection * speed * (1 + accelerationValue * 0.5f);
                velocity = speed * (1 + accelerationValue) / 2.4f;
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
    private void Rotate(){
        //do not rotate if timescale is 0
        if(Time.timeScale <= 0.1f){return;}
        if(movingActive == false){accelerationValue = 0;}
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
        if(accelerationValue > 1 * maxVelMulti){
            accelerationValue = 1 * maxVelMulti;
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
