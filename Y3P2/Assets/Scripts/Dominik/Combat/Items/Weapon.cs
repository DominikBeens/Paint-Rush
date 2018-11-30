using UnityEngine;

[CreateAssetMenu(menuName = "Items/Weapon")]
public class Weapon : Item
{

    [Space(10)]

    public float fireRate = 0.2f;
    public float paintDamage = 1f;
    public string paintImpactPoolName;
}
