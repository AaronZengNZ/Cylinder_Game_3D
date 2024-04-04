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
    [Header("Movement")]
    public float speed = 5.0f;
    public bool moving = false;
    public bool movingActive = false;
    public Vector3 mousePosInWorld;
    [Header("Cylinder")]
    public GameObject cylinder;
    public Vector3 cylinderRotationalOffset;
    // other
    public float prevYrotation = 0f;
    private float accelerationValue = 0f;
    
    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody>();
        anim = GetComponent<Animator>();
        
    }

    void Update(){
        CheckForToggleMovement(); 
        GetVariables();
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
        //case another ray from the player's position to the mouse position
        if(hitVoid){
            Vector3 tempMousePosInWorld = mousePosInWorld;
            tempMousePosInWorld.y = transform.position.y;
            mousePosInWorld = tempMousePosInWorld;
            cursorLocation.position = mousePosInWorld;
            //cast a raycast from playerPosition to mousePosInWorld
            if(Physics.Raycast(transform.position, mousePosInWorld - transform.position, out hit, 1000, LayerMask.GetMask("Wall"))){
                mousePosInWorld = hit.point;
            }
        }
        finalCursorLocation.position = mousePosInWorld;
    }

    void OnTriggerEnter(Collider other){
        if(other.tag == "Obstacle"){
            moving = false;
        }
    }

    private void Movement(){
        if(movingActive){
            if(moving){
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
        if(movingActive == false){accelerationValue = 0;}
        //point transform at lookpos
        transform.LookAt(new Vector3(mousePosInWorld.x, transform.position.y, mousePosInWorld.z));
        prevYrotation = transform.eulerAngles.y - 180;
        //float newYrotation = transform.eulerAngles.y;
        //prevYrotation = ChangeTowardsValue(prevYrotation, newYrotation, tempRotSpeed);
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
        value1 = value2 - 180;
        return value1;
    }

    private void CheckForToggleMovement(){
        accelerationValue += 0.2f * Time.deltaTime;
        if(accelerationValue > 1){
            accelerationValue = 1;
        }
        if(Input.GetKeyDown(KeyCode.Space) || Input.GetMouseButtonDown(1)){
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
    }

    public void SetActiveMovingFalse(){
        movingActive = false;
    }

    private float GetVector2Distance(Vector3 v1, Vector3 v2)
    {
        //get distance between x and z
        return Vector2.Distance(new Vector2(v1.x, v1.z), new Vector2(v2.x, v2.z));
    }
}
