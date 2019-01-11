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
        terminal = FindObjectOfType<CustomizationTerminal>();
        getMat = model.GetComponent<GetDefaultMat>();
    }
	
	// Update is called once per frame
	void Update () {
		
	}

    public void SyncThisPlayerSkin(int i)
    {
        photonView.RPC("SyncSkin", RpcTarget.All, i);

    }


    [PunRPC]
    private void SyncSkin(int i)
    {
        skinIndex = i;
        model.material = terminal.Skins[skinIndex];
        getMat.UpdateMaterial(terminal.Skins[skinIndex]);
      
    }

    [PunRPC]
    private void SyncMat()
    {
        PlayerManager.instance.playerSkinSync.model.material = terminal.Skins[skinIndex]; //Every client changes its own skin to what it should be, but other clients dont see this
        getMat.UpdateMaterial(terminal.Skins[skinIndex]);
        NotificationManager.instance.NewLocalNotification("did RPC");
    }

    public void SetModelMat(Material m)
    {
        model.material = m;
    }

    private void CallMatSync()
    {
        NotificationManager.instance.NewLocalNotification("called sync");
        photonView.RPC("SyncMat", RpcTarget.AllViaServer);
    }


    public override void OnPlayerEnteredRoom(Player newPlayer)
    {
       // photonView.RPC("SyncThisPlayerSkinOthers", RpcTarget.All);
    }

    [PunRPC]
    public void SyncThisPlayerSkinOthers()
    {
        PlayerManager.instance.playerSkinSync.CallMatSync();
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
