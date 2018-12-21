using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TempTelePortScript : MonoBehaviour {

    private void OnTriggerEnter(Collider other)
    {
        if (other.transform.root.tag == "Player")
        {
            GameManager.instance.SetGameState(1);
        }
    }
}
