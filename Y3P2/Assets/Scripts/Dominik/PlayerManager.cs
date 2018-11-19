using Photon.Pun;
using UnityEngine;

public class PlayerManager : MonoBehaviourPunCallbacks
{

    public static PlayerManager instance;

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

    }

    private void Initialise()
    {
        // This player is not mine.
        if (!IsConnectedAndMine())
        {
            // Set player on another layer.
            //SetLayer(transform, 14);
        }
        // This player is mine.
        else
        {
            DontDestroyOnLoad(gameObject);
        }
    }

    private bool IsConnectedAndMine()
    {
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
