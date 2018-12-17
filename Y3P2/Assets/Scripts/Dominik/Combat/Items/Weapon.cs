using UnityEngine;

public class Weapon : Item
{

    [Header("Default Settings")]

    public float fireRate = 0.2f;
    public float paintDamage = 1f;
    public bool spawnParticleOnImpact;
    public bool spawnDecalOnImpact;
}
