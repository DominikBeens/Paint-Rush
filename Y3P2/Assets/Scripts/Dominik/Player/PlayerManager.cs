using Photon.Pun;
using UnityEngine;

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
