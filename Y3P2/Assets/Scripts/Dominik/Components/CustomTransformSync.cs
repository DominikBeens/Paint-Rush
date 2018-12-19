using Photon.Pun;
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
            transform.localRotation = syncedRot;
            //transform.localRotation = Quaternion.Slerp(transform.localRotation, syncedRot, Time.deltaTime * smoothSpeed);
            //Debug.LogWarning("Current: " + transform.localRotation.eulerAngles + " Synced: " + syncedRot.eulerAngles);
            //transform.localRotation = Quaternion.RotateTowards(transform.localRotation, syncedRot, Time.deltaTime * smoothSpeed);
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
