using Photon.Pun;
using UnityEngine;

public class SpectatorManager : MonoBehaviourPunCallbacks
{

    [SerializeField]
    private GameObject spectatorDronePrefab;

    public void SpawnSpectatorDrone()
    {
        GameManager.CurrentGameSate = GameManager.GameState.Spectating;

        GameObject g = PhotonNetwork.Instantiate(spectatorDronePrefab.name, new Vector3(-15, 60, 70), Quaternion.identity, 0);
        g.GetComponent<SpectateDrone>().DisableCam();
    }
}
