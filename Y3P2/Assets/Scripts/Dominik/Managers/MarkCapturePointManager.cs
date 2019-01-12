using Photon.Pun;
using System;
using UnityEngine;

public class MarkCapturePointManager : MonoBehaviourPunCallbacks, IPunObservable
{

    public static MarkCapturePointManager instance;

    private MarkCapturePoint[] capturePoints;
    private int activePointID = -1;
    private int syncedActivePointID = -1;
    private bool readyToReceiveUpdates;

    public static event Action<MarkCapturePoint> OnCapturePointChanged = delegate { };

    private void Awake()
    {
        if (!instance)
        {
            instance = this;
        }
        else if (instance && instance != this)
        {
            Destroy(this);
        }
    }

    private void Start()
    {
        SetupStartingCapturePoint();
    }

    private void Update()
    {
        if (!PhotonNetwork.IsMasterClient && activePointID != syncedActivePointID && readyToReceiveUpdates)
        {
            activePointID = syncedActivePointID;
            OnCapturePointChanged(GetCapturePointFromID(activePointID));
        }
    }

    private void SetupStartingCapturePoint()
    {
        capturePoints = FindObjectsOfType<MarkCapturePoint>();
        for (int i = 0; i < capturePoints.Length; i++)
        {
            capturePoints[i].Init();
        }

        readyToReceiveUpdates = true;

        if (PhotonNetwork.IsMasterClient)
        {
            MarkCapturePoint point = capturePoints[UnityEngine.Random.Range(0, capturePoints.Length)];
            activePointID = point.CapturePointID;
            OnCapturePointChanged(GetCapturePointFromID(activePointID));
        }
    }

    public void PointCaptured()
    {
        photonView.RPC("ShuffleCapturePoint", RpcTarget.All);
    }

    [PunRPC]
    private void ShuffleCapturePoint()
    {
        if (PhotonNetwork.IsMasterClient)
        {
            int newActivePointID = activePointID;
            if (capturePoints.Length > 1)
            {
                while (newActivePointID == activePointID)
                {
                    MarkCapturePoint point = capturePoints[UnityEngine.Random.Range(0, capturePoints.Length)];
                    newActivePointID = point.CapturePointID;
                }
            }

            activePointID = newActivePointID;
            OnCapturePointChanged(GetCapturePointFromID(activePointID));
        }
    }

    private MarkCapturePoint GetCapturePointFromID(int ID)
    {
        for (int i = 0; i < capturePoints.Length; i++)
        {
            if (capturePoints[i].CapturePointID == ID)
            {
                return capturePoints[i];
            }
        }

        return null;
    }

    // Sync active capture point by syncing it's index in the capturePoints array.
    // The index in the array is the same on all clients if neither of them change the game's hierarchy before Awake() gets called.
    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        if (stream.IsWriting)
        {
            if (PhotonNetwork.IsMasterClient)
            {
                stream.SendNext(activePointID);
            }
        }
        else
        {
            if (!PhotonNetwork.IsMasterClient)
            {
                syncedActivePointID = (int)stream.ReceiveNext();
            }
        }
    }
}
