using Photon.Pun;
using UnityEngine;

public class PropSpawner : MonoBehaviour
{

    [SerializeField] private GameObject spawnable;

    private void Start()
    {
        if (PhotonNetwork.IsMasterClient)
        {
            PhotonNetwork.InstantiateSceneObject(spawnable.name, transform.position, Quaternion.identity);
        }
    }
}
