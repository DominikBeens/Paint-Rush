using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Door : MonoBehaviour {

    private Animator anim;
    private bool waiting;

    private List<Collider> nearbyPlayers = new List<Collider>();

	// Use this for initialization
	void Start () {
        anim = GetComponent<Animator>();
	}

    private void OnTriggerEnter(Collider other)
    {
        if(other.transform.root.tag == "Player")
        {
            if (!waiting)
            {
                StartCoroutine(Wait());
            }
        }
    }

    private void OnTriggerStay(Collider other)
    {
        if (!nearbyPlayers.Contains(other))
        {
            nearbyPlayers.Add(other);
        }
    }


    private void OnTriggerExit(Collider other)
    {
        if (nearbyPlayers.Contains(other))
        {
            nearbyPlayers.Remove(other);
        }
        if (other.transform.root.tag == "Player" && nearbyPlayers.Count <= 1)
        {
            anim.SetBool("Close", true);
        }
    }

    private IEnumerator Wait()
    {
        waiting = true;
        anim.SetBool("Close", false);
        anim.SetBool("Open", true);

        yield return new WaitForSeconds(anim.GetCurrentAnimatorStateInfo(0).length);

        anim.SetBool("Open", false);
        waiting = false;
    }
}
