using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class PickUpActivater : MonoBehaviourPunCallbacks{

    private bool waiting;
    private PlayerPickUpManager pkm;
    private PickUpManager pckm;
    private Entity entity;
    private SyncPlayerSkin skinSync;
    private bool reducePaint = false;


    private void Start()
    {
        pkm = GetComponent<PlayerPickUpManager>();
        pckm = FindObjectOfType<PickUpManager>();
        entity = GetComponentInChildren<Entity>();
        skinSync = GetComponent<SyncPlayerSkin>();
    }

    private void Update()
    {
        if(pkm.CurrentPickUp != null)
        {
            if(transform == PlayerManager.localPlayer)
            {
                if (Input.GetKeyDown("f") && !waiting)
                {
                    if (pkm.HasPickUp)
                    {
                        ActivatePickUp(pkm.CurrentPickUp);
                    }
                }
            }
           
        }

        //if (Input.GetKeyDown("o"))
        //{
        //    entity.Hit(2, 10);
        //}
    }


    public void ResetWaiting()
    {
        waiting = false;
    }

    public void ActivatePickUp(PickUp pickUp)
    {

        if(pickUp.Duration > 0)
        {
            NotificationManager.instance.NewLocalNotification("Activated " + pickUp.PickUpText + "<color=yellow> Duration:  " + pickUp.Duration + "</color>");
        }
        else
        {
            NotificationManager.instance.NewLocalNotification("Activated " + pickUp.PickUpText);
        }
        if (pickUp.Type == PickUp.PickUpType.InfiniteJetpack)
        {
            if (!waiting)
            {
                GetComponentInChildren<PlayerController>().ToggleInfiniteJetPack(true);
                StartCoroutine(Duration(pickUp));
            }
            
        }
        else if (pickUp.Type == PickUp.PickUpType.Cloak)
        {
            if (!waiting)
            {
                GetComponent<PhotonView>().RPC("ActivateCloak", RpcTarget.All);
                skinSync.ToggleCloak(true);
                //ActivateCloak();
                StartCoroutine(Duration(pickUp));

               
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
        else if (pickUp.Type == PickUp.PickUpType.ColorVac)
        {
            if (!waiting)
            {
                // GetComponent<PhotonView>().RPC("ColorVac", RpcTarget.All);
                ColorVac();
                StartCoroutine(Duration(pickUp));
            }
        }
        else if (pickUp.Type == PickUp.PickUpType.GrenadeLauncher)
        {
            if (!waiting)
            {
                PlayerManager.instance.weaponSlot.EquipWeapon(pickUp.Weapon);
                waiting = true;
            }
        }
        else if (pickUp.Type == PickUp.PickUpType.ForceField)
        {
            if (!waiting)
            {
                photonView.RPC("ForceField", RpcTarget.All);
                skinSync.ToggleShield(true);
                StartCoroutine(Duration(pickUp));
            }
        }

        UIManager.instance.SetPickUpImage(null, true);
        UIManager.instance.PickUpImageParent.transform.gameObject.SetActive(false);

        PlayerManager.instance.playerAudioManager.PlayClipOnce(PlayerManager.instance.playerAudioManager.GetClip("pickup_used"));

        pkm.ResetHasPickUp();
    }

    [PunRPC]
    private void ForceField()
    {
        pkm.SetShieldedBool(true);
        foreach (GameObject g in pkm.ObjectsToShield)
        {
            g.GetComponent<Renderer>().material = pkm.ForceFieldShader;
        }
    }

    [PunRPC]
    private void ResetForceField()
    {
        pkm.SetShieldedBool(false);
        foreach (GameObject g in pkm.ObjectsToShield)
        {
            g.GetComponent<Renderer>().material = g.GetComponent<GetDefaultMat>().DefMaterial;
        }
    }

    [PunRPC]
    private void PulseRemote()
    {
        foreach (MarkCapturePoint g in pckm.Points)
        {
            if (g.IsActive)
            {
                ObjectPooler.instance.GrabFromPool("MarkCaptureExplosion", g.transform.position, Quaternion.identity);
            }
        }
    }

    [PunRPC]
    private void ColorVac()
    {
        if (!reducePaint)
        {
            if(pkm.CurrentPickUp != null)
            {
                float f = pkm.CurrentPickUp.Duration;
                float d = pkm.CurrentPickUp.Damage;
                StartCoroutine(ReducePaint(f, d));
            }

        }
    }

    private IEnumerator ReducePaint(float t, float d)
    {
        reducePaint = true;
        for (int i = 0; i < t; i++)
        {
            yield return new WaitForSeconds(.15F);
            entity.HitAll(d);
            if (i >= t -1)
            {
                reducePaint = false;
            }
        }

    }
  

    public IEnumerator Duration(PickUp pickUp)
    {
        if (!waiting)
        {
            waiting = true;
            yield return new WaitForSeconds(pickUp.Duration);
            ResetPickUp(pickUp);
            waiting = false;
        }
    }

    public void ResetPickUp(PickUp pickUp)
    {
        if (pickUp.Type == PickUp.PickUpType.InfiniteJetpack)
        {
            GetComponentInChildren<PlayerController>().ToggleInfiniteJetPack(false);
        }
        else if(pickUp.Type == PickUp.PickUpType.Cloak)
        {
            skinSync.ToggleCloak(false);
            GetComponent<PhotonView>().RPC("ResetCloak", RpcTarget.All);
        }
        else if (pickUp.Type == PickUp.PickUpType.ColorVac)
        {
            //GetComponent<PhotonView>().RPC("ColorVac", RpcTarget.All);
            reducePaint = false;
        }
        else if (pickUp.Type == PickUp.PickUpType.GrenadeLauncher)
        {
            pkm.ResetWeapon();
        }
        else if (pickUp.Type == PickUp.PickUpType.ForceField)
        {
            GetComponent<PhotonView>().RPC("ResetForceField", RpcTarget.All);
            skinSync.ToggleShield(false);
        }
    }

    [PunRPC]
    private void ActivateCloak()
    {
        if (!entity.photonView.IsMine)
        {
            entity.paintController.ToggleUI(false);
        }

        foreach (GameObject r in pkm.objectsToCloak)
        {
            if(r != null)
            {
                r.GetComponent<Renderer>().material = pkm.CloakShader;
            }
        }
    }

    [PunRPC]
    private void ResetCloak()
    {
        foreach (GameObject r in pkm.objectsToCloak)
        {
            if (r != null)
            {
                r.GetComponent<Renderer>().material = r.GetComponent<GetDefaultMat>().DefMaterial;
            }
        }

        if (GameManager.CurrentGameSate == GameManager.GameState.Playing && !entity.photonView.IsMine)
        {
            entity.paintController.ToggleUI(true);
        }
    }
}
