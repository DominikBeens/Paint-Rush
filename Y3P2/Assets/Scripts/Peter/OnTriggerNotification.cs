using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OnTriggerNotification : MonoBehaviour {

    [SerializeField]
    private string message;

    private void OnTriggerEnter(Collider other)
    {
        if(PlayerManager.localPlayer == other.transform.root)
        {
            NotificationManager.instance.NewLocalNotification(message);
        }
    }
}
