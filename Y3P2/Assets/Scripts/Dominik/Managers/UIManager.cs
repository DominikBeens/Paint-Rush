using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{

    public static UIManager instance;

    [SerializeField] private List<Image> crosshair = new List<Image>();
    [SerializeField] private Animator crosshairAnim;

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

        WeaponSlot.OnChangeAmmoType += WeaponSlot_OnChangeAmmoType;
        WeaponSlot.OnHitEntity += WeaponSlot_OnHit;
    }

    public void Initialise(Color crosshairColor)
    {
        WeaponSlot_OnChangeAmmoType(crosshairColor);
    }

    private void WeaponSlot_OnChangeAmmoType(Color color)
    {
        for (int i = 0; i < crosshair.Count; i++)
        {
            crosshair[i].color = color;
        }

        crosshairAnim.SetTrigger("ChangeAmmo");
    }

    private void WeaponSlot_OnHit()
    {
        crosshairAnim.SetTrigger("Hit");
    }

    private void OnDisable()
    {
        WeaponSlot.OnChangeAmmoType -= WeaponSlot_OnChangeAmmoType;
        WeaponSlot.OnHitEntity -= WeaponSlot_OnHit;
    }
}
