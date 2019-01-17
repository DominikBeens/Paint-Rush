using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;

public class SyncPlayerSkin : MonoBehaviourPunCallbacks {

    [SerializeField]
    private Renderer model;
    private GetDefaultMat getMat;
    private int skinIndex;

    private CustomizationTerminal terminal;
    // Use this for initialization
    void Start () {
        Components();
    }
	private void Components()
    {
        terminal = FindObjectOfType<CustomizationTerminal>();
        getMat = model.GetComponent<GetDefaultMat>();
    }

	// Update is called once per frame
	void Update () {
		
	}

    public void SyncThisPlayerSkin(int i)
    {
        PhotonNetwork.RemoveRPCs(PhotonNetwork.LocalPlayer);
        photonView.RPC("SetSkinIndex", RpcTarget.AllBuffered, i);

        photonView.RPC("SyncSkin", RpcTarget.AllBuffered);
        // StartCoroutine(Wait(i));
    }

    //private IEnumerator Wait(int i)
    //{
    //    yield return new WaitForSeconds(.1F);
    //}

    [PunRPC]
    private void SetSkinIndex(int i)
    {
        skinIndex = i;
    }

    [PunRPC]
    private void SyncSkin()
    {
        model.material = terminal.Skins[skinIndex]; //NULLREFSOMEFUCKINGHOWIDKWHYFFS
        if(model == null)
        {
            NotificationManager.instance.NewNotification("MODEL FAGGOT");
        }
        if (model.material == null)
        {
            NotificationManager.instance.NewNotification("MATERIAL FAGGOT");
        }
        if (terminal == null)
        {
            NotificationManager.instance.NewNotification("TERMINAL FAGGOT");
        }
        if (terminal.Skins == null)
        {
            NotificationManager.instance.NewNotification("SKINS FAGGOT");
        }
        if (terminal.Skins[skinIndex] == null)
        {
            NotificationManager.instance.NewNotification("SKINS[INDEX] FAGGOT");
        }
        if (skinIndex == null)
        {
            NotificationManager.instance.NewNotification("SKININDEX FAGGOT");
        }
        getMat.UpdateMaterial(terminal.Skins[skinIndex]);
      
    }



    public void SetModelMat(Material m)
    {
        model.material = m;
    }

    public override void OnPlayerEnteredRoom(Player newPlayer)
    {
        Components();
    }







    public void Pepe()
    {
        terminal.PreviewCharRenderer.material = terminal.SecretSkin;
        photonView.RPC("BecomePepe", RpcTarget.All);
    }

    [PunRPC]
    private void BecomePepe()
    {
        SetModelMat(terminal.SecretSkin);
        getMat.UpdateMaterial(terminal.SecretSkin);
    }


}
