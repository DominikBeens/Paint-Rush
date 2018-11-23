using Photon.Pun;
using UnityEngine;
using TMPro;

public class PlayerManager : MonoBehaviourPunCallbacks
{

    public static PlayerManager instance;

    [SerializeField] private GameObject playerNameUIPrefab;

    #region PlayerComponents
    [HideInInspector] public Entity entity;
    [SerializeField] private GameObject playerCamera;
    private PlayerController playerController;
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
    }

    private void Initialise()
    {
        playerCamera.SetActive(IsConnectedAndMine() ? true : false);
        playerController.Inititalise(IsConnectedAndMine());
        weaponSlot.Initialise(IsConnectedAndMine());

        if (!IsConnectedAndMine())
        {
            SetupPlayerNamePlate();
            SetLayer(transform, 10);
        }
        else
        {
            entity.GetComponent<Collider>().enabled = false;
            DontDestroyOnLoad(gameObject);
        }
    }

    private void SetupPlayerNamePlate()
    {
        GameObject playerUI = Instantiate(playerNameUIPrefab, transform.position, Quaternion.identity, transform);
        TextMeshProUGUI nameText = playerUI.GetComponentInChildren<TextMeshProUGUI>();
        nameText.text = photonView.Owner.NickName;
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
