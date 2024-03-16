using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    [Header("References")]
    public Rigidbody rb;
    public Transform cursorLocation;
    public Animator anim;
    public GameObject bulletPrefab;
    [Header("Gun")]
    public float firerate = 2f;
    public float bulletDamage = 10f;
    public float bulletSpeed = 10f;
    public bool canShoot = true;
    public string offsetType = "sin";
    public float gunScreenShake = 1f;
    public float gunScreenShakeTime = 0.05f;
    public float gunOffsetAngle = 0f;
    public float gunOffsetSpeed = 2f;
    public float gunOffsetQuantity = 10f;
    private float cooldown = 0f;
    public float pierce = 1f;
    [Header("Movement")]
    public float speed = 5.0f;
    public bool moving = false;
    public bool movingActive = false;
    public float rotationSpeed = 10f;
    public Vector3 mousePosInWorld;
    [Header("Cylinder")]
    public GameObject cylinder;
    public Vector3 cylinderRotationalOffset;
    // other
    private float prevYrotation = 0f;
    private float accelerationValue = 0f;
    
    // Start is called before the first frame update
    void Start()
    {
        
    }

    void Update(){
        CalculateOffsetAngle();
        CheckForToggleMovement(); 
        GetVariables();
        GunCalculations();
        Movement();
        Rotate();  
    }

    private void CalculateOffsetAngle(){
        if(offsetType == "sin"){
            gunOffsetAngle = Mathf.Sin(Time.time * gunOffsetSpeed) * gunOffsetQuantity;
        }
        else if(offsetType == "cos"){
            gunOffsetAngle = Mathf.Cos(Time.time * gunOffsetSpeed) * gunOffsetQuantity;
        }
        else if(offsetType == "natural"){
            gunOffsetAngle = 0f;
        }
        else{
            Debug.LogError("Invalid offset type");
        }
    }

    private void GunCalculations(){
        if(canShoot){
            if(Input.GetMouseButton(0) && cooldown <= 0f){
                Shoot();
            }
            else if(cooldown > 0f){
                cooldown -= Time.deltaTime;
            }
        }
    }

    private void Shoot(){
        if(movingActive == true){return;  }
        CinemachineShake.Instance.ShakeCamera(gunScreenShake, gunScreenShakeTime);
        GameObject bullet = Instantiate(bulletPrefab, transform.position, transform.rotation);
        BulletScript bulletScript = bullet.GetComponent<BulletScript>();
        bulletScript.yDirection = prevYrotation + gunOffsetAngle;
        bulletScript.damage = bulletDamage;
        bulletScript.speed = bulletSpeed;
        bulletScript.pierce = pierce;
        cooldown = (1f / firerate);
    }

    private void GetVariables(){
        //get mouse pos in world with ground layermask raycast
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;
        if(Physics.Raycast(ray, out hit, 1000, LayerMask.GetMask("Ground"))){
            mousePosInWorld = hit.point;
        }
        cursorLocation.position = mousePosInWorld;
    }

    private void Movement(){
        if(movingActive){
            if(moving){
                UnityEngine.Debug.Log(transform.forward);
                //instead of using transform.forward, use prevYrotation
                Vector3 moveDirection = new Vector3(Mathf.Sin(prevYrotation * Mathf.Deg2Rad), 0, Mathf.Cos(prevYrotation * Mathf.Deg2Rad));
                rb.velocity = -moveDirection * speed * (1 + accelerationValue*0.5f);
                
                //if close enough to mouse
                if(GetVector2Distance(transform.position, mousePosInWorld) < 0.5f){
                    moving = false;
                }
            }
            else{
                rb.velocity = Vector3.zero;
            }
        }
        else{
            rb.velocity = Vector3.zero;
        }
    }
    private void Rotate(){
        //rotate cylinders x rotation to look at mouse ONLY. keep all other rotations the same
        float tempRotSpeed = rotationSpeed;
        if(movingActive == false){accelerationValue = 0;}
        prevYrotation = transform.eulerAngles.y;
        //point transform at lookpos
        transform.LookAt(new Vector3(mousePosInWorld.x, transform.position.y, mousePosInWorld.z));
        float newYrotation = transform.eulerAngles.y;
        prevYrotation = ChangeTowardsValue(prevYrotation, newYrotation, tempRotSpeed);
        transform.eulerAngles = new Vector3(transform.eulerAngles.x, prevYrotation, transform.eulerAngles.z);
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
        value1 = Mathf.LerpAngle(value1, (value2 - 180), changeSpeed * Time.deltaTime);
        return value1;
    }

    private void CheckForToggleMovement(){
        accelerationValue += 0.2f * Time.deltaTime;
        if(accelerationValue > 1){
            accelerationValue = 1;
        }
        if(Input.GetKeyDown(KeyCode.Space) || Input.GetMouseButtonDown(1)){
            moving = !moving;
        }
        anim.SetBool("Moving", moving);
    }

    public void SetActiveMovingTrue(){
        movingActive = true;
    }

    public void SetActiveMovingFalse(){
        movingActive = false;
    }
    public void ToggleCanShoot(){
        canShoot = !canShoot;
    }

    private float GetVector2Distance(Vector3 v1, Vector3 v2)
    {
        //get distance between x and z
        return Vector2.Distance(new Vector2(v1.x, v1.z), new Vector2(v2.x, v2.z));
    }
}
