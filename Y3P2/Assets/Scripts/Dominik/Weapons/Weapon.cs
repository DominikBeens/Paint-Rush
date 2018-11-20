using UnityEngine;

[CreateAssetMenu(menuName = "Items/Weapon")]
public class Weapon : Item
{

    [Space(10)]

    public float fireRate = 0.2f;
    public string bulletPoolName;

    public enum PaintColor { Red, Blue, Green, Yellow };
    public PaintColor paintColor;
}
