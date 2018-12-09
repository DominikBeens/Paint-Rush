using Photon.Pun;
using UnityEngine;

public class CustomTransformSync : MonoBehaviourPunCallbacks, IPunObservable
{

    private Quaternion syncedRot;

    private void LateUpdate()
    {
        if (!photonView.IsMine)
        {
            transform.localRotation = syncedRot;
        }
    }

    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        if (stream.IsWriting)
        {
            if (photonView.IsMine)
            {
                stream.SendNext(transform.localRotation);
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
