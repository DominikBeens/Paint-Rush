using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class PickUpManager : MonoBehaviour {

    [SerializeField]
    private List<GameObject> spawnPoints = new List<GameObject>();
    [SerializeField]
    private GameObject itemPadPrefab;

    private void Awake()
    {
        if (PhotonNetwork.IsMasterClient)
        {
            foreach (GameObject s in spawnPoints)
            {
                PhotonNetwork.InstantiateSceneObject(itemPadPrefab.name, s.transform.position, Quaternion.identity);
            }
        }
    }
}
