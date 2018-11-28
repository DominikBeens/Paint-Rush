using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PickUpCollider : MonoBehaviour {
    private PickUpManager pickM;
    [SerializeField]
    private PickUp myPickUp;
    public PickUp MyPickUp { get { return myPickUp; } }
    private void Start()
    {
        pickM = FindObjectOfType<PickUpManager>();
    }

    private void OnTriggerEnter(Collider other)
    {
        if(other.transform.root.tag == "Player")
        {
            pickM.PickUpPickUp(other.transform.root.gameObject, gameObject);
        }
    }
}
