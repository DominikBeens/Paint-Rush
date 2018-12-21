﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using UnityEngine.UI;
using TMPro;
public class PlayerPickUpManager : MonoBehaviourPunCallbacks {

    [SerializeField]
    private Material cloakShader;
    public Material CloakShader { get { return cloakShader; } }

    [SerializeField]
    private Weapon defaultWeapon;
    public Weapon DefaultWeapon { get { return defaultWeapon; } }

    [SerializeField]
    public List<GameObject> objectsToCloak = new List<GameObject>();
    public List<GameObject> ObjectsToCloak { get { return ObjectsToCloak; } }


    private PickUp currentPickUp;
    public PickUp CurrentPickUp { get { return currentPickUp; } }

    private bool hasPickUp;
    public bool HasPickUp { get { return hasPickUp; } }

    public Transform pickUpRoomLocation; //Also temp cheat code


    private void Start()
    {
        photonView.RPC("CheckChildren", RpcTarget.AllBuffered);

        if (photonView.IsMine)
        {
            GameManager.OnGameStateChanged += GameManager_OnGameStateChanged;
            WeaponSlot.OnFireWeapon += WeaponSlot_OnFireWeapon;
        }

        pickUpRoomLocation = GameObject.Find("PickRoomSpawnPos").transform;
    }

    private void Update()
    {
        if (Input.GetKeyDown("p") && photonView.IsMine)
        {
            transform.position = pickUpRoomLocation.position;
        }
    }

    private void WeaponSlot_OnFireWeapon()
    {
        if (WeaponSlot.currentWeapon.pickupExclusive)
        {
            StartCoroutine(ResetPickupWeapon());
        }
    }

    private IEnumerator ResetPickupWeapon()
    {
        yield return new WaitForEndOfFrame();
        ResetWeapon();
        GetComponent<PickUpActivater>().ResetWaiting();
    }

    private IEnumerator ResetPickUpOnDeath()
    {
        NotificationManager.instance.NewLocalNotification("Reset 1");

        yield return new WaitForEndOfFrame();
        GetComponent<PickUpActivater>().StopCoroutine("Duration");
        GetComponent<PickUpActivater>().ResetWaiting();
        GetComponent<PickUpActivater>().ResetPickUp(currentPickUp);
        ResetCurrentPickUp();
        NotificationManager.instance.NewLocalNotification("Reset 2");
    }

    private void GameManager_OnGameStateChanged(GameManager.GameState newState)
    {
        if (newState == GameManager.GameState.Respawning)
        {
            if (currentPickUp != null)
            {
                StartCoroutine(ResetPickUpOnDeath());
            }
        }
    }

    public IEnumerator JumpCooldownIcon(float coolDowntime)
    {
        UIManager.instance.JumpCooldownIcon.SetActive(true);
        yield return new WaitForSeconds(coolDowntime);
        UIManager.instance.JumpCooldownIcon.SetActive(false);
    }

    [PunRPC]
    public void CheckChildren()
    {
        objectsToCloak.Clear();

        GetDefaultMat[] mats = transform.GetComponentsInChildren<GetDefaultMat>();

        foreach (GetDefaultMat df in mats)
        {
            if (df != null)
            {
                if (!objectsToCloak.Contains(df.gameObject))
                {
                    objectsToCloak.Add(df.gameObject);
                }
            }
        }
    }

    public void CheckChildrenRPC()
    {
        photonView.RPC("CheckChildren", RpcTarget.AllBuffered);
    }

    public void SetPickUp(PickUp pickUp)
    {
        currentPickUp = pickUp;
        hasPickUp = true;
    }

    public void ResetCurrentPickUp()
    {
        SetPickUp(null);
        UIManager.instance.SetPickUpImage(null, true);
        UIManager.instance.PickUpImageParent.transform.gameObject.SetActive(false);
    }

    public void ResetWeapon()
    {
        PlayerManager.instance.weaponSlot.EquipWeapon(defaultWeapon);
        photonView.RPC("CheckChildren", RpcTarget.AllBuffered);
    }

    public void ResetHasPickUp()
    {
        hasPickUp = false;
    }

    public override void OnDisable()
    {
        if (photonView.IsMine)
        {
            GameManager.OnGameStateChanged -= GameManager_OnGameStateChanged;
            WeaponSlot.OnFireWeapon -= WeaponSlot_OnFireWeapon;
        }
    }
}
