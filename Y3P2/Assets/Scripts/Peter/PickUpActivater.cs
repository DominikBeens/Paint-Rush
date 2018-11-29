using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PickUpActivater : MonoBehaviour {

    private bool waiting;

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
    }
}
