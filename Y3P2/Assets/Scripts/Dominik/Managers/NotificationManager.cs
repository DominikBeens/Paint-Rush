using Photon.Pun;
using System.Collections.Generic;
using UnityEngine;

public class NotificationManager : MonoBehaviourPunCallbacks
{

    public static NotificationManager instance;

    private Queue<string> notificationQueue = new Queue<string>();
    private float nextNotificationTime;

    [SerializeField] private Transform notificationSpawn;
    [SerializeField] private float notificationInterval = 1f;

    private void Awake()
    {
        if (!instance)
        {
            instance = this;
        }
        else if (instance && instance != this)
        {
            Destroy(this);
        }
    }

    private void Update()
    {
        if (Time.time >= nextNotificationTime)
        {
            if (notificationQueue.Count > 0)
            {
                nextNotificationTime = Time.time + notificationInterval;
                ShowNotification(notificationQueue.Dequeue());
            }
        }
    }

    public void NewNotification(string text)
    {
        photonView.RPC("SendNotification", RpcTarget.All, text);
    }

    public void NewLocalNotification(string text)
    {
        SendNotification(text);
    }

    [PunRPC]
    private void SendNotification(string text)
    {
        notificationQueue.Enqueue(text);
    }

    private void ShowNotification(string text)
    {
        Notification newNotification = ObjectPooler.instance.GrabFromPool("Notification", notificationSpawn.position, Quaternion.identity).GetComponent<Notification>();
        newNotification.transform.SetParent(notificationSpawn);
        newNotification.Initialise(text);
    }
}
