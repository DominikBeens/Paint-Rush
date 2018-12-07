using Photon.Pun;
using UnityEngine;

public class PlayerManager : MonoBehaviourPunCallbacks, IPunObservable
{

    public static PlayerManager instance;

    // Basically a copy of the GameManagers gamestate but this one is accesible from all clients and 
    // the one from the GameManager not since everyone has his own GameManager.
    private GameManager.GameState playerState;
    public GameManager.GameState PlayerState { get { return playerState; } }

    #region PlayerComponents
    private PlayerAnimationController playerAnimController;
    [SerializeField] private GameObject playerCamera;
    [HideInInspector] public Entity entity;
    [HideInInspector] public PlayerController playerController;
    [HideInInspector] public WeaponSlot weaponSlot;
    #endregion

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
            SetLayer(transform, 10);
            return;
        }

        entity.GetComponent<Collider>().enabled = false;
        GameManager.OnGameStateChanged += GameManager_OnGameStateChanged;

        DontDestroyOnLoad(gameObject);
    }

    private void Update()
    {
        //TESTING
        if (photonView.IsMine)
        {
            if (Input.GetKeyDown(KeyCode.T))
            {
                if (GameManager.CurrentGameSate == GameManager.GameState.Playing)
                {
                    GameManager.CurrentGameSate = GameManager.GameState.Respawning;
                }
                else if (GameManager.CurrentGameSate == GameManager.GameState.Respawning)
                {
                    GameManager.CurrentGameSate = GameManager.GameState.Lobby;
                }
                else
                {
                    GameManager.CurrentGameSate = GameManager.GameState.Playing;
                }
            }
        }
    }

    private void GameManager_OnGameStateChanged(GameManager.GameState newState)
    {
        SetPlayerState(newState);

        switch (newState)
        {
            case GameManager.GameState.Lobby:

                transform.position = GameManager.instance.lobbySpawnPoint.position;
                playerController.enabled = true;
                break;
            case GameManager.GameState.Playing:

                Transform randomSpawn = GameManager.instance.GetRandomSpawn();
                transform.position = randomSpawn.position;
                transform.rotation = randomSpawn.rotation;
                playerController.enabled = true;
                break;
            case GameManager.GameState.Respawning:

                transform.position = GameManager.instance.respawnBooth.position;
                SaveManager.instance.SaveStat(SaveManager.SavedStat.Deaths);
                playerController.enabled = false;
                break;
        }
    }

    private void SetPlayerState(GameManager.GameState newState)
    {
        playerState = newState;

        if (!photonView.IsMine)
        {
            switch (newState)
            {
                case GameManager.GameState.Lobby:

                    entity.paintController.ToggleUI(false);
                    break;
                case GameManager.GameState.Playing:

                    entity.paintController.ToggleUI(true);
                    break;
                case GameManager.GameState.Respawning:

                    entity.paintController.ToggleUI(false);
                    break;
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

    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        if (stream.IsWriting)
        {
            stream.SendNext((int)GameManager.CurrentGameSate);
        }
        else
        {
            if (!photonView.IsMine)
            {
                GameManager.GameState newState = (GameManager.GameState)stream.ReceiveNext();
                if (playerState != newState)
                {
                    SetPlayerState(newState);
                }
            }
        }
    }
}
