using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class SpectatorManager : MonoBehaviourPunCallbacks {

    public static bool spectating;

    [SerializeField]
    private GameObject spectatorDronePrefab;

	
	// Update is called once per frame
	void Update () {
        if (spectating)
        {
            if (Input.GetKeyDown("e"))
            {
                PlayerManager.localPlayer.GetComponent<Rigidbody>().AddRelativeForce(Vector3.back * 100000);

                spectating = false;
            }
        }
	}

    public void SpawnSpectatorDrone()
    {
      
            GameObject g = PhotonNetwork.Instantiate(spectatorDronePrefab.name, new Vector3(-15, 60, 70), Quaternion.identity, 0);
            g.GetComponent<SpectateDrone>().DisableCam();
            spectating = true;
            UIManager.instance.ToggleCrosshair(false);
     
    }

   

}
