using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
public class UiTextManager : MonoBehaviour
{
    [Header("Variable References")]
    [Tooltip("this stands for time, 2nd stands for change in time since last fire")] public float[] t;
    [Tooltip("this stands for speed")] public float[] s;
    [Tooltip("this stands for power")] public float[] p;
    [Tooltip("this stands for mass")] public float[] m;
    [Tooltip("this stands for cooldown")] public float[] c;
    [Tooltip("this stands for level")] public float l;
    [Tooltip("this stands for upgrade points")] public float up;
    [Tooltip("this stands for defence")] public float d;
    [Tooltip("this stands for maxHealth")] public float mh;
    [Tooltip("this stands for currentHealth")] public float ch;
    [Header("Text References")]
    public TextMeshProUGUI gunText;
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        UpdateGunText();
    }

    public void GunUpdateVariableTool(float tempct, float temps, float tempp, float tempm, float tempc){
        t[1] = tempct;
        s[0] = temps;
        p[0] = tempp;
        m[0] = tempm;
        c[0] = tempc;
    }

    private void UpdateGunText(){
        string tempGunText = "";
        string timeSinceLastFire = t[1].ToString("F2");
        string cooldown = (1f / c[0]).ToString("F2");
        if((1f / c[0]) > 100f){
            cooldown = "∞";
        }
        float bulletSpeed = s[0];
        float bulletDamage = p[0];
        float bulletMass = m[0];
        tempGunText += "Case[∆t("+timeSinceLastFire+")>"+cooldown+"]{Projectile(s="+bulletSpeed+", p="+bulletDamage+", m="+bulletMass+")}";
        gunText.text = tempGunText;
    }
}
