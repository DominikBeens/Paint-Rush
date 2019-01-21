using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;

public class SyncPlayerSkin : MonoBehaviourPunCallbacks, IPunObservable {

    [SerializeField]
    private Renderer model;
    private GetDefaultMat getMat;
    private int skinIndex;

    private CustomizationTerminal terminal;
    private PlayerPickUpManager pckm;

    private bool isPepe;
    private bool shield;
    private bool cloak;
    // Use this for initialization
    void Start () {
        Components();
    }
	private void Components()
    {
        terminal = FindObjectOfType<CustomizationTerminal>();
        pckm = GetComponent<PlayerPickUpManager>();
        getMat = model.GetComponent<GetDefaultMat>();
    }

	// Update is called once per frame
	void Update () {
		
	}

    public void SyncThisPlayerSkin(int i)
    {
        skinIndex = i;
        SyncSkin();
        // StartCoroutine(Wait(i));
    }

    //private IEnumerator Wait(int i)
    //{
    //    yield return new WaitForSeconds(.1F);
    //}


    private void SyncSkin()
    {
        isPepe = false;
        model.material = terminal.Skins[skinIndex]; 
        getMat.UpdateMaterial(terminal.Skins[skinIndex]);
      
    }



  

    public override void OnPlayerEnteredRoom(Player newPlayer)
    {
        Components();
    }


    public void ToggleShield(bool toggle)
    {
        shield = toggle;
    }

    public void ToggleCloak(bool toggle)
    {
        cloak = toggle;
    }



    public void Pepe()
    {
        terminal.PreviewCharRenderer.material = terminal.SecretSkin;
        model.material = terminal.SecretSkin;
        getMat.UpdateMaterial(terminal.SecretSkin);
        isPepe = true;
    }


    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        if (stream.IsWriting)
        {
            if (PhotonNetwork.LocalPlayer.IsLocal )
            {
                stream.SendNext(isPepe);
                stream.SendNext(shield);
                stream.SendNext(cloak);
                if (!isPepe)
                {
                    stream.SendNext(skinIndex);
                    model.material = terminal.Skins[skinIndex];
                    getMat.UpdateMaterial(terminal.Skins[skinIndex]);
                }
              
                else
                {
                    model.material = terminal.SecretSkin;
                    getMat.UpdateMaterial(terminal.SecretSkin);
                }
                if (cloak)
                {
                    model.material = pckm.CloakShader;
                }
                if (shield)
                {
                    model.material = pckm.ForceFieldShader;
                }
            }
        }
        else
        {
            if (PhotonNetwork.LocalPlayer.IsLocal)
            {
                isPepe = (bool)stream.ReceiveNext();
                shield = (bool)stream.ReceiveNext();
                cloak = (bool)stream.ReceiveNext();
                if (!isPepe)
                {
                    skinIndex = (int)stream.ReceiveNext();
                }
                if (!isPepe)
                {
                    if (model.material != terminal.Skins[skinIndex])
                    {
                        model.material = terminal.Skins[skinIndex];
                        getMat.UpdateMaterial(terminal.Skins[skinIndex]);
                    }
                  
                }
                else if (isPepe)
                {
                    model.material = terminal.SecretSkin;
                    getMat.UpdateMaterial(terminal.SecretSkin);
                }
                if (cloak)
                {
                    model.material = pckm.CloakShader;
                }
                if (shield)
                {
                    model.material = pckm.ForceFieldShader;
                }
            }
        }
    }
}
