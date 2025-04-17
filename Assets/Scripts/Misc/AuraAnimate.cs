using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AuraAnimate : MonoBehaviour
{
    public Transform targetTransform;
    public Vector3 rotateTransform;
    public float randomizeAmount = 0f;
    // Start is called before the first frame update
    void Start()
    {
        rotateTransform.x *= Random.Range(1f-randomizeAmount, 1f+randomizeAmount);
        rotateTransform.y *= Random.Range(1f-randomizeAmount, 1f+randomizeAmount);
        rotateTransform.z *= Random.Range(1f-randomizeAmount, 1f+randomizeAmount);
    }

    // Update is called once per frame
    void Update()
    {
        //rotate targettransform
        Vector3 tempRot = rotateTransform * Time.deltaTime;
        targetTransform.Rotate(tempRot);
    }
}
