using Photon.Pun;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class Entity : MonoBehaviourPunCallbacks
{

    private PlayerManager myPlayerManager;

    //[SerializeField] private int entityID;
    public enum EntityType { Humanoid, Prop, TestDummy };
    [SerializeField] private EntityType entityType;

    [Space(10)]

    public PaintController paintController;

    [Space(10)]

    public UnityEvent OnHit;
    public UnityEvent OnDeath;

    private void Start()
    {
        myPlayerManager = transform.root.GetComponent<PlayerManager>();
        paintController.Initialise(this);

        if (photonView && !photonView.IsMine)
        {
            photonView.RPC("SendUpdates", RpcTarget.Others);
        }
    }

    public void Hit(int paintColor, float amount)
    {
        if (myPlayerManager && myPlayerManager.playerPickupManager.Shielded)
        {
            return;
        }

        if (GameManager.CurrentGameSate == GameManager.GameState.Playing && TimeManager.CurrentGameTimeState == TimeManager.GameTimeState.InProgress)
        {
            photonView.RPC("HitRPC", RpcTarget.All, paintColor, amount, PlayerManager.instance.photonView.ViewID);
        }
        else
        {
            if (entityType == EntityType.TestDummy)
            {
                photonView.RPC("HitRPC", RpcTarget.All, paintColor, amount, PlayerManager.instance.photonView.ViewID);
            }
        }
    }

    public void HitAll(float amount, int overrideAttackerID = -1)
    {
        if (myPlayerManager && myPlayerManager.playerPickupManager.Shielded)
        {
            return;
        }

        if (GameManager.CurrentGameSate == GameManager.GameState.Playing && TimeManager.CurrentGameTimeState == TimeManager.GameTimeState.InProgress)
        {
            photonView.RPC("HitAllRPC", RpcTarget.All, amount, overrideAttackerID != -1 ? overrideAttackerID : PlayerManager.instance.photonView.ViewID);
        }
        else
        {
            if (entityType == EntityType.TestDummy)
            {
                photonView.RPC("HitAllRPC", RpcTarget.All, amount, overrideAttackerID != -1 ? overrideAttackerID : PlayerManager.instance.photonView.ViewID);
            }
        }
    }

    [PunRPC]
    private void HitRPC(int paintColor, float amount, int attackerID)
    {
        OnHit.Invoke();
        paintController.ModifyPaint((PaintController.PaintType)paintColor, amount, attackerID);
    }

    [PunRPC]
    private void HitAllRPC(float amount, int attackerID)
    {
        OnHit.Invoke();
        for (int i = 0; i < paintController.PaintValues.Count; i++)
        {
            paintController.ModifyPaint(paintController.PaintValues[i].paintType, amount, attackerID);
        }
    }

    [PunRPC]
    private void SendUpdates()
    {
        if (photonView.IsMine)
        {
            photonView.RPC("ReceiveUpdates", RpcTarget.All,
                paintController.PaintValues[0].paintValue,
                paintController.PaintValues[1].paintValue,
                paintController.PaintValues[2].paintValue,
                paintController.PaintValues[3].paintValue,
                paintController.CurrentPaintMark == null ? -1 : (int)paintController.CurrentPaintMark.markType,
                paintController.CurrentPaintMark == null ? -1 : paintController.CurrentPaintMark.markValue);
        }
    }

    [PunRPC]
    private void ReceiveUpdates(float val1, float val2, float val3, float val4, int markType, float markValue)
    {
        List<float> values = new List<float> { val1, val2, val3, val4 };
        paintController.SetRawValues(values);

        if (markType != -1)
        {
            paintController.CurrentPaintMark = new PaintController.PaintMark { markType = (PaintController.PaintType)markType, markValue = markValue };
        }
    }

    [PunRPC]
    public void SyncCaptureMark(Vector3 capturePoint)
    {
        paintController.MarkCaptured();
        ObjectPooler.instance.GrabFromPool("MarkCaptureExplosion", capturePoint, Quaternion.identity);
    }

    public void DestroyEntity()
    {
        if (PhotonNetwork.IsMasterClient)
        {
            PhotonNetwork.Destroy(transform.root.gameObject);
        }
    }

    public override void OnDisable()
    {
        if (photonView.IsMine)
        {
            paintController.UnsubscribeEvents();
        }
    }
}
