using Photon.Pun;
using UnityEngine;

public class WeaponPrefab : MonoBehaviourPunCallbacks
{

    [SerializeField] private Transform projectileSpawn;

    private void Awake()
    {
        if (photonView.IsMine)
        {
            WeaponSlot.OnFireWeapon += WeaponSlot_OnFireWeapon;
            WeaponSlot.OnEquipWeapon += WeaponSlot_OnEquipWeapon;
        }
    }

    private void WeaponSlot_OnFireWeapon()
    {
        Weapon weapon = WeaponSlot.currentWeapon;

        ProjectileManager.ProjectileData data = new ProjectileManager.ProjectileData
        {
            spawnPosition = projectileSpawn.position,
            spawnRotation = projectileSpawn.rotation,
            projectilePool = weapon.bulletPoolName,
            speed = weapon.bulletSpeed,
            paintType = (int)weapon.paintType,
            paintAmount = weapon.paintDamage,
            projectileOwnerID = PlayerManager.instance.photonView.ViewID,
        };
        ProjectileManager.instance.FireProjectile(data);
    }

    private void WeaponSlot_OnEquipWeapon(Weapon weapon)
    {

    }

    [PunRPC]
    private void PickUpDestroy()
    {
        if (PhotonNetwork.IsMasterClient)
        {
            PhotonNetwork.RemoveRPCs(photonView);
            PhotonNetwork.Destroy(gameObject);
        }
    }

    public override void OnDisable()
    {
        if (photonView.IsMine)
        {
            WeaponSlot.OnFireWeapon -= WeaponSlot_OnFireWeapon;
            WeaponSlot.OnEquipWeapon -= WeaponSlot_OnEquipWeapon;
        }
    }
}
