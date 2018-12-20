using System;
using System.Collections.Generic;
using UnityEngine;

public class Projectile : MonoBehaviour
{

    private Rigidbody rb;
    private Transform owner;
    private List<Material> projectileMats = new List<Material>();
    private Vector3 defaultSize;

    [SerializeField] private string myPoolName;
    [SerializeField] private float selfDestroyTime = 5f;
    [SerializeField] private bool dealDamageOnHit = true;

    public enum MoveType { Continuous, Impact };
    [SerializeField] private MoveType moveType;

    [Header("Visual")]
    [SerializeField] private GameObject projectileModel;
    [SerializeField] private bool randomizeSize;
    [SerializeField] private float randomizeAmount;
    //[SerializeField] private TrailRenderer trailRenderer;

    [Space(10)]

    [SerializeField] private string impactObjectToSpawn;

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
        Renderer[] renderers = GetComponentsInChildren<Renderer>();
        for (int i = 0; i < renderers.Length; i++)
        {
            projectileMats.Add(renderers[i].material);
        }

        defaultSize = projectileModel ? projectileModel.transform.localScale : Vector3.zero;
    }

    private void OnEnable()
    {
        Invoke("ReturnToPool", selfDestroyTime);
    }

    private void FixedUpdate()
    {
        if (moveType == MoveType.Continuous)
        {
            rb.velocity = transform.forward * fireData.speed;
        }
    }

    public void Fire(FireData fireData)
    {
        this.fireData = fireData;
        OnFire(this);
        //SetColors();

        if (moveType == MoveType.Impact)
        {
            rb.AddForce(transform.forward * fireData.speed, ForceMode.Impulse);
        }

        if (randomizeSize)
        {
            RandomizeSize();
        }
    }

    private void SetColors()
    {
        Color color = PlayerManager.instance.entity.paintController.GetPaintColor((PaintController.PaintType)fireData.paintType);

        for (int i = 0; i < projectileMats.Count; i++)
        {
            projectileMats[i].color = color;
        }

        //trailRenderer.startColor = color;
        //trailRenderer.endColor = color;
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
            // If we hit a player.
            PlayerManager player = other.transform.root.GetComponent<PlayerManager>();
            if (player)
            {
                // Check if that player isnt the same player that fired the projectile because we dont want to hit ourselves.
                if (player.photonView.ViewID != fireData.ownerID)
                {
                    HandleHitEntity(entity);
                    return;
                }
                // If it is then ignore him.
                else
                {
                    return;
                }
            }
            // If we didnt hit a player but some other entity, hit it.
            else
            {
                HandleHitEntity(entity);
                return;
            }
        }

        HandleHitEnvironment();
    }

    private void HandleHitEntity(Entity entity)
    {
        // Projectile needs to be mine and do more than 0 damage to trigger the Hit();
        if (fireData.ownerID == PlayerManager.instance.photonView.ViewID && dealDamageOnHit)
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
        if (!string.IsNullOrEmpty(impactObjectToSpawn))
        {
            GameObject newSpawn = ObjectPooler.instance.GrabFromPool(impactObjectToSpawn, transform.position, transform.rotation);
            ForceExplosion forceExplosion = newSpawn.GetComponent<ForceExplosion>();
            if (forceExplosion)
            {
                forceExplosion.Init(fireData);
            }

            //PaintImpactParticle pip = newSpawn.GetComponent<PaintImpactParticle>();
            //if (pip)
            //{
            //    pip.Initialise(PlayerManager.instance.entity.paintController.GetPaintColor((PaintController.PaintType)fireData.paintType));
            //}
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
        CancelInvoke();
    }

    private void OnDisable()
    {
        ResetProjectile();
    }
}