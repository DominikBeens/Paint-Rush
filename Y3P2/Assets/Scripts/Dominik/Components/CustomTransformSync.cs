﻿using Photon.Pun;
using UnityEngine;

public class CustomTransformSync : MonoBehaviourPunCallbacks, IPunObservable
{

    private Quaternion rotToSync;
    private Quaternion syncedRot;

    [SerializeField] private float smoothSpeed = 1f;

    private void LateUpdate()
    {
        if (photonView.IsMine)
        {
            rotToSync = transform.localRotation;
        }
        else
        {
            if (GameManager.CurrentGameSate == GameManager.GameState.Playing && TimeManager.CurrentGameTimeState == TimeManager.GameTimeState.InProgress)
            {
                transform.localRotation = syncedRot;
            }
            else
            {
                Vector3 defaultRot = syncedRot.eulerAngles;
                defaultRot.x = PlayerController.defaultAngleX;
                defaultRot.z = PlayerController.defaultAngleZ;
                transform.localEulerAngles = defaultRot;
            }
        }
    }

    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        if (stream.IsWriting)
        {
            if (photonView.IsMine)
            {
                stream.SendNext(rotToSync);
            }
        }
        else
        {
            if (!photonView.IsMine)
            {
                syncedRot = (Quaternion)stream.ReceiveNext();
            }
        }
    }
}
