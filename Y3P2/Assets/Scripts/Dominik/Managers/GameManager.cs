using Photon.Pun;
using Photon.Realtime;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviourPunCallbacks
{

    public static GameManager instance;

    [SerializeField] private GameObject playerPrefab;
    [SerializeField] private GameObject projectileManagerPrefab;
    [SerializeField] private GameObject notificationManagerPrefab;

    [Space(10)]

    [SerializeField] private List<Transform> playerSpawnPoints = new List<Transform>();

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

    private void Start()
    {
        if (!PlayerManager.instance && playerPrefab)
        {
            Transform randomSpawn = GetRandomSpawn();
            PhotonNetwork.Instantiate(playerPrefab.name, randomSpawn.position, randomSpawn.rotation);
        }

        if (PhotonNetwork.IsMasterClient)
        {
            if (!FindObjectOfType<ProjectileManager>())
            {
                PhotonNetwork.InstantiateSceneObject(projectileManagerPrefab.name, Vector3.zero, Quaternion.identity);
            }

            if (!FindObjectOfType<NotificationManager>())
            {
                PhotonNetwork.InstantiateSceneObject(notificationManagerPrefab.name, Vector3.zero, Quaternion.identity);
            }
        }
    }

    public Transform GetRandomSpawn()
    {
        return playerSpawnPoints[Random.Range(0, playerSpawnPoints.Count)];
    }

    public override void OnPlayerEnteredRoom(Player newPlayer)
    {
        if (PhotonNetwork.IsMasterClient)
        {
            NotificationManager.instance.NewNotification("<color=red>" + newPlayer.NickName + "</color> has entered the game!");
        }
    }

    public override void OnPlayerLeftRoom(Player otherPlayer)
    {
        if (PhotonNetwork.IsMasterClient)
        {
            NotificationManager.instance.NewNotification("<color=red>" + otherPlayer.NickName + "</color> has left the game!");
        }
    }

    public void LeaveRoom()
    {
        PhotonNetwork.LeaveRoom();
    }

    public override void OnLeftRoom()
    {
        DB.MenuPack.SceneManager.instance.LoadScene(0, false);
    }

    public override void OnDisconnected(DisconnectCause cause)
    {
        Debug.LogWarning(cause);

#if UNITY_EDITOR
        return;
#else
        if (cause != DisconnectCause.DisconnectByClientLogic || cause != DisconnectCause.DisconnectByServerLogic)
        {
            PhotonNetwork.Destroy(PlayerManager.instance.gameObject);
        }
#endif
    }
}
