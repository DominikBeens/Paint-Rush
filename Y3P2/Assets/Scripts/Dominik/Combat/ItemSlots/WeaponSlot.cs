using Photon.Pun;
using System;
using UnityEngine;

public class WeaponSlot : EquipmentSlot
{

    public static Weapon currentWeapon;
    public static bool canAttack = true;
    public static PaintController.PaintType currentPaintType;

    private float nextAttackTime;
    private int numOfPaintTypes;
    private int currentPaintTypeIndex;

    private bool gamePaused;

    public static event Action OnFireWeapon = delegate { };
    public static event Action<Weapon> OnEquipWeapon = delegate { };
    public static event Action<Color> OnChangeAmmoType = delegate { };
    public static event Action OnHitEntity = delegate { };

    [SerializeField] private Transform weaponSpawn;
    [SerializeField] private Weapon startingWeapon;

    public override void Initialise(bool local)
    {
        base.Initialise(local);

        if (local)
        {
            numOfPaintTypes = Enum.GetValues(typeof(PaintController.PaintType)).Length;
            EquipWeapon(startingWeapon);
            DB.MenuPack.SceneManager.OnGamePaused += SceneManager_OnGamePaused;
        }
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.G))
        {
            photonView.RPC("DebugSyncShoot", RpcTarget.All);
        }

        if (CanUseWeapon())
        {
            HandleFiring();
        }

        HandleMousePaintSwitching();
        HandleKeyPaintSwitching();
    }

    [PunRPC]
    private void DebugSyncShoot()
    {
        OnFireWeapon();
    }

    private void HandleFiring()
    {
        if (Input.GetMouseButton(0))
        {
            if (Time.time >= nextAttackTime)
            {
                nextAttackTime = Time.time + currentWeapon.fireRate;
                OnFireWeapon();
            }
        }
    }

    private void HandleMousePaintSwitching()
    {
        float scroll = Input.GetAxis("Mouse ScrollWheel");
        if (scroll == 0)
        {
            return;
        }

        currentPaintTypeIndex += scroll > 0 ? 1 : 0;
        currentPaintTypeIndex -= scroll < 0 ? 1 : 0;

        currentPaintTypeIndex = currentPaintTypeIndex == numOfPaintTypes ? 0 : currentPaintTypeIndex;
        currentPaintTypeIndex = currentPaintTypeIndex == -1 ? numOfPaintTypes - 1 : currentPaintTypeIndex;


        currentPaintType = (PaintController.PaintType)currentPaintTypeIndex;
        OnChangeAmmoType(PlayerManager.instance.entity.paintController.GetPaintColor(currentPaintType));
    }

    private void HandleKeyPaintSwitching()
    {
        bool pressedKey = false;

        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            currentPaintTypeIndex = 0;
            pressedKey = true;
        }
        if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            currentPaintTypeIndex = 1;
            pressedKey = true;
        }
        if (Input.GetKeyDown(KeyCode.Alpha3))
        {
            currentPaintTypeIndex = 2;
            pressedKey = true;
        }
        if (Input.GetKeyDown(KeyCode.Alpha4))
        {
            currentPaintTypeIndex = 3;
            pressedKey = true;
        }

        if (pressedKey)
        {
            currentPaintType = (PaintController.PaintType)currentPaintTypeIndex;
            OnChangeAmmoType(PlayerManager.instance.entity.paintController.GetPaintColor(currentPaintType));
        }
    }

    private bool CanUseWeapon()
    {
        return currentWeapon != null && 
            equipedItem != null && 
            GameManager.CurrentGameSate != GameManager.GameState.Respawning && 
            !gamePaused && 
            TimeManager.CurrentGameTimeState != TimeManager.GameTimeState.Ending &&
            !CustomizationTerminal.customizing;
    }

    public void HitEntity()
    {
        OnHitEntity();
    }

    public void EquipWeapon(Weapon weapon)
    {
        int[] ids = Equip(weapon, weaponSpawn);
        currentWeapon = currentEquipment as Weapon;
        if (currentWeapon != null)
        {
            ParentEquipment(ids[0], ids[1]);
        }

        OnEquipWeapon(weapon);
    }

    protected override void ParentEquipment(int equipmentID, int parentID)
    {
        photonView.RPC("ParentWeapon", RpcTarget.All, equipmentID, parentID);
    }

    [PunRPC]
    private void ParentWeapon(int equipmentID, int parentID)
    {
        PhotonView pv = PhotonNetwork.GetPhotonView(equipmentID);
        if (pv)
        {
            pv.transform.SetParent(PhotonNetwork.GetPhotonView(parentID).transform);
            pv.transform.localPosition = Vector3.zero;
            pv.transform.localRotation = Quaternion.identity;
        }
        else
        {
            NotificationManager.instance.NewLocalNotification("<color=red>DEBUG: Couldnt find PhotonView to parent weapon to!");
            Debug.LogWarning("Couldnt find PhotonView to parent weapon to! Weapon might be floating in space right now.");
        }
    }

    public override void OnPlayerEnteredRoom(Photon.Realtime.Player newPlayer)
    {
        if (photonView.IsMine)
        {
            int[] ids = GetEquipedItemIDs(weaponSpawn);
            ParentEquipment(ids[0], ids[1]);
        }
    }

    private void SceneManager_OnGamePaused(bool b)
    {
        gamePaused = b;
    }

    public override void OnDisable()
    {
        DB.MenuPack.SceneManager.OnGamePaused -= SceneManager_OnGamePaused;
    }
}
