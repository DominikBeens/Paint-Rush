using Photon.Pun;
using UnityEngine;

public class WeaponPrefab : MonoBehaviourPunCallbacks
{

    private Vector3 screenMiddle;

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
        screenMiddle = new Vector3(Screen.width / 2f, Screen.height / 2f, 0);
        Vector3 fireDirection = Vector3.zero;

        RaycastHit hitFromCam;
        if (Physics.Raycast(Camera.main.ScreenPointToRay(screenMiddle), out hitFromCam))
        {
            fireDirection = hitFromCam.point - projectileSpawn.position;
        }

        Weapon weapon = WeaponSlot.currentWeapon;

        ProjectileManager.ProjectileData data = new ProjectileManager.ProjectileData
        {
            spawnPosition = projectileSpawn.position,
            spawnRotation = fireDirection != Vector3.zero ? Quaternion.LookRotation(fireDirection, Vector3.forward) : projectileSpawn.rotation,
            //spawnRotation = projectileSpawn.rotation,
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
