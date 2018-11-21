using Photon.Pun;
using UnityEngine;
using UnityEngine.Events;

public class Entity : MonoBehaviourPunCallbacks
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
        paintController.Initialise();
    }

    public void Hit(int paintColor, float amount)
    {
        photonView.RPC("HitRPC", RpcTarget.All, paintColor, amount);
    }

    [PunRPC]
    private void HitRPC(int paintColor, float amount)
    {
        OnHit.Invoke();
        paintController.AddPaint((PaintController.PaintType)paintColor, amount);
    }

    public void DestroyEntity()
    {
        if (PhotonNetwork.IsMasterClient)
        {
            PhotonNetwork.Destroy(transform.root.gameObject);
        }
    }
}
