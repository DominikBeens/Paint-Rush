using Photon.Pun;
using System.Collections.Generic;
using UnityEngine;

public class NotificationManager : MonoBehaviourPunCallbacks
{

    public static NotificationManager instance;

    private Queue<string> notificationQueue = new Queue<string>();
    private Queue<NotificationType> notificationTypeQueue = new Queue<NotificationType>();
    private float nextNotificationTime;

    private Queue<string> localNotificationQueue = new Queue<string>();
    private Queue<NotificationType> localNotificationTypeQueue = new Queue<NotificationType>();
    private float nextLocalNotificationTime;
    private string lastLocalQueueEntry;

    public enum NotificationType { Default, MarkCaptured, MarkGained, MarkDestroyed, KillStreak, JoinedGame, LeftGame };

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
                ShowNotification(notificationQueue.Dequeue(), false, notificationTypeQueue.Dequeue());
            }
        }

        if (Time.time >= nextLocalNotificationTime)
        {
            if (localNotificationQueue.Count > 0)
            {
                nextLocalNotificationTime = Time.time + notificationInterval;
                ShowNotification(localNotificationQueue.Dequeue(), true, localNotificationTypeQueue.Dequeue());
            }
        }
    }

    public void NewNotification(string text, NotificationType type = NotificationType.Default)
    {
        photonView.RPC("SendNotification", RpcTarget.All, text, (int)type);
    }

    public void NewLocalNotification(string text, NotificationType type = NotificationType.Default)
    {
        if (text == lastLocalQueueEntry && localNotificationQueue.Contains(lastLocalQueueEntry))
        {
            return;
        }

        localNotificationQueue.Enqueue(text);
        localNotificationTypeQueue.Enqueue(type);
        lastLocalQueueEntry = text;
    }

    [PunRPC]
    private void SendNotification(string text, int type)
    {
        notificationQueue.Enqueue(text);
        notificationTypeQueue.Enqueue((NotificationType)type);
    }

    private void ShowNotification(string text, bool local, NotificationType type)
    {
        Transform spawn = local ? localNotificationSpawn : notificationSpawn;

        Notification newNotification = ObjectPooler.instance.GrabFromPool("Notification", spawn.position, Quaternion.identity).GetComponent<Notification>();
        newNotification.transform.SetParent(spawn);
        newNotification.Initialise(text);

        // Play audio here.
        switch (type)
        {
            case NotificationType.Default:
                break;

            case NotificationType.MarkCaptured:
                break;

            case NotificationType.MarkGained:
                break;

            case NotificationType.MarkDestroyed:
                break;

            case NotificationType.KillStreak:
                break;

            case NotificationType.JoinedGame:
                break;

            case NotificationType.LeftGame:
                break;
        }
    }
}
