using UnityEngine;

public class Knockbackable : MonoBehaviour
{

    private Entity entity;

    [SerializeField] private float knockBackForce = 5f;

    private void Awake()
    {
        entity = transform.root.GetComponentInChildren<Entity>();
        entity.OnHit.AddListener(() => KnockBack());
    }

    private void KnockBack()
    {
        Vector3 toPlayer = transform.position - PlayerManager.localPlayer.position;
        entity.photonView.RPC("KnockBack", Photon.Pun.RpcTarget.All, toPlayer, knockBackForce);
    }

    private void OnDisable()
    {
        entity.OnHit.RemoveListener(() => KnockBack());
    }
}
