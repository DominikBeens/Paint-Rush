using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class JumpPad : MonoBehaviour {

    public float upForce = 10;
    public float forwardForce = 2000;
	
    private void Launch(Rigidbody player)
    {
        player.AddRelativeForce(Vector3.forward * forwardForce, ForceMode.Impulse);
        player.AddRelativeForce(Vector3.up * upForce, ForceMode.Impulse);
    }

    private void OnTriggerEnter(Collider other)
    {
        if(other.transform.root.tag == "Player" && other.transform.root.GetComponent<Photon.Pun.PhotonView>().IsMine)
        {
            Launch(other.transform.root.GetComponent<Rigidbody>());
        }
    }
}
