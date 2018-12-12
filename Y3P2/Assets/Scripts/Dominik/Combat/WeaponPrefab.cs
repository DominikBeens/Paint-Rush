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

    private void Start()
    {
        if (photonView.IsMine)
        {
            paintDisplayBars = GetComponentsInChildren<PaintUILocalPlayer>();
            for (int i = 0; i < PlayerManager.instance.entity.paintController.PaintValues.Count; i++)
            {
                if (paintDisplayBars[i])
                {
                    paintDisplayBars[i].Initialise(PlayerManager.instance.entity.paintController.PaintValues[i]);
                }
            }
        }
    }

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

                        paintDecal = true;
                        hitPoint = hitFromWeapon.point;
                        paintDecalRot = Quaternion.LookRotation(-hitFromWeapon.normal);
                    }

                    PlayerManager.instance.weaponSlot.HitEntity();
                }


                if (!string.IsNullOrEmpty(WeaponSlot.currentWeapon.paintImpactPoolName))
                {
                    paintImpact = true;
                    hitPoint = hitFromWeapon.point;
                    paintImpactRot = Quaternion.LookRotation(ray.direction);
                }

                if (GameManager.CurrentGameSate == GameManager.GameState.Playing)
                {
                    SaveManager.instance.SaveStat(SaveManager.SavedStat.ShotsFired);
                }
            }
        }

        photonView.RPC("SpawnEffects", RpcTarget.All, paintImpact, paintDecal, hitPoint, paintDecalRot, paintImpactRot, (int)WeaponSlot.currentPaintType);

        //Weapon weapon = WeaponSlot.currentWeapon;

        //ProjectileManager.ProjectileData data = new ProjectileManager.ProjectileData
        //{
        //    spawnPosition = projectileSpawn.position,
        //    spawnRotation = fireDirection != Vector3.zero ? Quaternion.LookRotation(fireDirection, Vector3.forward) : projectileSpawn.rotation,
        //    //spawnRotation = projectileSpawn.rotation,
        //    projectilePool = weapon.bulletPoolName,
        //    speed = weapon.bulletSpeed,
        //    paintType = (int)weapon.paintType,
        //    paintAmount = weapon.paintDamage,
        //    projectileOwnerID = PlayerManager.instance.photonView.ViewID,
        //};
        //ProjectileManager.instance.FireProjectile(data);
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
