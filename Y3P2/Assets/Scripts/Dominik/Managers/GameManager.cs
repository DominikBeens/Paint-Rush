using Photon.Pun;
using Photon.Realtime;
using System.Collections.Generic;
using UnityEngine;
using System;

public class GameManager : MonoBehaviourPunCallbacks
{

    public static GameManager instance;
    public static Color32 personalColor;
    public static string personalColorString;

    [SerializeField]
    private List<PickUp> pickUps = new List<PickUp>();
    public List<PickUp> PickUps { get { return pickUps; } }

    [SerializeField]
    private GameObject doorPrefab;
    [SerializeField]
    private List<GameObject> doorSpawnLocs = new List<GameObject>();

    public enum GameState { Lobby, Playing, Respawning };
    private GameState gameState;
    public static GameState CurrentGameSate
    {
        get
        {
            return instance.gameState;
        }
        set
        {
            instance.gameState = value;
            OnGameStateChanged(instance.gameState);
        }
    }
    public static event Action<GameState> OnGameStateChanged = delegate { };

    [SerializeField] private GameObject playerPrefab;
    [SerializeField] private GameObject projectileManagerPrefab;
    [SerializeField] private GameObject notificationManagerPrefab;
    [SerializeField] private GameObject scoreBoardManagerPrefab;

    [Space(10)]

    [SerializeField] private List<Transform> playerSpawnPoints = new List<Transform>();
    // Temp(?)
    public Transform lobbySpawnPoint;
    public Transform respawnBooth;

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

            if (!FindObjectOfType<ScoreboardManager>())
            {
                PhotonNetwork.InstantiateSceneObject(scoreBoardManagerPrefab.name, Vector3.zero, Quaternion.identity);
            }
        }
    }

    private void Start()
    {
        if (!PlayerManager.instance && playerPrefab)
        {
            PhotonNetwork.Instantiate(playerPrefab.name, lobbySpawnPoint.position, lobbySpawnPoint.rotation);

            personalColor = new Color(UnityEngine.Random.value, UnityEngine.Random.value, UnityEngine.Random.value, 1);
            personalColorString = ColorUtility.ToHtmlStringRGBA(personalColor);
        }

        CurrentGameSate = GameState.Lobby;

        if (PhotonNetwork.IsMasterClient)
        {
            SpawnDoors();
        }
    }

    // Used in UnityEvents like in CollisionEventZone.cs
    public void SetGameState(int newState)
    {
        CurrentGameSate = (GameState)newState;
    }

    public Transform GetRandomSpawn()
    {
        return playerSpawnPoints[UnityEngine.Random.Range(0, playerSpawnPoints.Count)];
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

    private void SpawnDoors()
    {
        foreach (GameObject g in doorSpawnLocs)
        {
            PhotonNetwork.InstantiateSceneObject(doorPrefab.name, g.transform.position, g.transform.rotation);
        }
    }
}
