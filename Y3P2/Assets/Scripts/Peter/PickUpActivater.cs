using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class PickUpActivater : MonoBehaviour {

    private bool waiting;
    private PlayerPickUpManager pkm;
    private PickUpManager pckm;
    private Entity entity;

    private void Start()
    {
        pkm = GetComponent<PlayerPickUpManager>();
        pckm = FindObjectOfType<PickUpManager>();
        entity = GetComponentInChildren<Entity>();
    }

    private void Update()
    {
        if(pkm.CurrentPickUp != null)
        {
            if(transform == PlayerManager.localPlayer)
            {
                if (Input.GetKeyDown("f") && !waiting)
                {
                    ActivatePickUp(pkm.CurrentPickUp);
                }
            }
           
        }
    }

    public void ActivatePickUp(PickUp pickUp)
    {

        NotificationManager.instance.NewLocalNotification("Activated" + pickUp.PickUpText + "Duration: <Color = blue> " + pickUp.Duration + "/color");
        if (pickUp.Type == PickUp.PickUpType.InfiniteJetpack)
        {
            if (!waiting)
            {
                GetComponentInChildren<PlayerController>().ToggleInfiniteJetPack();
                StartCoroutine(Duration(pickUp));
            }
            
        }
        else if (pickUp.Type == PickUp.PickUpType.Cloak)
        {
            if (!waiting)
            {
                ActivateCloak();
                StartCoroutine(Duration(pickUp));

                if (!entity.photonView.IsMine)
                {
                    entity.paintController.ToggleUI(false);
                }
            }
        }
        else if (pickUp.Type == PickUp.PickUpType.PulseRemote)
        {
            if (!waiting)
            {
                GetComponent<PhotonView>().RPC("PulseRemote", RpcTarget.All);
                StartCoroutine(Duration(pickUp));
            }
        }


        pkm.SetPickUp(null);
        UIManager.instance.SetPickUpImage(null, true);


    }

    [PunRPC]
    private void PulseRemote()
    {
        ObjectPooler.instance.GrabFromPool("MarkCaptureExplosion", pckm.Points[0].transform.position, Quaternion.identity);
    }

    private IEnumerator Duration(PickUp pickUp)
    {
        if (!waiting)
        {
            waiting = true;
            yield return new WaitForSeconds(pickUp.Duration);
            ResetPickUp(pickUp);
            waiting = false;
        }
    }

    private void ResetPickUp(PickUp pickUp)
    {
        if (pickUp.Type == PickUp.PickUpType.InfiniteJetpack)
        {
            GetComponentInChildren<PlayerController>().ToggleInfiniteJetPack();
        }
        else if (pickUp.Type == PickUp.PickUpType.Cloak)
        {
            foreach (GameObject r in pkm.objectsToCloak)
            {
                r.GetComponent<Renderer>().material = r.GetComponent<GetDefaultMat>().DefMaterial;
            }

            if (GameManager.CurrentGameSate == GameManager.GameState.Playing && !entity.photonView.IsMine)
            {
                entity.paintController.ToggleUI(true);
            }
        }
    }

    private void ActivateCloak()
    {
        foreach (GameObject r in pkm.objectsToCloak)
        {
            r.GetComponent<Renderer>().material = pkm.CloakShader;
        }
    }
}
