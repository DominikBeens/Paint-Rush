using Photon.Pun;
using UnityEngine;

public class WeaponPrefab : MonoBehaviourPunCallbacks
{

    private Vector3 screenMiddle;
    private Camera mainCam;
    private bool initialisedEvents;
    private PaintImpactParticle paintMuzzleFlashParticle;
    private PaintUILocalPlayer[] paintDisplayBars;

    [SerializeField] private LayerMask hitLayerMask;
    [SerializeField] private Transform projectileSpawn;
    [SerializeField] private ParticleSystem muzzleFlashParticle;

    private void Awake()
    {
        if (photonView.IsMine)
        {
            mainCam = Camera.main;

            WeaponSlot.OnFireWeapon += WeaponSlot_OnFireWeapon;
            WeaponSlot.OnEquipWeapon += WeaponSlot_OnEquipWeapon;
            initialisedEvents = true;
        }
        else
        {
            SetLayer(transform, 10);
        }

        paintMuzzleFlashParticle = GetComponentInChildren<PaintImpactParticle>();
    }

    // Init paint UI on gun panel.
    //private void Start()
    //{
    //    if (photonView.IsMine)
    //    {
    //        paintDisplayBars = GetComponentsInChildren<PaintUILocalPlayer>();
    //        for (int i = 0; i < PlayerManager.instance.entity.paintController.PaintValues.Count; i++)
    //        {
    //            if (paintDisplayBars.Length > 0 && paintDisplayBars[i])
    //            {
    //                paintDisplayBars[i].Initialise(PlayerManager.instance.entity.paintController.PaintValues[i]);
    //            }
    //        }
    //    }
    //}

    public override void OnEnable()
    {
        if (!initialisedEvents && photonView.IsMine)
        {
            WeaponSlot.OnFireWeapon += WeaponSlot_OnFireWeapon;
            WeaponSlot.OnEquipWeapon += WeaponSlot_OnEquipWeapon;
            initialisedEvents = true;
        }
    }

    private void WeaponSlot_OnFireWeapon()
    {
        if (WeaponSlot.currentWeapon is Weapon_HitScan)
        {
            Fire_HitScan();
        }
        else if (WeaponSlot.currentWeapon is Weapon_Projectile)
        {
            Fire_Projectile();
        }

        if (GameManager.CurrentGameSate == GameManager.GameState.Playing)
        {
            SaveManager.instance.SaveStat(SaveManager.SavedStat.ShotsFired);
        }
    }

    private void Fire_HitScan()
    {
        screenMiddle = new Vector3(Screen.width / 2f, Screen.height / 2f, 0);

        bool paintImpact = false;
        bool paintDecal = false;
        Vector3 hitPoint = Vector3.zero;
        Quaternion paintDecalRot = Quaternion.identity;
        Quaternion paintImpactRot = Quaternion.identity;

        RaycastHit hitFromCam;
        if (Physics.Raycast(mainCam.ScreenPointToRay(screenMiddle), out hitFromCam, 5000, hitLayerMask))
        {
            RaycastHit hitFromWeapon;
            Ray ray = new Ray(projectileSpawn.position, (hitFromCam.point - projectileSpawn.position));
            if (Physics.Raycast(ray, out hitFromWeapon, 5000, hitLayerMask))
            {
                Entity hitEntity = hitFromWeapon.transform.GetComponentInChildren<Entity>();
                if (hitEntity)
                {
                    if (GameManager.CurrentGameSate == GameManager.GameState.Playing)
                    {
                        hitEntity.Hit((int)WeaponSlot.currentPaintType, WeaponSlot.currentWeapon.paintDamage);
                        SaveManager.instance.SaveStat(SaveManager.SavedStat.ShotsHit);

                        if (WeaponSlot.currentWeapon.spawnDecalOnImpact)
                        {
                            //paintDecal = true;
                            hitPoint = hitFromWeapon.point;
                            paintDecalRot = Quaternion.LookRotation(-hitFromWeapon.normal);
                        }
                    }

                    PlayerManager.instance.weaponSlot.HitEntity();
                }


                if (WeaponSlot.currentWeapon.spawnParticleOnImpact)
                {
                    paintImpact = true;
                    hitPoint = hitFromWeapon.point;
                    paintImpactRot = Quaternion.LookRotation(ray.direction);
                }
            }
        }

        photonView.RPC("SpawnEffects", RpcTarget.All, paintImpact, paintDecal, hitPoint, paintDecalRot, paintImpactRot, (int)WeaponSlot.currentPaintType);
    }

    private void Fire_Projectile()
    {
        Weapon_Projectile weapon = WeaponSlot.currentWeapon as Weapon_Projectile;

        ProjectileManager.ProjectileData data = new ProjectileManager.ProjectileData
        {
            spawnPosition = projectileSpawn.position,
            //spawnRotation = fireDirection != Vector3.zero ? Quaternion.LookRotation(fireDirection, Vector3.forward) : projectileSpawn.rotation,
            spawnRotation = projectileSpawn.rotation,
            projectilePool = weapon.projectilePoolName,
            speed = weapon.projectileSpeed,
            paintType = (int)WeaponSlot.currentPaintType,
            paintAmount = weapon.paintDamage,
            projectileOwnerID = PlayerManager.instance.photonView.ViewID,
        };
        ProjectileManager.instance.FireProjectile(data);

        //photonView.RPC("SpawnEffects", RpcTarget.All, paintImpact, paintDecal, hitPoint, paintDecalRot, paintImpactRot, (int)WeaponSlot.currentPaintType);
        photonView.RPC("SpawnEffects", RpcTarget.All, false, false, Vector3.zero, Quaternion.identity, Quaternion.identity, (int)WeaponSlot.currentPaintType);
    }

    private void WeaponSlot_OnEquipWeapon(Weapon weapon)
    {

    }

    [PunRPC]
    private void SpawnEffects(bool paintImpactParticle, bool paintDecal, Vector3 hitPosition, Quaternion decalRot, Quaternion paintImpactRot, int paintType)
    {
        paintMuzzleFlashParticle.Initialise(PlayerManager.instance.entity.paintController.GetPaintColor((PaintController.PaintType)paintType));
        muzzleFlashParticle.Play();

        AudioController audioController = ObjectPooler.instance.GrabFromPool("Audio_GunFire", transform.position, Quaternion.identity).GetComponent<AudioController>();
        audioController.Play(transform);

        if (paintImpactParticle)
        {
            GameObject newSpawn = ObjectPooler.instance.GrabFromPool("PaintImpact", hitPosition, paintImpactRot);
            PaintImpactParticle pip = newSpawn.GetComponent<PaintImpactParticle>();
            if (pip)
            {
                pip.Initialise(PlayerManager.instance.entity.paintController.GetPaintColor((PaintController.PaintType)paintType));
            }
        }

        if (paintDecal)
        {
            GameObject newSpawn = ObjectPooler.instance.GrabFromPool("PaintDecal", hitPosition, decalRot);
            PaintDecal decal = newSpawn.GetComponent<PaintDecal>();
            if (decal)
            {
                decal.Initialise(PlayerManager.instance.entity.paintController.GetPaintColor((PaintController.PaintType)paintType));
            }
        }
    }

    private void SetLayer(Transform root, int layer)
    {
        root.gameObject.layer = layer;
        foreach (Transform child in root)
        {
            SetLayer(child, layer);
        }
    }

    public override void OnDisable()
    {
        if (photonView.IsMine)
        {
            WeaponSlot.OnFireWeapon -= WeaponSlot_OnFireWeapon;
            WeaponSlot.OnEquipWeapon -= WeaponSlot_OnEquipWeapon;
            initialisedEvents = false;
        }
    }
}
