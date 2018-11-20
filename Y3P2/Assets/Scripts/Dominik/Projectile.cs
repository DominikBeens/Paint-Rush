using Photon.Pun;
using System;
using UnityEngine;

public class Projectile : MonoBehaviour
{

    private Rigidbody rb;
    protected bool hitAnything;
    private Transform owner;
    private Collider hitCollider;

    [SerializeField] private string myPoolName;
    [SerializeField] private float selfDestroyTime = 5f;
    [SerializeField] private string prefabToSpawnOnHit;
    [SerializeField] private string prefabToSpawnOnDeath;

    public event Action<Projectile> OnFire = delegate { };
    public event Action<Projectile> OnEntityHit = delegate { };
    public event Action<Projectile> OnEnvironmentHit = delegate { };

    public struct FireData
    {
        public float speed;
        public int ownerID;
    }
    public FireData fireData;

    public virtual void Awake()
    {
        rb = GetComponent<Rigidbody>();
    }

    public virtual void OnEnable()
    {
        Invoke("ReturnToPool", selfDestroyTime);
    }

    public virtual void FixedUpdate()
    {
        rb.velocity = transform.forward * fireData.speed;
    }

    public virtual void Fire(FireData fireData)
    {
        this.fireData = fireData;
        OnFire(this);
    }

    private void OnTriggerEnter(Collider other)
    {
        Entity entity = other.GetComponent<Entity>();
        if (entity)
        {
            hitCollider = entity.myCollider;
            HandleHitEntity(entity);
            return;
        }

        if (other.tag == "Environment")
        {
            hitCollider = other;
            HandleHitEnvironment();
            return;
        }
    }

    public virtual void HandleHitEntity(Entity entity)
    {
        if (fireData.ownerID == PlayerManager.instance.photonView.ViewID)
        {
            entity.Hit();
        }

        OnEntityHit(this);
        SpawnPrefabOnHit();

        hitAnything = true;
        ReturnToPool();
    }

    public virtual void HandleHitEnvironment()
    {
        SpawnPrefabOnHit();
        OnEnvironmentHit(this);

        hitAnything = true;
        ReturnToPool();
    }

    private void SpawnPrefabOnHit()
    {
        if (!string.IsNullOrEmpty(prefabToSpawnOnHit))
        {
            GameObject newSpawn = ObjectPooler.instance.GrabFromPool(prefabToSpawnOnHit, hitCollider.ClosestPoint(transform.position), Quaternion.identity);
        }
    }

    protected void ReturnToPool()
    {
        if (!string.IsNullOrEmpty(myPoolName))
        {
            ObjectPooler.instance.AddToPool(myPoolName, gameObject);
        }
        else
        {
            gameObject.SetActive(false);
        }
    }

    private void ResetProjectile()
    {
        rb.velocity = Vector3.zero;
        hitCollider = null;
        hitAnything = false;
        CancelInvoke();
    }

    public virtual void OnDisable()
    {
        ResetProjectile();

        if (!string.IsNullOrEmpty(prefabToSpawnOnDeath))
        {
            GameObject newSpawn = ObjectPooler.instance.GrabFromPool(prefabToSpawnOnDeath, transform.position, Quaternion.identity);
        }
    }
}