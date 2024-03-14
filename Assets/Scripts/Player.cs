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
    private float cooldown = 0f;
    [Header("Movement")]
    public float speed = 5.0f;
    public bool moving = false;
    public bool movingActive = false;
    public float rotationSpeed = 10f;
    public float mousePosInWorldAngle;
    [Header("Cylinder")]
    public GameObject cylinder;
    public Vector3 cylinderRotationalOffset;
    // other
    private float prevYrotation = 0f;
    private float accuracyImprover = 0f;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        CheckForToggleMovement(); 
        GetVariables();
        Movement();
        Rotate();  
        GunCalculations();
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
        GameObject bullet = Instantiate(bulletPrefab, transform.position, transform.rotation);
        BulletScript bulletScript = bullet.GetComponent<BulletScript>();
        bulletScript.yDirection = prevYrotation;
        bulletScript.damage = bulletDamage;
        bulletScript.speed = bulletSpeed;
        cooldown = (1f / firerate);
        if(movingActive == true){
            bulletScript.yDirection = Mathf.Sin(Time.time * 5f) * 360f;
            cooldown = (2f / firerate);
        }
    }

    private void GetVariables(){
        //find angle from center of screen to mouse on screen
        Vector2 mousePos = Input.mousePosition;
        Vector2 screenCenter = new Vector2(Screen.width / 2, Screen.height / 2);
        //calculate compass angle from center of screen to mouse on screen
        mousePosInWorldAngle = 360 - CalculateAngle(screenCenter, mousePos) - 45f;
    }

    private void Movement(){
        if(movingActive){
            if(moving){
                rb.velocity = transform.forward * speed * (1 + accuracyImprover * 0.5f);
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
        if(movingActive == false){accuracyImprover = 0;}
        prevYrotation = transform.eulerAngles.y;
        float newYrotation = mousePosInWorldAngle;
        prevYrotation = ChangeTowardsValue(prevYrotation, newYrotation, rotationSpeed);
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
        string direction = "right";
        //accuracy improver
        accuracyImprover += 0.2f * Time.deltaTime;
        if(accuracyImprover > 1f){
            accuracyImprover = 1f;
        }
        float difference = Mathf.Abs(value1 - value2 - 180);
        if(difference > 180){
            difference -= 360;
            difference = Mathf.Abs(difference);
        }
        UnityEngine.Debug.Log(difference);
        if(movingActive == false || difference <= Time.deltaTime * changeSpeed * (1+accuracyImprover)){
            value1 = value2;
        }
        else if(value1 < value2){
            value1 += 360;
            if(value1 - value2 < 180){
                value1 += changeSpeed * Time.deltaTime - 360;
            }
            else{
                value1 -= changeSpeed * Time.deltaTime - 360;
            }
        }
        else if(value2 < value1){
            value2 += 360;
            if(value2 - value1 < 180){
                value1 -= changeSpeed * Time.deltaTime;
            }
            else{
                value1 += changeSpeed * Time.deltaTime;
            }
        }
        return value1;
    }

    private void CheckForToggleMovement(){
        //space or right click
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
}
