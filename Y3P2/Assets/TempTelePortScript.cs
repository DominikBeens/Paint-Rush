using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class TempTelePortScript : MonoBehaviour {

    private void OnTriggerEnter(Collider other)
    {
        if (other.transform.root.tag == "Player")
        {
            if (other.GetComponent<PhotonView>().IsMine)
            {
                GameManager.instance.SetGameState(1);
            }
        }
    }
}
