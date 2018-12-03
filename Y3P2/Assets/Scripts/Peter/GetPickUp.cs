using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class GetPickUp : MonoBehaviour {

    [SerializeField]
    private float minCooldown;
    [SerializeField]
    private float maxCooldown;
    [SerializeField]
    private PickUp myPickup;
    [SerializeField]
    private GameObject pickUpObject;

    private bool cooldown;

    private PhotonView photonView;

    private void Start()
    {
        photonView = GetComponent<PhotonView>();

       
        SpawnPickUp();
    }

    public IEnumerator Cooldown()
    {

        cooldown = true;
        yield return new WaitForSeconds(Random.Range(minCooldown, maxCooldown));
        cooldown = false;

        SpawnPickUp();
    }

    public void OnTriggerEnter(Collider other)
    {
        if(other.transform.root.tag == "Player")
        {
            if (!cooldown)
            {
                other.transform.root.GetComponent<PlayerPickUpManager>().CheckChildren();
                other.transform.root.GetComponent<PickUpActivater>().ActivatePickUp(myPickup);
                StartCoroutine(Cooldown());
                if (PhotonNetwork.IsMasterClient)
                {
                    DestroyObject();
                }
            }
        }
    }

    private void DestroyObject()
    {
        if(pickUpObject != null)
        {
            PhotonNetwork.Destroy(pickUpObject);
        }

        myPickup = null;
    }

    private void SpawnPickUp()
    {

        int i = Random.Range(0, GameManager.instance.PickUps.Count);
       if(myPickup == null)
        {
            pickUpObject = PhotonNetwork.InstantiateSceneObject(GameManager.instance.PickUps[i].itemPrefab.name, transform.position + new Vector3(0, 1, 0), Quaternion.identity);
            myPickup = GameManager.instance.PickUps[i];
        }
       
    }

  
}
