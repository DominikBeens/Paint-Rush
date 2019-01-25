using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MAKEACTIVE : MonoBehaviour {

    private bool boooool;
	// Use this for initialization
	void Awake () {
	}
	
	// Update is called once per frame
	void Update () {
        if (!gameObject.activeSelf && !boooool)
        {
            gameObject.SetActive(true);
            boooool = true;
        }
    }
}
