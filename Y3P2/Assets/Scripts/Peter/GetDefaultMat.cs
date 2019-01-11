using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GetDefaultMat : MonoBehaviour {

    [SerializeField]
    private Material defMaterial;
    public Material DefMaterial { get { return defMaterial; } }
    
	// Use this for initialization
	void Start () {
        defMaterial = GetComponent<Renderer>().material;
	}

    public void UpdateMaterial(Material newMat)
    {
        defMaterial = newMat;
    }
	
	
}
