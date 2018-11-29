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
    private GameObject pickUpParent;

    private bool cooldown;

    private void Start()
    {
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
                DestroyChildren();
            }
        }
    }

    private void DestroyChildren()
    {
        foreach (Transform t in pickUpParent.transform)
        {
            Destroy(t.gameObject);
        }
    }

    private void SpawnPickUp()
    {
        GameObject g = PhotonNetwork.InstantiateSceneObject(GameManager.instance.PickUps[Random.Range(0, GameManager.instance.PickUps.Count)].itemPrefab.name, Vector3.zero, Quaternion.identity);
        g.transform.SetParent(pickUpParent.transform);
        g.transform.localPosition = Vector3.zero;
    }
}
