using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PickUpManager : MonoBehaviour {

    private List<GameObject> itemPads = new List<GameObject>();
    private bool waiting;

    public void PickUpPickUp(GameObject player, GameObject itemPad)
    {
        ActivatePickUp(player, itemPad.GetComponent<PickUpCollider>().MyPickUp);
    }

    private void ActivatePickUp(GameObject player, PickUp pickUp)
    {
        if (pickUp.Type == PickUp.PickUpType.InfiniteJetpack)
        {
            player.GetComponent<PlayerController>().ToggleInfiniteJetPack();
            StartCoroutine(InfiniteJetPack(player, pickUp.Duration));
        }
    }

    private IEnumerator InfiniteJetPack(GameObject player, int duration)
    {
        if (!waiting)
        {
            waiting = true;
            yield return new WaitForSeconds(duration);
            waiting = false;
            player.GetComponent<PlayerController>().ToggleInfiniteJetPack();
        }
    }
}
