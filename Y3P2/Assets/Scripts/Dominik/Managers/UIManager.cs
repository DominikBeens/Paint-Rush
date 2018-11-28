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

    [Space(10)]

    [SerializeField] private GameObject statsPanel;
    [SerializeField] private TextMeshProUGUI statsText;

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

        ToggleStatsPanel(false);
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
                if (entity && !entity.photonView.IsSceneView)
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

        if (Input.GetKey(KeyCode.Tab))
        {
            ToggleStatsPanel(true);
        }
        else if (Input.GetKeyUp(KeyCode.Tab))
        {
            ToggleStatsPanel(false);
        }

        if (statsPanel.activeInHierarchy)
        {
            statsText.text =
                "Kills: <color=yellow>" + SaveManager.saveData.kills + "</color>\n" +
                "Deaths: <color=yellow>" + SaveManager.saveData.deaths + "</color>\n\n" +
                "Marks Gained: <color=yellow>" + SaveManager.saveData.marksGained + "</color>\n" +
                "Marks Destroyed: <color=yellow>" + SaveManager.saveData.marksDestroyed + "</color>\n" +
                "Game-points Gained: <color=yellow>" + SaveManager.saveData.gamePointsGained + "</color>\n\n" +
                "Shots Fired: <color=yellow>" + SaveManager.saveData.shotsFired + "</color>\n" +
                "Shots Hit: <color=yellow>" + SaveManager.saveData.shotsHit + "</color>\n\n" +
                "Pickups Collected: <color=yellow>" + SaveManager.saveData.pickupsCollected + "</color>";
        }
    }

    private void ToggleStatsPanel(bool toggle)
    {
        if (toggle != statsPanel.activeInHierarchy)
        {
            statsPanel.SetActive(toggle);
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
