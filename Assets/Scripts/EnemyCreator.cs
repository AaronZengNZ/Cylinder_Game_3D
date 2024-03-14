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
    public Transform getSlot(GameObject newObject){
        if(slotsTakenUp >= slotsToInstantiate || instantiating){
            return null;
        }
        bool foundSlot = false;
        //first check if there are any empty slots. if there are, set the slot to newObject
        for(int i = 0; i < childObjects.Length; i++){
            if(childObjects[i] == null){
                childObjects[i] = newObject;
                foundSlot = true;
            }
        }
        if(!foundSlot){
            GameObject[] tempChildObjects = new GameObject[childObjects.Length+1];
            for(int i = 0; i < childObjects.Length; i++){
                tempChildObjects[i] = childObjects[i];
            }
            tempChildObjects[tempChildObjects.Length-1] = newObject;
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
        InstantiateEnemy();
    }

    void OnTriggerEnter(Collider other){
        if(other.gameObject.tag == "PlayerBullet"){
            other.gameObject.GetComponent<BulletScript>().HitEnemy();
        }
    }

    private void InstantiateEnemy(){
        if(slotsFilled >= slotsToInstantiate && instantiating == false){
            instantiating = true;
            //destroy all childobjects and instantiate enemy
            for(int i = 0; i < childObjects.Length; i++){
                Destroy(childObjects[i]);
                childObjects[i] = null;
            }
            slotsTakenUp = 0f;
            slotsFilled = 0f;
            StartCoroutine(UnInstantiate());
            GameObject newEnemy = Instantiate(enemyToInstantiate, transform.position, transform.rotation);
        }
    }

    IEnumerator UnInstantiate(){
        yield return new WaitForSeconds(1.5f);
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
