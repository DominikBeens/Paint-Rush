using Photon.Pun;
using UnityEngine;

public class PlayerManager : MonoBehaviourPunCallbacks
{

    public static PlayerManager instance;

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
}
