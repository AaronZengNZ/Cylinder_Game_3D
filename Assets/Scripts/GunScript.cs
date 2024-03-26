using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GunScript : MonoBehaviour
{
    [Header("References")]
    public StatsManager statsManager;
    public Player player;
    public GameObject bulletPrefab;
    public UiTextManager uiTextManager;
    [Header("Private Stats")]
    public float gunScreenShake = 1f;
    public float gunScreenShakeTime = 0.05f;
    private float firerate = 2f;
    private float bulletDamage = 10f;
    private float bulletSpeed = 10f;
    private bool canShoot = true;
    private string offsetType = "sin";
    private float gunOffsetAngle = 0f;
    private float gunOffsetSpeed = 2f;
    private float gunOffsetQuantity = 10f;
    private float bulletMass = 1f;
    private float cooldown = 0f;
    private float pierce = 1f;
    private float timeSinceLastFire = 0f;
    private bool movingActive = false;
    private float prevYrotation = 0f;
    // Start is called before the first frame update
    void Start()
    {
        uiTextManager = GameObject.Find("UITextManager").GetComponent<UiTextManager>();
    }

    // Update is called once per frame
    void Update()
    {
        GetVariables();
        CalculateOffsetAngle();
        GunCalculations();
        UpdateUITextManagerVariables();
    }

    private void UpdateUITextManagerVariables(){
        uiTextManager.GunUpdateVariableTool(timeSinceLastFire, bulletSpeed, bulletDamage, bulletMass, firerate);
    }

    private void GetVariables(){
        movingActive = player.movingActive;
        prevYrotation = player.prevYrotation;
        firerate = statsManager.firerate;
        bulletDamage = statsManager.damage;
        bulletSpeed = statsManager.bulletSpeed;
        offsetType = statsManager.GunOffsetType;
        gunOffsetSpeed = statsManager.gunOffsetSpeed;
        gunOffsetQuantity = statsManager.gunOffsetQuantity;
        bulletMass = statsManager.bulletMass;
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
            timeSinceLastFire += Time.deltaTime;
            if(Input.GetMouseButton(0) && cooldown <= 0f){
                Shoot();
            }
            else if(cooldown > 0f){
                cooldown -= Time.deltaTime;
            }
        }
    }

    private void Shoot(){
        if(movingActive == true){return;}
        timeSinceLastFire = 0f;
        CinemachineShake.Instance.ShakeCamera(gunScreenShake, gunScreenShakeTime);
        GameObject bullet = Instantiate(bulletPrefab, transform.position, transform.rotation);
        BulletScript bulletScript = bullet.GetComponent<BulletScript>();
        bulletScript.yDirection = prevYrotation + gunOffsetAngle;
        bulletScript.damage = bulletDamage;
        bulletScript.speed = bulletSpeed;
        bulletScript.pierce = pierce;
        cooldown += (1f / firerate);
    }
}
