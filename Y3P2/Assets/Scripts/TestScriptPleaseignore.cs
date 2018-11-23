using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
public class TestScriptPleaseignore : MonoBehaviourPun {

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
        if (Input.GetKeyDown(KeyCode.Keypad2))
        {
            PhotonNetwork.LoadLevel(2);
        }
        else if (Input.GetKeyDown(KeyCode.Keypad1))
        {
            PhotonNetwork.LoadLevel(1);
        }
    }
}
