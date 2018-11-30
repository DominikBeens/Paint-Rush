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
                other.transform.root.GetComponent<PickUpActivater>().ActivatePickUp(myPickup);
                StartCoroutine(Cooldown());
                DestroyObject();
            }
        }
    }

    private void DestroyObject()
    {
        Destroy(pickUpObject);
        pickUpObject = null;
    }

    private void SpawnPickUp()
    {

        int i = Random.Range(0, GameManager.instance.PickUps.Count);
        if (PhotonNetwork.IsMasterClient)
        {
            pickUpObject = PhotonNetwork.InstantiateSceneObject(GameManager.instance.PickUps[i].itemPrefab.name, transform.position + new Vector3(0, 1, 0), Quaternion.identity);
            myPickup = GameManager.instance.PickUps[i];
        }
        else
        {
            photonView.RPC("MasterSpawnPickUp", RpcTarget.MasterClient);
        }
    }

    [PunRPC]
    private void MasterSpawnPickUp()
    {
        SpawnPickUp();
    }
}
