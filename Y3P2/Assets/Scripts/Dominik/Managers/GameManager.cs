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
    [SerializeField] private GameObject timeManagerPrefab;
    [SerializeField] private GameObject markCapturePointManagerPrefab;

    [Space(10)]

    [SerializeField] private List<Transform> playerSpawnPoints = new List<Transform>();
    // Temp(?) / Refactorable
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
            InstantiateNetworkedManagers();
        }
    }

    private void Start()
    {
        if (!PlayerManager.instance && playerPrefab)
        {
            Vector3 pos = lobbySpawnPoint.position;
            pos.x += UnityEngine.Random.Range(-2, 2);
            pos.z += UnityEngine.Random.Range(-2, 2);
            PhotonNetwork.Instantiate(playerPrefab.name, pos, lobbySpawnPoint.rotation);

            personalColor = new Color(UnityEngine.Random.value, UnityEngine.Random.value, UnityEngine.Random.value, 1);
            personalColorString = ColorUtility.ToHtmlStringRGBA(personalColor);
        }

        CurrentGameSate = GameState.Lobby;
    }

    private void InstantiateNetworkedManagers()
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

        if (!FindObjectOfType<TimeManager>())
        {
            PhotonNetwork.InstantiateSceneObject(timeManagerPrefab.name, Vector3.zero, Quaternion.identity);
        }

        if (!FindObjectOfType<MarkCapturePointManager>())
        {
            PhotonNetwork.InstantiateSceneObject(markCapturePointManagerPrefab.name, Vector3.zero, Quaternion.identity);
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
            NotificationManager.instance.NewNotification("<color=red>" + newPlayer.NickName + "</color> has entered the game!", NotificationManager.NotificationType.JoinedGame);
        }
    }

    public override void OnPlayerLeftRoom(Player otherPlayer)
    {
        if (PhotonNetwork.IsMasterClient)
        {
            NotificationManager.instance.NewNotification("<color=red>" + otherPlayer.NickName + "</color> has left the game!", NotificationManager.NotificationType.LeftGame);
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
