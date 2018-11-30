using Photon.Pun;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using Photon.Realtime;

public class PlayerManager : MonoBehaviourPunCallbacks
{

    public static PlayerManager instance;

    #region PlayerComponents
    [SerializeField] private GameObject playerCamera;
    [SerializeField] private PlayerAnimationController playerAnimController;
    [HideInInspector] public Entity entity;
    [HideInInspector] public PlayerController playerController;
    [HideInInspector] public WeaponSlot weaponSlot;
    #endregion

    public class PlayerGameStats
    {
        public int playerPhotonViewID;
        public string playerName;
        public int playerGamePoints;
    }
    private List<PlayerGameStats> playerGameStats = new List<PlayerGameStats>();

    private void Awake()
    {
        if (photonView.IsMine || !PhotonNetwork.IsConnected)
        {
            instance = this;
        }

        GatherPlayerComponents();
        Initialise();
    }

    private void GatherPlayerComponents()
    {
        entity = GetComponentInChildren<Entity>();
        playerController = GetComponentInChildren<PlayerController>();
        weaponSlot = GetComponentInChildren<WeaponSlot>();
        playerAnimController = GetComponentInChildren<PlayerAnimationController>();
    }

    private void Initialise()
    {
        playerCamera.SetActive(IsConnectedAndMine() ? true : false);
        playerController.Inititalise(IsConnectedAndMine());
        weaponSlot.Initialise(IsConnectedAndMine());
        //playerAnimController.Initialise(IsConnectedAndMine());


        if (!IsConnectedAndMine())
        {
            instance.AddToPlayerGameStats(photonView.ViewID, photonView.Owner.NickName);
            SetLayer(transform, 10);
            return;
        }

        AddToPlayerGameStats(photonView.ViewID, photonView.Owner.NickName);

        entity.GetComponent<Collider>().enabled = false;
        GameManager.OnGameStateChanged += GameManager_OnGameStateChanged;

        photonView.RPC("SendGameStats", RpcTarget.Others);

        DontDestroyOnLoad(gameObject);
    }

    private void GameManager_OnGameStateChanged(GameManager.GameState newState)
    {
        playerController.enabled = newState == GameManager.GameState.Playing ? true : false;

        if (newState == GameManager.GameState.Playing)
        {
            Transform randomSpawn = GameManager.instance.GetRandomSpawn();
            transform.position = randomSpawn.position;
            transform.rotation = randomSpawn.rotation;
        }
        else
        {
            transform.position = GameManager.instance.respawnBooth.position;
            SaveManager.saveData.deaths++;
        }
    }

    private void AddToPlayerGameStats(int viewID, string name)
    {
        playerGameStats.Add(new PlayerGameStats { playerPhotonViewID = viewID, playerName = name, playerGamePoints = 0 });
    }

    private void RemoveFromPlayerGameStats(string name)
    {
        for (int i = 0; i < playerGameStats.Count; i++)
        {
            if (playerGameStats[i].playerName == name)
            {
                playerGameStats.RemoveAt(i);
                return;
            }
        }
    }

    private int GetGamePointAmount()
    {
        for (int i = 0; i < playerGameStats.Count; i++)
        {
            if (playerGameStats[i].playerPhotonViewID == photonView.ViewID)
            {
                return playerGameStats[i].playerGamePoints;
            }
        }

        return 0;
    }

    public List<PlayerGameStats> GetSortedPlayerGameStats()
    {
        List<PlayerGameStats> sorted = new List<PlayerGameStats>(playerGameStats);
        sorted = sorted.OrderBy(x => x.playerGamePoints).Reverse().ToList();

        return sorted;
    }

    public void RegisterPlayerGamePoint(int playerViewID)
    {
        for (int i = 0; i < playerGameStats.Count; i++)
        {
            if (playerGameStats[i].playerPhotonViewID == playerViewID)
            {
                playerGameStats[i].playerGamePoints++;
            }
        }
    }

    [PunRPC]
    private void SendGameStats()
    {
        photonView.RPC("ReceiveGameStats", RpcTarget.All, instance.photonView.ViewID, instance.GetGamePointAmount());
    }

    [PunRPC]
    private void ReceiveGameStats(int viewID, int gamePoints)
    {
        for (int i = 0; i < playerGameStats.Count; i++)
        {
            if (playerGameStats[i].playerPhotonViewID == viewID)
            {
                playerGameStats[i].playerGamePoints = gamePoints;
            }
        }
    }

    private bool IsConnectedAndMine()
    {
        if (UnityEngine.SceneManagement.SceneManager.GetActiveScene().buildIndex != 1)
        {
            return true;
        }
        return PhotonNetwork.IsConnected && photonView.IsMine ? true : false;
    }

    private void SetLayer(Transform root, int layer)
    {
        root.gameObject.layer = layer;
        foreach (Transform child in root)
        {
            SetLayer(child, layer);
        }
    }

    public void Respawn()
    {
        Transform randomSpawn = GameManager.instance.GetRandomSpawn();
        transform.position = randomSpawn.position;
        transform.rotation = randomSpawn.rotation;
    }

    public override void OnPlayerLeftRoom(Player otherPlayer)
    {
        RemoveFromPlayerGameStats(otherPlayer.NickName);
    }
}
