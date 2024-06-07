using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StatsManager : MonoBehaviour
{
    public Statistic[] statistics = new Statistic[1];
    void Start()
    {
        
    }

    public void UpdateStatistic(string statName, float statFloat = 0.42f, string statString = "undefined"){
        foreach(Statistic stat in statistics){
            if(stat.statName == statName){
                if(statFloat != 0.42f){
                    stat.statFloat = statFloat;
                }
                if(statString != "undefined"){
                    stat.statString = statString;
                }
            }
        }
    }

    public void UpdateStatCalculation(string statName, float statAdditive, float statMultiplicative = 1f){
        foreach(Statistic stat in statistics){
            if(stat.statName == statName){
                stat.statFloat = (stat.baseFloat + statAdditive) * statMultiplicative;
            }
        }
    }

    public float GetStatFloat(string statName){
        foreach(Statistic stat in statistics){
            if(stat.statName == statName){
                return stat.statFloat;
            }
        }
        return 0.42f;
    }

    public string GetStatString(string statName){
        foreach(Statistic stat in statistics){
            if(stat.statName == statName){
                return stat.statString;
            }
        }
        return "undefined";
    }
    // Update is called once per frame
    void Update()
    {
        
    }
}
