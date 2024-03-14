using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CubeComponent : MonoBehaviour
{
    public Rigidbody rb;
    public float speed = 10f;
    public EnemyCreator enemyCreator;
    public Transform target;
    public float damage = 10f;
    public float maxHp = 10f;
    float hp = 10f;
    public bool slotFilled = false;
    public Transform player;
    public float randomOffset = 0.5f;
    private Vector3 randomChaseOffset;
    public Material passiveMaterial;
    public Material offensiveMaterial;
    public MeshRenderer meshRenderer;
    void Start()
    {
        meshRenderer = GetComponent<MeshRenderer>();
        hp = maxHp;
        speed *= Random.Range(0.7f, 1.3f);
        slotFilled = false;
        rb = GetComponent<Rigidbody>();
        player = GameObject.Find("Player").transform;
        NextRandomOffset();
        FindClosestEnemyCreator();
    }

    private void NextRandomOffset(){
        randomChaseOffset = new Vector3(Random.Range(-randomOffset, randomOffset) + player.position.x, 0.65f, Random.Range(-randomOffset, randomOffset) + player.position.z);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "PlayerBullet" && target == null)
        {
            BulletScript bulletScript = other.gameObject.GetComponent<BulletScript>();
            TakeDamage(bulletScript.damage);
            bulletScript.HitEnemy();
        }
    }

    private void TakeDamage(float damage)
    {
        if(target != null){return;}
        hp -= damage;
        if (hp <= 0)
        {
            Die();
        }
    }

    private void Die(){
        Destroy(gameObject);
    }

    private void FindClosestEnemyCreator()
    {
        //loop through all enemy creators and make an array of the closest to the furthest. try to getSlot(), if fails go to another enemycreator
        EnemyCreator[] enemyCreators = FindObjectsOfType<EnemyCreator>();
        float[] distances = new float[enemyCreators.Length];
        for (int i = 0; i < enemyCreators.Length; i++)
        {
            distances[i] = Vector3.Distance(transform.position, enemyCreators[i].transform.position);
        }
        for (int i = 0; i < enemyCreators.Length; i++)
        {
            float smallest = Mathf.Infinity;
            int smallestIndex = 0;
            for (int j = 0; j < distances.Length; j++)
            {
                if (distances[j] < smallest)
                {
                    smallest = distances[j];
                    smallestIndex = j;
                }
            }
            Transform slot = enemyCreators[smallestIndex].getSlot(gameObject);
            if (slot != null)
            {
                enemyCreator = enemyCreators[smallestIndex];
                target = slot;
                return;
            }
            else
            {
                distances[smallestIndex] = Mathf.Infinity;
            }
        }
    }

    // Update is called once per frame
    void Update()
    {
        Movement();
        if(target == null){
            meshRenderer.material = offensiveMaterial;
        }
        else{
            meshRenderer.material = passiveMaterial;
        }
    }

    private void Movement()
    {
        if (target == null)
        {
            FindClosestEnemyCreator();
        }
        if (target == null)
        {
            //move towards random chase offset
            Vector3 direction = randomChaseOffset - transform.position;
            rb.velocity = direction.normalized * speed;
            if (GetVector2Distance(transform.position, randomChaseOffset) <= 0.05f)
            {
                NextRandomOffset();
            }
            return;
        }
        else
        {
            Vector3 direction = target.position - transform.position;
            rb.velocity = direction.normalized * speed;
            //if close enough, call slotFilled() and go to target position, set rb velocity to 0
            if (Vector3.Distance(transform.position, target.position) <= 0.15f)
            {
                if (slotFilled == false)
                {
                    enemyCreator.slotFilled();
                    slotFilled = true;
                }
                transform.position = new Vector3(target.position.x, transform.position.y, target.position.z);
                rb.velocity = Vector3.zero;
            }
        }
    }

    private float GetVector2Distance(Vector3 v1, Vector3 v2)
    {
        //get distance between x and z
        return Vector2.Distance(new Vector2(v1.x, v1.z), new Vector2(v2.x, v2.z));
    }
}
