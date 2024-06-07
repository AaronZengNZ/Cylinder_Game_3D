using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Statistic : MonoBehaviour
{
    public float baseFloat;
    public float statFloat;
    public string statString;
    public string statName;

    public float GetStatFloat(){
        return statFloat;
    }

    public string GetStatString(){
        return statString;
    }
}
