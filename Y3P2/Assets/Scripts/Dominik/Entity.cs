using Photon.Pun;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class Entity : MonoBehaviourPunCallbacks, IPunObservable
{

    private Rigidbody rb;
    private PlayerManager myPlayerManager;
    private List<float> syncedPaintValues = new List<float>();

    //[SerializeField] private int entityID;
    public enum EntityType { Humanoid, Prop, TestDummy };
    [SerializeField] private EntityType entityType;

    [Space(10)]

    public PaintController paintController;

    [Space(10)]

    public UnityEvent OnHit;
    public UnityEvent OnDeath;

    private void Awake()
    {
        rb = GetComponentInChildren<Rigidbody>();
        myPlayerManager = transform.root.GetComponent<PlayerManager>();
        paintController.Initialise(this);

        //if (photonView && !photonView.IsMine)
        //{
        //    photonView.RPC("SendUpdates", RpcTarget.Others);
        //}
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
    private void PaintFilled(int paintType, int attackerID)
    {
        paintController.PaintFilled(paintType, attackerID);
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
        paintController.SyncPaintValues(values);

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

    [PunRPC]
    private void KnockBack(Vector3 direction, float force)
    {
        if (rb)
        {
            rb.AddForce(direction * force, ForceMode.Impulse);
            rb.AddForce(Vector3.up * (0.5f * force), ForceMode.Impulse);
        }
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

    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        if (stream.IsWriting)
        {
            if (photonView.IsMine)
            {
                stream.SendNext(paintController.CurrentPaintMark != null);

                if (paintController.CurrentPaintMark != null)
                {
                    stream.SendNext((int)paintController.CurrentPaintMark.markType);
                    stream.SendNext(paintController.CurrentPaintMark.markValue);
                }

                for (int i = 0; i < paintController.PaintValues.Count; i++)
                {
                    stream.SendNext(paintController.PaintValues[i].paintValue);
                }
            }
        }
        else
        {
            if (!photonView.IsMine)
            {
                bool hasMark = (bool)stream.ReceiveNext();
                if (hasMark)
                {
                    int syncedMarkType = (int)stream.ReceiveNext();
                    float syncedMarkValue = (float)stream.ReceiveNext();

                    if (paintController.CurrentPaintMark == null)
                    {
                        paintController.CurrentPaintMark = new PaintController.PaintMark { markType = (PaintController.PaintType)syncedMarkType, markValue = syncedMarkValue };
                    }
                    else
                    {
                        paintController.SyncMarkValue(syncedMarkValue);
                    }
                }
                else
                {
                    if (paintController.CurrentPaintMark != null)
                    {
                        paintController.MarkDestroyed();
                    }
                }

                for (int i = 0; i < paintController.PaintValues.Count; i++)
                {
                    syncedPaintValues.Insert(i, (float)stream.ReceiveNext());
                }
                paintController.SyncPaintValues(syncedPaintValues);
            }
        }
    }
}
