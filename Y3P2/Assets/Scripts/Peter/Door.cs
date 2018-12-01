using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Door : MonoBehaviour {

    private Animator anim;
    private bool waiting;

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

    private IEnumerator Wait()
    {
        waiting = true;
        anim.SetBool("Open", true);

        yield return new WaitForSeconds(anim.GetCurrentAnimatorStateInfo(0).length);

        anim.SetBool("Open", false);
        waiting = false;
    }
}
