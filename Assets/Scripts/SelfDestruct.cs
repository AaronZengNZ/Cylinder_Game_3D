using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SelfDestruct : MonoBehaviour
{
    public float selfDestructTime = 5f;
    public float destructShakeIntensity = 5f;
    public float destructShakeTime = 0.2f;
    void Start()
    {
        CinemachineShake.Instance.ShakeCamera(destructShakeIntensity, destructShakeTime);
        StartCoroutine(SelfDestructTimer());
    }

    IEnumerator SelfDestructTimer(){
        yield return new WaitForSeconds(selfDestructTime);
        Destroy(gameObject);
    }
}
