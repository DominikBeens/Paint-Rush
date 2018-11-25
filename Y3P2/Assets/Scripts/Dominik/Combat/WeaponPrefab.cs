using Photon.Pun;
using UnityEngine;

public class WeaponPrefab : MonoBehaviourPunCallbacks
{

    private Vector3 screenMiddle;
    private Camera mainCam;

    [SerializeField] private Transform projectileSpawn;

    private void Awake()
    {
        if (photonView.IsMine)
        {
            mainCam = Camera.main;

            WeaponSlot.OnFireWeapon += WeaponSlot_OnFireWeapon;
            WeaponSlot.OnEquipWeapon += WeaponSlot_OnEquipWeapon;
        }
        else
        {
            SetLayer(transform, 10);
        }
    }

    private void WeaponSlot_OnFireWeapon()
    {
        screenMiddle = new Vector3(Screen.width / 2f, Screen.height / 2f, 0);

        RaycastHit hitFromCam;
        if (Physics.Raycast(mainCam.ScreenPointToRay(screenMiddle), out hitFromCam))
        {
            RaycastHit hitFromWeapon;
            if (Physics.Raycast(projectileSpawn.position, (hitFromCam.point - projectileSpawn.position), out hitFromWeapon))
            {
                Entity hitEntity = hitFromWeapon.transform.GetComponentInChildren<Entity>();
                if (hitEntity)
                {
                    hitEntity.Hit((int)WeaponSlot.currentPaintType, WeaponSlot.currentWeapon.paintDamage);
                    PlayerManager.instance.weaponSlot.HitEntity();
                }

                if (!string.IsNullOrEmpty(WeaponSlot.currentWeapon.paintImpactPoolName))
                {
                    photonView.RPC("SpawnPrefabOnHit", RpcTarget.All, WeaponSlot.currentWeapon.paintImpactPoolName, hitFromWeapon.point, (int)WeaponSlot.currentPaintType);
                }
            }
        }

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
    private void SpawnPrefabOnHit(string prefabPoolName, Vector3 position, int paintType)
    {
        GameObject newSpawn = ObjectPooler.instance.GrabFromPool(prefabPoolName, position, transform.rotation);
        PaintImpactParticle pip = newSpawn.GetComponent<PaintImpactParticle>();
        if (pip)
        {
            pip.Initialise(PlayerManager.instance.entity.paintController.GetPaintColor((PaintController.PaintType)paintType));
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
        }
    }
}
