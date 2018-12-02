using Photon.Pun;
using UnityEngine;

public class WeaponPrefab : MonoBehaviourPunCallbacks
{

    private Vector3 screenMiddle;
    private Camera mainCam;
    private bool initialisedEvents;
    private PaintImpactParticle paintMuzzleFlashParticle;

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

        RaycastHit hitFromCam;
        if (Physics.Raycast(mainCam.ScreenPointToRay(screenMiddle), out hitFromCam))
        {
            RaycastHit hitFromWeapon;
            Ray ray = new Ray(projectileSpawn.position, (hitFromCam.point - projectileSpawn.position));
            if (Physics.Raycast(ray, out hitFromWeapon))
            {
                Entity hitEntity = hitFromWeapon.transform.GetComponentInChildren<Entity>();
                if (hitEntity)
                {
                    hitEntity.Hit((int)WeaponSlot.currentPaintType, WeaponSlot.currentWeapon.paintDamage);
                    PlayerManager.instance.weaponSlot.HitEntity();
                    photonView.RPC("SpawnPrefab", RpcTarget.All, "PaintDecal", hitFromWeapon.point, Quaternion.LookRotation(-hitFromWeapon.normal), (int)WeaponSlot.currentPaintType);
                    SaveManager.instance.SaveStat(SaveManager.SavedStat.ShotsHit);
                }

                if (!string.IsNullOrEmpty(WeaponSlot.currentWeapon.paintImpactPoolName))
                {
                    // TODO: Change Quaternion.identity to face impact.
                    photonView.RPC("SpawnPrefab", RpcTarget.All, WeaponSlot.currentWeapon.paintImpactPoolName, hitFromWeapon.point, Quaternion.LookRotation(ray.direction), (int)WeaponSlot.currentPaintType);
                }

                SaveManager.instance.SaveStat(SaveManager.SavedStat.ShotsFired);
            }
        }

        photonView.RPC("PlayMuzzleFlash", RpcTarget.All, (int)WeaponSlot.currentPaintType);

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
    private void PlayMuzzleFlash(int paintType)
    {
        paintMuzzleFlashParticle.Initialise(PlayerManager.instance.entity.paintController.GetPaintColor((PaintController.PaintType)paintType));
        muzzleFlashParticle.Play();
    }

    [PunRPC]
    private void SpawnPrefab(string prefabPoolName, Vector3 position, Quaternion rotation, int paintType)
    {
        GameObject newSpawn = ObjectPooler.instance.GrabFromPool(prefabPoolName, position, rotation);
        PaintImpactParticle pip = newSpawn.GetComponent<PaintImpactParticle>();
        if (pip)
        {
            pip.Initialise(PlayerManager.instance.entity.paintController.GetPaintColor((PaintController.PaintType)paintType));
            return;
        }

        PaintDecal decal = newSpawn.GetComponent<PaintDecal>();
        if (decal)
        {
            decal.Initialise(PlayerManager.instance.entity.paintController.GetPaintColor((PaintController.PaintType)paintType));
        }

        //BulletTrail bulletTrail = ObjectPooler.instance.GrabFromPool("BulletTrail", Vector3.zero, Quaternion.identity).GetComponent<BulletTrail>();
        //bulletTrail.Initialise(projectileSpawn.position, position, PlayerManager.instance.entity.paintController.GetPaintColor(WeaponSlot.currentWeapon.paintType));
    }

    private void SetLayer(Transform root, int layer)
    {
        root.gameObject.layer = layer;
        foreach (Transform child in root)
        {
            SetLayer(child, layer);
        }
    }

    //[PunRPC]
    //private void PickUpDestroy()
    //{
    //    if (PhotonNetwork.IsMasterClient)
    //    {
    //        PhotonNetwork.RemoveRPCs(photonView);
    //        PhotonNetwork.Destroy(gameObject);
    //    }
    //}

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
