using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Door : MonoBehaviour
{

    private Animator anim;
    private bool waiting;
    private Transform mainCam;

    private List<Collider> nearbyPlayers = new List<Collider>();

    [SerializeField] private bool usePortal;
    [SerializeField] private Door connectedDoor;
    [SerializeField] private Transform connectedPortalCam;
    [SerializeField] private Material portalMat;
    [SerializeField] private Renderer portalVisual;

    private void Start()
    {
        anim = GetComponent<Animator>();
        mainCam = Camera.main.transform;

        if (usePortal)
        {
            portalMat = Instantiate(portalMat);

            Camera portalCam = connectedPortalCam.GetComponent<Camera>();
            portalCam.targetTexture = new RenderTexture(Screen.width, Screen.height, 24);
            portalMat.mainTexture = portalCam.targetTexture;

            portalVisual.material = portalMat;

            GetComponentInChildren<PortalTeleporter>().Init(connectedPortalCam);
        }

        connectedPortalCam.gameObject.SetActive(false);
    }

    private void Update()
    {
        if (!usePortal)
        {
            return;
        }

        if (nearbyPlayers.Count > 0)
        {
            Vector3 offset = PlayerManager.localPlayer.position - transform.position;
            Vector3 newPos = connectedDoor.transform.position + offset;
            connectedPortalCam.transform.position = new Vector3(newPos.x, mainCam.position.y, newPos.z);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.transform.root.tag == "Player")
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

        if (usePortal)
        {
            connectedDoor.PortalOpen(other);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (nearbyPlayers.Contains(other))
        {
            nearbyPlayers.Remove(other);
        }

        if (other.transform.root.tag == "Player" && nearbyPlayers.Count <= 0)
        {
            anim.SetBool("Close", true);
        }

        if (usePortal)
        {
            connectedDoor.PortalClose(other);
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
    
    public void PortalOpen(Collider player)
    {
        if (!nearbyPlayers.Contains(player))
        {
            nearbyPlayers.Add(player);

            if (!waiting)
            {
                StartCoroutine(Wait());
            }
        }
    }

    public void PortalClose(Collider player)
    {
        if (nearbyPlayers.Contains(player))
        {
            nearbyPlayers.Remove(player);

            if (nearbyPlayers.Count <= 0)
            {
                anim.SetBool("Close", true);
            }
        }
    }
}
