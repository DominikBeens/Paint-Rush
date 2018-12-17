using UnityEngine;

[CreateAssetMenu(menuName = "Items/Weapon Projectile")]
public class Weapon_Projectile : Weapon
{

    [Header("Projectile Settings")]

    [SerializeField] private string projectilePoolName;
}