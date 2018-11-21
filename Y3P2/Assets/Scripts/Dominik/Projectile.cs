using System;
using UnityEngine;

public class Projectile : MonoBehaviour
{

    private Rigidbody rb;
    private Transform owner;
    private Collider hitCollider;
    private Material projectileMat;
    private Vector3 defaultSize;

    [SerializeField] private string myPoolName;
    [SerializeField] private float selfDestroyTime = 5f;
    [SerializeField] private string prefabToSpawnOnHit;
    [SerializeField] private string prefabToSpawnOnDeath;

    [Header("Visual")]
    [SerializeField] private GameObject projectileModel;
    [SerializeField] private bool randomizeSize;
    [SerializeField] private float randomizeAmount;

    public event Action<Projectile> OnFire = delegate { };
    public event Action<Projectile> OnEntityHit = delegate { };
    public event Action<Projectile> OnEnvironmentHit = delegate { };

    public struct FireData
    {
        public float speed;
        public int ownerID;
        public int paintType;
        public float paintAmount;
    }
    public FireData fireData;

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
        projectileMat = GetComponentInChildren<Renderer>().material;
        defaultSize = projectileModel ? projectileModel.transform.localScale : Vector3.zero;
    }

    private void OnEnable()
    {
        Invoke("ReturnToPool", selfDestroyTime);
    }

    private void FixedUpdate()
    {
        rb.velocity = transform.forward * fireData.speed;
    }

    public void Fire(FireData fireData)
    {
        this.fireData = fireData;
        OnFire(this);
        projectileMat.color = PlayerManager.instance.entity.paintController.GetPaintColor((PaintController.PaintType)fireData.paintType);

        if (randomizeSize)
        {
            RandomizeSize();
        }
    }

    private void RandomizeSize()
    {
        float randomizedScale = defaultSize.x + UnityEngine.Random.Range(-randomizeAmount, randomizeAmount);
        projectileModel.transform.localScale = new Vector3(randomizedScale, randomizedScale, randomizedScale);
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

        //if (other.tag == "Environment")
        //{
            hitCollider = other;
            HandleHitEnvironment();
            return;
        //}
    }

    private void HandleHitEntity(Entity entity)
    {
        if (fireData.ownerID == PlayerManager.instance.photonView.ViewID)
        {
            entity.Hit(fireData.paintType, fireData.paintAmount);
        }

        OnEntityHit(this);
        SpawnPrefabOnHit();

        ReturnToPool();
    }

    private void HandleHitEnvironment()
    {
        SpawnPrefabOnHit();
        OnEnvironmentHit(this);

        ReturnToPool();
    }

    private void SpawnPrefabOnHit()
    {
        if (!string.IsNullOrEmpty(prefabToSpawnOnHit))
        {
            GameObject newSpawn = ObjectPooler.instance.GrabFromPool(prefabToSpawnOnHit, hitCollider.ClosestPoint(transform.position), transform.rotation);
        }
    }

    private void ReturnToPool()
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
        CancelInvoke();
    }

    private void OnDisable()
    {
        ResetProjectile();

        if (!string.IsNullOrEmpty(prefabToSpawnOnDeath))
        {
            GameObject newSpawn = ObjectPooler.instance.GrabFromPool(prefabToSpawnOnDeath, transform.position, Quaternion.identity);
        }
    }
}