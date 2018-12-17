using UnityEngine;

[CreateAssetMenu(menuName = "Items/Weapon Projectile")]
public class Weapon_Projectile : Weapon
{

    [Header("Projectile Settings")]

    public string projectilePoolName;
    public float projectileSpeed;
}