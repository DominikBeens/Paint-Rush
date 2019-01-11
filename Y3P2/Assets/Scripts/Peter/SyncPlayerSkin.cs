using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;

public class SyncPlayerSkin : MonoBehaviourPunCallbacks {

    [SerializeField]
    private Renderer model;

    private CustomizationTerminal terminal;
    // Use this for initialization
    void Start () {
        terminal = FindObjectOfType<CustomizationTerminal>();
    }
	
	// Update is called once per frame
	void Update () {
		
	}

    public void SyncThisPlayerSkin(int i)
    {
        photonView.RPC("SyncSkin", RpcTarget.AllBuffered, i);
    }

    //public void SyncSkinForOthers(int i)
    //{
        //Function does get called when player joins
        //Skin don't sync tho
        //  NotificationManager.instance.NewLocalNotification("EWGIOBWIGOBEOSIDBIOGWBSOIGIBWODSJKGEBVS");
       // photonView.RPC("SyncSkin", RpcTarget.Others, i);
    //}

    [PunRPC]
    private void SyncSkin(int i)
    {
        model.material = terminal.Skins[i];

      
    }

    //public override void OnPlayerEnteredRoom(Player newPlayer)
    //{
    //    SyncPlayerSkin[] players = FindObjectsOfType<SyncPlayerSkin>();

    //    foreach (SyncPlayerSkin s in players)
    //    {
    //        s.SyncSkinForOthers(terminal.Skins.IndexOf(s.model.material));

    //    }
    //}
}
