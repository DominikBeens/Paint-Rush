using Photon.Pun;
using UnityEngine;
using UnityEngine.Events;

public class Entity : MonoBehaviourPunCallbacks
{

    [HideInInspector] public Collider myCollider;

    [SerializeField] private int entityID;
    [SerializeField] private bool instaDestroyOnDeath;
    [SerializeField] private Rigidbody rb;
    public enum EntityType { Humanoid, Prop};
    [SerializeField] private EntityType entityType;

    [Space(10)]

    public UnityEvent OnHit;
    public UnityEvent OnDeath;

    private void Awake()
    {
        myCollider = GetComponent<Collider>();
    }

    public void Hit()
    {
        photonView.RPC("HitRPC", RpcTarget.All);
    }

    [PunRPC]
    private void HitRPC()
    {
        OnHit.Invoke();
        // Modify color values.
    }

    public void DestroyEntity()
    {
        if (PhotonNetwork.IsMasterClient)
        {
            PhotonNetwork.Destroy(transform.root.gameObject);
        }
    }

    private void OnDestroy()
    {
        //EntityManager.instance.RemoveFromAliveTargets(this);
    }
}
