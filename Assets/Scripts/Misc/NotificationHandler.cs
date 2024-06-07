using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NotificationHandler : MonoBehaviour
{
    public GameObject notificationPrefab;
    public GameObject notificationParent;
    public float maxNotifications = 5f;
    public void NewNotification(string text){
        GameObject newNotification = Instantiate(notificationPrefab, notificationParent.transform);
        newNotification.GetComponent<NotificationObject>().text = text;
        if(notificationParent.transform.childCount > maxNotifications){
            Destroy(notificationParent.transform.GetChild(0).gameObject);
        }
    }
}
