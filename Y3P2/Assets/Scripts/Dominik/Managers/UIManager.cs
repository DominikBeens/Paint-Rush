using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{

    public static UIManager instance;
    private Camera mainCam;
    private Vector3 screenMiddle;

    private struct LastHitPlayer
    {
        public Transform transform;
        public string name;
    }
    private LastHitPlayer lastHitPlayer; 

    [SerializeField] private List<Image> crosshair = new List<Image>();
    [SerializeField] private Animator crosshairAnim;

    [Space(10)]

    [SerializeField] private List<PaintUILocalPlayer> paintUILocalPlayer = new List<PaintUILocalPlayer>();
    public List<PaintUILocalPlayer> PaintUILocalPlayer { get { return paintUILocalPlayer; } }

    [Space(10)]

    [SerializeField] private LayerMask playerLayerMask;
    [SerializeField] private TextMeshProUGUI hitPlayerText;

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

        DB.MenuPack.SceneManager.OnGamePaused += SceneManager_OnGamePaused;
        GameManager.OnGameStateChanged += GameManager_OnGameStateChanged;

    }

    private void GameManager_OnGameStateChanged(GameManager.GameState newState)
    {
        ToggleCrosshair(newState == GameManager.GameState.Playing ? true : false);
    }

    private void SceneManager_OnGamePaused(bool b)
    {
        if (GameManager.CurrentGameSate == GameManager.GameState.Playing)
        {
            ToggleCrosshair(!b);
        }
    }

    public void Initialise(Color crosshairColor)
    {
        mainCam = Camera.main;
        WeaponSlot_OnChangeAmmoType(crosshairColor);
    }

    private void Update()
    {
        screenMiddle = new Vector3(Screen.width / 2f, Screen.height / 2f, 0);
        RaycastHit hit;
        if (Physics.Raycast(mainCam.ScreenPointToRay(screenMiddle), out hit, 100, playerLayerMask))
        {
            if (hit.transform != lastHitPlayer.transform)
            {
                Entity entity = hit.transform.root.GetComponentInChildren<Entity>();
                if (entity)
                {
                    hitPlayerText.text = entity.photonView.Owner.NickName;
                    lastHitPlayer = new LastHitPlayer { transform = hit.transform, name = entity.photonView.Owner.NickName };
                }
            }
            else
            {
                hitPlayerText.text = lastHitPlayer.name;
            }
        }
        else
        {
            hitPlayerText.text = "";
        }
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

    private void ToggleCrosshair(bool b)
    {
        for (int i = 0; i < crosshair.Count; i++)
        {
            crosshair[i].enabled = b;
        }
    }

    private void OnDisable()
    {
        WeaponSlot.OnChangeAmmoType -= WeaponSlot_OnChangeAmmoType;
        WeaponSlot.OnHitEntity -= WeaponSlot_OnHit;

        DB.MenuPack.SceneManager.OnGamePaused -= SceneManager_OnGamePaused;
        GameManager.OnGameStateChanged -= GameManager_OnGameStateChanged;
    }
}
