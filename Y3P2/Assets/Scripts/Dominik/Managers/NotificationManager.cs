using Photon.Pun;
using System.Collections.Generic;
using UnityEngine;

public class NotificationManager : MonoBehaviourPunCallbacks
{

    public static NotificationManager instance;

    private Queue<string> notificationQueue = new Queue<string>();
    private float nextNotificationTime;

    private Queue<string> localNotificationQueue = new Queue<string>();
    private float nextLocalNotificationTime;

    [SerializeField] private Transform notificationSpawn;
    [SerializeField] private Transform localNotificationSpawn;
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
                ShowNotification(notificationQueue.Dequeue(), false);
            }
        }

        if (Time.time >= nextLocalNotificationTime)
        {
            if (localNotificationQueue.Count > 0)
            {
                nextLocalNotificationTime = Time.time + notificationInterval;
                ShowNotification(localNotificationQueue.Dequeue(), true);
            }
        }
    }

    public void NewNotification(string text)
    {
        photonView.RPC("SendNotification", RpcTarget.All, text);
    }

    public void NewLocalNotification(string text)
    {
        localNotificationQueue.Enqueue(text);
    }

    [PunRPC]
    private void SendNotification(string text)
    {
        notificationQueue.Enqueue(text);
    }

    private void ShowNotification(string text, bool local)
    {
        Transform spawn = local ? localNotificationSpawn : notificationSpawn;

        Notification newNotification = ObjectPooler.instance.GrabFromPool("Notification", spawn.position, Quaternion.identity).GetComponent<Notification>();
        newNotification.transform.SetParent(spawn);
        newNotification.Initialise(text);
    }
}
