using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;

public class CinemachineShake : MonoBehaviour
{
    public static CinemachineShake Instance { get; private set; }
    private float shakeTimer;
    private CinemachineVirtualCamera cinemachineVirtualCamera;

    private void Awake(){
        Instance = this;
        cinemachineVirtualCamera = GetComponent<CinemachineVirtualCamera>();
    }

    public void ShakeCamera(float intensity, float time){
        CinemachineBasicMultiChannelPerlin cinemachineBasicMultiChannelPerlin = 
            cinemachineVirtualCamera.GetCinemachineComponent<CinemachineBasicMultiChannelPerlin>();
        float currentAmplitude = cinemachineBasicMultiChannelPerlin.m_AmplitudeGain;
        if(intensity > currentAmplitude){
            cinemachineBasicMultiChannelPerlin.m_AmplitudeGain = intensity;
        }
        shakeTimer = time;
    }

    private void Update(){
        if(shakeTimer > 0){
            shakeTimer -= Time.deltaTime;
            if(shakeTimer <= 0){
                shakeTimer = 0;
                CinemachineBasicMultiChannelPerlin cinemachineBasicMultiChannelPerlin = 
                    cinemachineVirtualCamera.GetCinemachineComponent<CinemachineBasicMultiChannelPerlin>();

                cinemachineBasicMultiChannelPerlin.m_AmplitudeGain = 0;
            }
        }
    }
}
