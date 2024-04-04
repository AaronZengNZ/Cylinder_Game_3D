using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StatsManager : MonoBehaviour
{
    [Header("Gun")]
    public float firerate = 2f;
    public float firerateBase = 2f;
    public float damage = 5f;
    public float damageBase = 5f;
    public float pierce = 10f;
    public float pierceBase = 10f;
    public float bulletSpeed = 10f;
    public float bulletSpeedBase = 10f;
    public float bulletMass = 1f;
    public float bulletMassBase = 1f;
    public string GunOffsetType = "sin";
    public float gunOffsetSpeed = 2f;
    public float gunOffsetQuantity = 10f;
    public float gunOffsetQuantityBase = 2f;
    [Header("Player")]
    public float speed = 5.0f;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    public void UpgradeManagerStatTool(float firerateAdditiveChanges, float firerateMultiplicativeChanges,
    float damageAdditiveChanges, float damageMultiplicativeChanges, 
    float pierceAdditiveChanges, float pierceMultiplicativeChanges,
    float bulletSpeedAdditiveChanges, float bulletSpeedMultiplicativeChanges,
    float bulletMassAdditiveChanges, float bulletMassMultiplicativeChanges,
    float gunOffsetSpeedChanges, float gunOffsetQuantityChanges){
        firerate = (firerateBase + firerateAdditiveChanges) * firerateMultiplicativeChanges;
        damage = (damageBase + damageAdditiveChanges) * damageMultiplicativeChanges;
        pierce = (pierceBase + pierceAdditiveChanges) * pierceMultiplicativeChanges;
        bulletSpeed = (bulletSpeedBase + bulletSpeedAdditiveChanges) * bulletSpeedMultiplicativeChanges;
        bulletMass = (bulletMassBase + bulletMassAdditiveChanges) * bulletMassMultiplicativeChanges;
        gunOffsetQuantity = gunOffsetQuantityBase = gunOffsetQuantityChanges;
    }
    // Update is called once per frame
    void Update()
    {
        
    }
}
