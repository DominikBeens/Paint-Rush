using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class PickUpActivater : MonoBehaviour {

    private bool waiting;
    private PlayerPickUpManager pkm;

    private void Start()
    {
        pkm = GetComponent<PlayerPickUpManager>();
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
            }

        }
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
