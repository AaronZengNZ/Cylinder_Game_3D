using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class GunScript : MonoBehaviour
{
    [Header("References")]
    public StatsManager statsManager;
    public Player player;
    public GameObject bulletPrefab;
    public UiTextManager uiTextManager;
    public UpgradeManager upgradeManager;
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
    private float pierce = 1f;
    private float timeSinceLastFire = 0f;
    private bool movingActive = false;
    private float prevYrotation = 0f;
    // Start is called before the first frame update
    void Start()
    {
        uiTextManager = GameObject.Find("UITextManager").GetComponent<UiTextManager>();
        upgradeManager = GameObject.Find("UpgradeManager").GetComponent<UpgradeManager>();
        player = GameObject.Find("Player").GetComponent<Player>();
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
        firerate = statsManager.GetStatFloat("firerate");
        bulletDamage = statsManager.GetStatFloat("damage");
        bulletSpeed = statsManager.GetStatFloat("bulletSpeed");
        pierce = statsManager.GetStatFloat("pierce");
        offsetType = statsManager.GetStatString("gunOffsetType");
        gunOffsetSpeed = statsManager.GetStatFloat("gunOffsetSpeed");
        gunOffsetQuantity = statsManager.GetStatFloat("gunOffsetQuantity");
        bulletMass = statsManager.GetStatFloat("bulletMass");
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
            if (Input.GetMouseButton(0) && timeSinceLastFire >= 1f / firerate)
            {
                Shoot();
                upgradeManager.FireCalculations();
            }
            else if(timeSinceLastFire >= (1f / firerate + Time.deltaTime)){
                upgradeManager.NotFiringCalculations();
            }
        }
    }

    private void Shoot(){
        if(movingActive == true){return;}
        timeSinceLastFire = 1f / firerate + Time.deltaTime;
        float tempDamageMulti = 1f;
        if (timeSinceLastFire / (1f / firerate) > 5f)
        {
            tempDamageMulti = timeSinceLastFire / (5f / firerate);
            timeSinceLastFire = 5f / firerate;
        }
        while (timeSinceLastFire >= 1f / firerate)
        {
            timeSinceLastFire -= 1f / firerate;
            //CinemachineShake.Instance.ShakeCamera(gunScreenShake, gunScreenShakeTime);
            GameObject bullet = Instantiate(bulletPrefab, transform.position, transform.rotation);
            BulletScript bulletScript = bullet.GetComponent<BulletScript>();
            float randomOffset = Random.Range(-(Mathf.Pow(firerate, 0.6f) / 5f), (Mathf.Pow(firerate, 0.6f) / 5f));

            bulletScript.yDirection = prevYrotation + gunOffsetAngle + randomOffset;
            bulletScript.damage = bulletDamage * tempDamageMulti;
            bulletScript.speed = bulletSpeed;
            bulletScript.pierce = pierce;
        }
    }
}
