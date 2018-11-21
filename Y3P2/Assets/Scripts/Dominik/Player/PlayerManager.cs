using Photon.Pun;
using UnityEngine;

public class PlayerManager : MonoBehaviourPunCallbacks
{

    public static PlayerManager instance;

    #region PlayerComponents
    [SerializeField] private GameObject playerCamera;
    private FirstPersonController playerController;
    private WeaponSlot weaponSlot;
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
        playerController = GetComponentInChildren<FirstPersonController>();
        weaponSlot = GetComponentInChildren<WeaponSlot>();
    }

    private void Initialise()
    {
        playerCamera.SetActive(IsConnectedAndMine() ? true : false);
        playerController.Inititalise(IsConnectedAndMine());
        weaponSlot.Initialise(IsConnectedAndMine());

        if (!IsConnectedAndMine())
        {
            SetLayer(transform, 10);
        }
        else
        {
            DontDestroyOnLoad(gameObject);
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
}
