using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class PickUpActivater : MonoBehaviour {

    private bool waiting;
    private PlayerPickUpManager pkm;
    private Entity entity;

    private void Start()
    {
        pkm = GetComponent<PlayerPickUpManager>();
        entity = GetComponentInChildren<Entity>();
    }

    private void Update()
    {
        if(pkm.CurrentPickUp != null)
        {
            if (Input.GetKeyDown("f") && !waiting)
            {
                ActivatePickUp(pkm.CurrentPickUp);
            }
        }
    }

    public void ActivatePickUp(PickUp pickUp)
    {
        if(pickUp.Type == PickUp.PickUpType.InfiniteJetpack)
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


        pkm.SetPickUp(null);
        UIManager.instance.SetPickUpImage(null, true);


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
