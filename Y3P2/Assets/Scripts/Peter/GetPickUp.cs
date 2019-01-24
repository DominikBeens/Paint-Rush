using Photon.Pun;
using Photon.Realtime;
using System.Collections.Generic;
using UnityEngine;
using System.Collections;

public class GetPickUp : MonoBehaviourPunCallbacks
{

    [SerializeField]
    private float minCooldown;
    [SerializeField]
    private float maxCooldown;
    [SerializeField]
    private PickUp myPickup;
    [SerializeField]
    private GameObject pickUpObject;

    private bool cooldown;

    public bool cheatPad; //I know it's public
    
    public List<PickUp> pickUps = new List<PickUp>(); //It's temporary cheating code

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
        if (other.transform.root.tag == "Player")
        {
            if (other.transform.root == PlayerManager.localPlayer)
            {
                if (!cooldown)
                {
                   // other.transform.root.GetComponent<PlayerPickUpManager>().CheckChildren();
                    other.transform.root.GetComponent<PlayerPickUpManager>().SetPickUp(myPickup);
                    UIManager.instance.PickUpImageParent.transform.gameObject.SetActive(true);
                    UIManager.instance.SetPickUpImage(myPickup.PickUpSprite, false);
                    NotificationManager.instance.NewLocalNotification(myPickup.PickUpText);

                    PlayerManager.instance.playerAudioManager.PlayClipOnce(PlayerManager.instance.playerAudioManager.GetClip("pickup_gained"));
                    SaveManager.instance.SaveStat(SaveManager.SavedStat.PickupsCollected);
                }
            }
            if (!cooldown)
            {
                StartCoroutine(Cooldown());
                photonView.RPC("DestroyObject", RpcTarget.All);
            }
        }
    }

    [PunRPC]
    private void DestroyObject()
    {
        if (!PhotonNetwork.IsMasterClient)
        {
            return;
        }

        if (PhotonNetwork.IsMasterClient && pickUpObject != null)
        {
            PhotonNetwork.Destroy(pickUpObject);
            myPickup = null;

        }
    }

    private void SpawnPickUp()
    {
        // Only the master client handles pickups and syncs it to other clients using OnPhotonSerializeView().
        if (!PhotonNetwork.IsMasterClient)
        {
            return;
        }

        int pickupType = Random.Range(0, GameManager.instance.PickUps.Count);
        if (myPickup == null)
        {
            photonView.RPC("SpawnPickupRPC", RpcTarget.All, pickupType);
            photonView.RPC("SyncMyPickup", RpcTarget.Others, (int)myPickup.Type);
        }
    }

    [PunRPC]
    private void SpawnPickupRPC(int pickupType)
    {
        if (PhotonNetwork.IsMasterClient)
        {
            if (!cheatPad)
            {
                pickUpObject = PhotonNetwork.InstantiateSceneObject(GameManager.instance.PickUps[pickupType].itemPrefab.name, transform.position + new Vector3(0, 1, 0), Quaternion.identity);
                myPickup = GameManager.instance.PickUps[pickupType];
            }
            else
            {
                pickUpObject = PhotonNetwork.InstantiateSceneObject(pickUps[0].itemPrefab.name, transform.position + new Vector3(0, 1, 0), Quaternion.identity);
                myPickup = pickUps[0];
            }

        }
    }

    // Receive myPickup data.
    [PunRPC]
    private void SyncMyPickup(int pickupType)
    {
        myPickup = GameManager.instance.PickUps[pickupType];
    }

    // Send myPickup to everyone so that the new player receives the correct data.
    public override void OnPlayerEnteredRoom(Player newPlayer)
    {
        if (PhotonNetwork.IsMasterClient)
        {
            photonView.RPC("SyncMyPickup", RpcTarget.Others, (int)myPickup.Type);
        }
    }
}
