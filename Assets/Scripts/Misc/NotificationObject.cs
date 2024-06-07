using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class NotificationObject : MonoBehaviour
{
    public string text = "<error - no text. you silly goose, fix this NOW>";
    public TextMeshProUGUI textOutput;
    private void Update(){
        textOutput.text = text;
    }
    public void Destroy(){
        Destroy(gameObject);
    }
}
