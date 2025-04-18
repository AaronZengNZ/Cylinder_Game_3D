using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyCreator : MonoBehaviour
{
    public Transform[] instantiationLocations;
    public GameObject enemyToInstantiate;
    public GameObject[] childObjects;
    public float slotsTakenUp = 0f;
    public float slotsToInstantiate = 12f;
    public float slotsFilled = 0f;
    private bool instantiating = false;
    public CubeSpawner cubeSpawner;

    public float enemiesToInstantiate = 0f;
    float enemiesInstantiated = 0f;

    void Start()
    {
        cubeSpawner = GameObject.Find("CubeSpawner").GetComponent<CubeSpawner>();
        StartCoroutine(BeginAnim());
    }

    IEnumerator BeginAnim()
    {
        instantiating = true;
        //over 1 second, go from -1 to 0
        float time = 0;
        Vector3 startPos = new Vector3(transform.position.x, transform.position.y - 1, transform.position.z);
        Vector3 endPos = transform.position;
        while (time < 1)
        {
            time += Time.deltaTime;
            transform.position = Vector3.Lerp(startPos, endPos, time);
            yield return null;
        }
        transform.position = endPos;
        instantiating = false;
    }

    public Transform getSlot(GameObject newObject)
    {
        if (slotsTakenUp >= slotsToInstantiate || instantiating)
        {
            return null;
        }
        bool foundSlot = false;
        //first check if there are any empty slots. if there are, set the slot to newObject
        for (int i = 0; i < childObjects.Length; i++)
        {
            if (childObjects[i] == null)
            {
                childObjects[i] = newObject;
                foundSlot = true;
            }
        }
        if (!foundSlot)
        {
            GameObject[] tempChildObjects = new GameObject[childObjects.Length + 1];
            for (int i = 0; i < childObjects.Length; i++)
            {
                tempChildObjects[i] = childObjects[i];
            }
            tempChildObjects[tempChildObjects.Length - 1] = newObject;
            childObjects = tempChildObjects;
        }

        Transform slot = instantiationLocations[Mathf.FloorToInt(slotsTakenUp)];
        slotsTakenUp++;
        return slot;
    }
    public void slotFilled(){
        slotsFilled++;
    }

    void Update(){
        if(slotsFilled >= slotsToInstantiate){        
            InstantiateEnemy();
        }
    }

    void OnTriggerEnter(Collider other){
        if(other.gameObject.tag == "PlayerBullet"){
            other.gameObject.GetComponent<BulletScript>().HitEnemy();
        }
    }

    private void InstantiateEnemy(){
        if (slotsFilled >= slotsToInstantiate && instantiating == false)
        {
            instantiating = true;
            //destroy all childobjects and instantiate enemy
            for (int i = 0; i < childObjects.Length; i++)
            {
                Destroy(childObjects[i]);
                childObjects[i] = null;
            }
            GameObject newEnemy = Instantiate(enemyToInstantiate, transform.position, transform.rotation);
            newEnemy.GetComponent<Enemy>().hitpointsPerBlock *= cubeSpawner.cubeHpMulti;
            newEnemy.GetComponent<Enemy>().xpDrop *= cubeSpawner.xpMultiplier;
            newEnemy.GetComponent<Enemy>().particlesDropped += Mathf.Pow(cubeSpawner.difficulty, 1.5f);
            newEnemy.GetComponent<Enemy>().aliveMaterial = cubeSpawner.GetMaterialForDifficulty();
            newEnemy.GetComponent<Enemy>().attackDamage *= cubeSpawner.damageMultiplier;

            enemiesInstantiated++;
            if (enemiesInstantiated < enemiesToInstantiate || enemiesToInstantiate == 0)
            {
                StartCoroutine(UnInstantiate());
            }
            else
            {
                //play disappear animation and destroy
                StartCoroutine(DoneAndDestroy());
            }
        }
    }

    IEnumerator DoneAndDestroy()
    {
        yield return new WaitForSeconds(1.5f);
        float time = 0;
        Vector3 startPos = transform.position;
        Vector3 endPos = new Vector3(transform.position.x, transform.position.y - 1, transform.position.z);
        while (time < 1)
        {
            time += Time.deltaTime;
            transform.position = Vector3.Lerp(startPos, endPos, time);
            yield return null;
        }
        Destroy(gameObject);
    }

    IEnumerator UnInstantiate(){
        yield return new WaitForSeconds(1.5f);
        slotsTakenUp = 0f;
        slotsFilled = 0f;
        instantiating = false;
    }

    public void GameobjectDestroyed(GameObject destroyedObject){
        for(int i = 0; i < childObjects.Length; i++){
            if(childObjects[i] == destroyedObject){
                childObjects[i] = null;
                slotsFilled--;
                if(slotsTakenUp > slotsFilled){
                    slotsTakenUp--;
                }
            }
        }
    }
}
