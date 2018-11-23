using UnityEngine;

[CreateAssetMenu(menuName = "Items/Weapon")]
public class Weapon : Item
{

    [Space(10)]

    public float fireRate = 0.2f;
    public float bulletSpeed = 10f;
    public string bulletPoolName;

    public PaintController.PaintType paintType;
    public float paintDamage = 1f;
    public string paintImpactPoolName;
}
