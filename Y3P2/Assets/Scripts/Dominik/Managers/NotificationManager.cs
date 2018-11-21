using Photon.Pun;
using UnityEngine;

public class NotificationManager : MonoBehaviourPunCallbacks
{

    public static NotificationManager instance;

    [SerializeField] private Transform notificationSpawn;

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

    public void NewNotification(string text)
    {
        photonView.RPC("SendNotification", RpcTarget.All, text);
    }

    [PunRPC]
    private void SendNotification(string text)
    {
        Notification newNotification = ObjectPooler.instance.GrabFromPool("Notification", notificationSpawn.position, Quaternion.identity).GetComponent<Notification>();
        newNotification.transform.SetParent(notificationSpawn);
        newNotification.Initialise(text);
    }
}
