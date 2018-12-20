using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using UnityEngine.UI;
using TMPro;
public class PlayerPickUpManager : MonoBehaviour {

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


    private void Start()
    {
        GetComponent<PhotonView>().RPC("CheckChildren", RpcTarget.AllBuffered);
        //UIManager.instance.JumpCooldownIcon.SetActive(false);
    }

    private void Update()
    {
        if(GameManager.CurrentGameSate == GameManager.GameState.Respawning)
        {
            if(currentPickUp != null)
            {
                ResetCurrentPickUp();
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
        GetComponent<PhotonView>().RPC("CheckChildren", RpcTarget.AllBuffered);
    }

    public void SetPickUp(PickUp pickUp)
    {
        currentPickUp = pickUp;
    }

    public void ResetCurrentPickUp()
    {
        SetPickUp(null);
        UIManager.instance.SetPickUpImage(null, true);
        UIManager.instance.PickUpImageParent.transform.gameObject.SetActive(false);
    }

    public void ResetWeapon()
    {
        GetComponent<WeaponSlot>().EquipWeapon(defaultWeapon);
        GetComponent<PhotonView>().RPC("CheckChildren", RpcTarget.AllBuffered);
    }
   
}
