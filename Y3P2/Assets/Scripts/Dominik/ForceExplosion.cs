using UnityEngine;

public class ForceExplosion : MonoBehaviour 
{

    private CollisionEventZone collisionZone;
    private Collider explosionCollider;
    private bool hitPlayer;

    [SerializeField] private string myPoolName;
    [SerializeField] private float explosionForce = 50f;
    [SerializeField] private float explosionUpForce = 10f;

    private void Awake()
    {
        explosionCollider = GetComponent<Collider>();

        collisionZone = GetComponent<CollisionEventZone>();
        collisionZone.OnZoneEnter.AddListener(() => AddForce(collisionZone.EventCaller));
    }

    private void OnEnable()
    {
        hitPlayer = false;
        explosionCollider.enabled = true;

        Invoke("DisableCollider", 0.1f);
        Invoke("ReturnToPool", 2);
    }

    private void AddForce(Transform player)
    {
        if (hitPlayer)
        {
            return;
        }

        if (player.root == PlayerManager.instance.transform)
        {
            Rigidbody rb = player.root.GetComponent<Rigidbody>();
            if (rb)
            {
                Vector3 fixedCurrentPosition = new Vector3(transform.position.x, rb.transform.position.y, transform.position.z);
                rb.AddForce((rb.transform.position - fixedCurrentPosition) * explosionForce, ForceMode.Impulse);
                rb.AddForce(Vector3.up * explosionUpForce, ForceMode.Impulse);

                hitPlayer = true;
            }
        }
    }

    private void DisableCollider()
    {
        explosionCollider.enabled = false;
    }

    private void ReturnToPool()
    {
        ObjectPooler.instance.AddToPool(myPoolName, gameObject);
    }
}
