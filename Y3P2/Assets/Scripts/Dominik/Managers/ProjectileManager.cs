using Photon.Pun;
using UnityEngine;

public class ProjectileManager : MonoBehaviourPunCallbacks
{

    public static ProjectileManager instance;

    public class ProjectileData
    {
        public Vector3 spawnPosition;
        public Quaternion spawnRotation;
        public string projectilePool;
        public float speed;
        public int projectileOwnerID = 9999;
    }

    private void Awake()
    {
        if (!instance)
        {
            instance = this;
        }
        else if (instance && instance != this)
        {
            Destroy(this);
        }
    }

    public void FireProjectile(ProjectileData data)
    {
        photonView.RPC("FireProjectileRPC", RpcTarget.All,
            data.spawnPosition,
            data.spawnRotation,
            data.projectilePool,
            data.speed,
            data.projectileOwnerID);
    }

    [PunRPC]
    private void FireProjectileRPC(Vector3 position, Quaternion rotation, string projectilePoolName, float speed, int ownerID = 9999)
    {
        Projectile newProjectile = ObjectPooler.instance.GrabFromPool(projectilePoolName, position, rotation).GetComponent<Projectile>();
        newProjectile.Fire(new Projectile.FireData
        {
            speed = speed,
            ownerID = ownerID,
        });
    }
}
