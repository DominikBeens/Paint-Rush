using Photon.Pun;
using UnityEngine;
using UnityEngine.Events;

public class Entity : MonoBehaviourPunCallbacks, IPunObservable
{

    [HideInInspector] public Collider myCollider;

    [SerializeField] private int entityID;
    public enum EntityType { Humanoid, Prop};
    [SerializeField] private EntityType entityType;

    [Space(10)]

    public PaintController paintController;

    [Space(10)]

    public UnityEvent OnHit;
    public UnityEvent OnDeath;

    private void Awake()
    {
        myCollider = GetComponent<Collider>();
        paintController.Initialise(this);
    }

    public void Hit(int paintColor, float amount)
    {
        photonView.RPC("HitRPC", RpcTarget.All, paintColor, amount, PlayerManager.instance.photonView.ViewID);
    }

    [PunRPC]
    private void HitRPC(int paintColor, float amount, int attackerID)
    {
        OnHit.Invoke();
        paintController.AddPaint((PaintController.PaintType)paintColor, amount, attackerID);
    }

    public void DestroyEntity()
    {
        if (PhotonNetwork.IsMasterClient)
        {
            PhotonNetwork.Destroy(transform.root.gameObject);
        }
    }

    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        for (int i = 0; i < paintController.PaintValues.Count; i++)
        {
            if (stream.IsWriting)
            {
                stream.SendNext(paintController.PaintValues[i].paintValue);
                stream.SendNext((int)paintController.CurrentPaintState);
            }
            else
            {
                if (!photonView.IsMine)
                {
                    paintController.PaintValues[i].paintValue = (float)stream.ReceiveNext();
                    paintController.SetPaintState((PaintController.PaintState)(int)stream.ReceiveNext(), paintController.LastHitPaintType);
                }
            }
        }
    }
}
