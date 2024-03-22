using System.Numerics;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Vector3 = UnityEngine.Vector3;
using Vector2 = UnityEngine.Vector2;

public class CubeComponentManager : MonoBehaviour
{
    [Header("References")]
    public GameObject[] cubeComponents;
    public Transform player;
    public Material passiveMaterial;
    public Material offensiveMaterial;
    private Rigidbody[] rbs = new Rigidbody[10000];
    private EnemyCreator[] enemyCreators = new EnemyCreator[10000];
    private Transform[] targets = new Transform[10000];
    private MeshRenderer[] meshRenderers = new MeshRenderer[10000];
    private Vector3[] randomChaseOffsets = new Vector3[10000];
    [Header("Stats")]
    public float maxDistanceToSee = 50f;
    public float baseSpeed = 10f;
    private float[] speeds = new float[10000];
    public float damage = 10f;
    private bool[] slotFilleds = new bool[10000];
    public float randomOffset = 0.5f;
    public float maxObjectsUpdatedPerSecond = 1000;
    public float objectsUpdated = 0;
    public float chaseDeactivationDistance = 0.5f;
    float cubeComponentsThatExist;
    void Start(){
        FindCubes();
        SetStartingVariablesOfComponents();
    }

    void Update(){
        FindCubes();
        float tempObjectsUpdated = objectsUpdated;
        float tempAmountOfObjectsToUpdate = maxObjectsUpdatedPerSecond * Time.deltaTime;
        cubeComponentsThatExist = 0f;
        for(int i = 0; i < cubeComponents.Length; i++){
            if(cubeComponents[i] != null){
                cubeComponentsThatExist = i;
            }
        }
        float timeToUpdateSingleComponent = cubeComponentsThatExist / maxObjectsUpdatedPerSecond;
        chaseDeactivationDistance = timeToUpdateSingleComponent * 5f;
        for(int i = 0; i < cubeComponentsThatExist; i++){
            if(i >= objectsUpdated && i < objectsUpdated + tempAmountOfObjectsToUpdate){
                if(cubeComponents[i] != null){
                    RunSimulatedUpdateOfIndex(i + 1);
                }
                tempObjectsUpdated++;
            }
        }
        objectsUpdated = tempObjectsUpdated;
        if(objectsUpdated >= cubeComponentsThatExist){
            objectsUpdated = 0;
        }
    }

    private void RunSimulatedUpdateOfIndex(float index){
        int realIndex = (int)index - 1;
        CalculateAndRunMovementOfComponent(realIndex);
        SetActivationOfMeshRendererBasedOnVisible(realIndex);
        SetVariablesOfComponentScript(realIndex);
        SetMaterialOfMeshRendererBasedOnTarget(realIndex);
    }

    private void SetVariablesOfComponentScript(int index){
        CubeComponent tempComponentScript = cubeComponents[index].GetComponent<CubeComponent>();
        tempComponentScript.cubeComponentManager = this;
        if(targets[index] == null){
            tempComponentScript.hasTarget = false;
        }
        else{
            tempComponentScript.hasTarget = true;
        }
    }

    private void CalculateAndRunMovementOfComponent(int index){
        if(targets[index] == null){
            FindTargetForComponent(index);
        }
        if(targets[index] == null){
            ChasePlayerInsteadFallback(index);
        }
        else{
            ChaseEnemySuccess(index);
        }
    }

    private void ChaseEnemySuccess(int index){
        Vector3 direction = targets[index].position - cubeComponents[index].transform.position;
        rbs[index].velocity = direction.normalized * speeds[index];
        if(ReturnDistanceBetweenVector2Comparison(cubeComponents[index].transform.position, targets[index].position) < chaseDeactivationDistance){
            if(slotFilleds[index] == false){
                enemyCreators[index].slotFilled();
                slotFilleds[index] = true;
            }
            cubeComponents[index].transform.position = 
            new Vector3(targets[index].position.x, cubeComponents[index].transform.position.y, targets[index].position.z);
            rbs[index].velocity = Vector3.zero;
        }
    }

    private void ChasePlayerInsteadFallback(int index){
        Vector3 direction = randomChaseOffsets[index] - cubeComponents[index].transform.position;
        rbs[index].velocity = direction.normalized * speeds[index];
        if(ReturnDistanceBetweenVector2Comparison(cubeComponents[index].transform.position, randomChaseOffsets[index]) < chaseDeactivationDistance){
            FindChaseOffsetForComponent(index);
        }
        FindEnemyCreatorForComponent(index);
        FindTargetForComponent(index);
    }

    private void SetMaterialOfMeshRendererBasedOnTarget(int index){
        if(targets[index] == null){
            meshRenderers[index].material = offensiveMaterial;
        }
        else{
            meshRenderers[index].material = passiveMaterial;
        }
    }
    
    private void SetActivationOfMeshRendererBasedOnVisible(int index){
        if(Mathf.Abs(cubeComponents[index].transform.position.x) > maxDistanceToSee || Mathf.Abs(cubeComponents[index].transform.position.z) > maxDistanceToSee){
            meshRenderers[index].enabled = false;
        }
        else{
            meshRenderers[index].enabled = true;
        }
    }    
    private void FindCubes(){
        GameObject[] newCubeComponents = GameObject.FindGameObjectsWithTag("CubeComponent");
        GameObject[] oldCubeComponents = cubeComponents;
        if(newCubeComponents.Length > oldCubeComponents.Length){
            cubeComponents = new GameObject[newCubeComponents.Length];
        }
        else{
            cubeComponents = new GameObject[oldCubeComponents.Length];
        
        }
        //add in the old cube components
        for(int i = 0; i < oldCubeComponents.Length; i++){
            cubeComponents[i] = oldCubeComponents[i];
        }
        float newCubeComponentsLooped = 0;
        //loop through the arrays and add in the new cube components
        for(int i = 0; i < newCubeComponents.Length; i++){
            CubeComponent tempNewCubeComponent = newCubeComponents[i].GetComponent<CubeComponent>();
            if(tempNewCubeComponent.index != 0){
                cubeComponents[(int)tempNewCubeComponent.index - 1] = newCubeComponents[i];
            }
            else{
                tempNewCubeComponent.index = SetComponentToFirstAvailableSlotAndReturnIndex(newCubeComponents[i]);
                SetVariablesOfComponent(tempNewCubeComponent.index);
            }
        }
    }

    private float SetComponentToFirstAvailableSlotAndReturnIndex(GameObject tempComponent){
        for(int i = 0; i < cubeComponents.Length; i++){
            if(cubeComponents[i] == null){
                cubeComponents[i] = tempComponent;
                return i + 1;
            }
        }
        return 0;
    }

    public void ComponentDestroyed(float index){
        cubeComponents[(int)index - 1] = null;
    }

    private void SetStartingVariablesOfComponents(){
        for(int i = 0; i < cubeComponents.Length; i++){
            if(cubeComponents[i] != null){
                SetVariablesOfComponent(cubeComponents[i].GetComponent<CubeComponent>().index);
            }
        }
    }

    private void SetVariablesOfComponent(float index){
        int realIndex = (int)index - 1;
        FindRigidbodyForComponent(realIndex);
        FindEnemyCreatorForComponent(realIndex);
        FindTargetForComponent(realIndex);
        FindMeshRendererForComponent(realIndex);
        FindChaseOffsetForComponent(realIndex);
        SetDefaultVariablesForComponent(realIndex);
        SetRandomSpeedForComponent(realIndex);
    }

    private void SetDefaultVariablesForComponent(int index){
        speeds[index] = baseSpeed;
        slotFilleds[index] = false;
    }

    private void SetRandomSpeedForComponent(int index){
        speeds[index] = baseSpeed * Random.Range(0.7f, 1.3f);
    }

    private void FindChaseOffsetForComponent(int index){
        randomChaseOffsets[index] = new Vector3(Random.Range(-randomOffset, randomOffset) + player.position.x
            , 0.65f, Random.Range(-randomOffset, randomOffset) + player.position.z);
    }

    private void FindMeshRendererForComponent(int index){
        MeshRenderer meshRenderer = cubeComponents[index].GetComponent<MeshRenderer>();
        if(meshRenderer != null){
            meshRenderers[index] = meshRenderer;
        }
        else{
            meshRenderers[index] = null;
        }
    }

    private void FindTargetForComponent(int index){
        Transform slot;
        if(enemyCreators[index] == null){
            slot = null;
        }
        else{
            slot = enemyCreators[index].getSlot(cubeComponents[index]);
        }
        if(slot != null){
            targets[index] = slot;
        }
        else{
            targets[index] = null;
        }
    }

    private void FindEnemyCreatorForComponent(int index){
        EnemyCreator[] tempEnemyCreators = FindObjectsOfType<EnemyCreator>();
        float closestDistance = Mathf.Infinity;
        EnemyCreator closestEnemyCreator = null;
        for(int i = 0; i < tempEnemyCreators.Length; i++){
            float distance = 
                ReturnDistanceBetweenVector2Comparison(tempEnemyCreators[i].transform.position, cubeComponents[index].transform.position);
            if(distance < closestDistance && tempEnemyCreators[i].slotsTakenUp < tempEnemyCreators[i].slotsToInstantiate){
                closestDistance = distance;
                closestEnemyCreator = tempEnemyCreators[i];
            }
        }
        if(closestDistance == Mathf.Infinity){
            enemyCreators[index] = null;
        }
        else{
            enemyCreators[index] = closestEnemyCreator;
        }
    }
    
    private void FindRigidbodyForComponent(int index){
        Rigidbody rb = cubeComponents[index].GetComponent<Rigidbody>();
        if(rb != null){
            rbs[index] = rb;
        }
        else{
            rbs[index] = null;
        }
    
    }
    
    private float ReturnDistanceBetweenVector2Comparison(Vector3 v1, Vector3 v2){
        return Vector2.Distance(new Vector2(v1.x, v1.z), new Vector2(v2.x, v2.z));
    }
}
