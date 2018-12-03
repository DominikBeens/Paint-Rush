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

    [SerializeField] private GameObject leaderboardAndStatsCanvas;
    [SerializeField] private TextMeshProUGUI statsText;
    [SerializeField] private TextMeshProUGUI leaderboardText;

    [Space(10)]

    [SerializeField]
    private GameObject jumpCooldownIcon;
    public GameObject JumpCooldownIcon { get { return jumpCooldownIcon; } }

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

        ToggleLeaderboardAndStats(false);
    }

    private void GameManager_OnGameStateChanged(GameManager.GameState newState)
    {
        switch (newState)
        {
            case GameManager.GameState.Lobby:

                ToggleCrosshair(true);
                break;
            case GameManager.GameState.Playing:

                ToggleCrosshair(true);
                break;
            case GameManager.GameState.Respawning:

                ToggleCrosshair(false);
                break;
        }
    }

    private void SceneManager_OnGamePaused(bool b)
    {
        switch (GameManager.CurrentGameSate)
        {
            case GameManager.GameState.Lobby:

                ToggleCrosshair(!b);
                break;
            case GameManager.GameState.Playing:

                ToggleCrosshair(!b);
                break;
        }
    }

    public void Initialise(Color crosshairColor)
    {
        mainCam = Camera.main;
        WeaponSlot_OnChangeAmmoType(crosshairColor);
    }

    private void Update()
    {
        HandleTargetedPlayerPanel();
        HandleLeaderboardAndStatsPanel();
    }

    private void HandleTargetedPlayerPanel()
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
    }

    private void HandleLeaderboardAndStatsPanel()
    {
        if (Input.GetKey(KeyCode.Tab))
        {
            ToggleLeaderboardAndStats(true);
            ToggleCrosshair(false);
        }
        else if (Input.GetKeyUp(KeyCode.Tab))
        {
            ToggleLeaderboardAndStats(false);
            ToggleCrosshair(GameManager.CurrentGameSate == GameManager.GameState.Playing ? true : false);
            leaderboardText.text = null;
        }

        if (leaderboardAndStatsCanvas.activeInHierarchy)
        {
            int markAccuracy = SaveManager.instance.GetSavedStat(SaveManager.SavedStat.MarksGained) == 0 ? 0 : 
                (int)((float)SaveManager.instance.GetSavedStat(SaveManager.SavedStat.GamePointsGained) / SaveManager.instance.GetSavedStat(SaveManager.SavedStat.MarksGained) * 100);
            int shotAccuracy = SaveManager.instance.GetSavedStat(SaveManager.SavedStat.ShotsFired) == 0 ? 0 : 
                (int)((float)SaveManager.instance.GetSavedStat(SaveManager.SavedStat.ShotsHit) / SaveManager.instance.GetSavedStat(SaveManager.SavedStat.ShotsFired) * 100);

            statsText.text =
                "Kills: <color=yellow>" + SaveManager.instance.GetSavedStat(SaveManager.SavedStat.Kills) + "</color>\n" +
                "Deaths: <color=yellow>" + SaveManager.instance.GetSavedStat(SaveManager.SavedStat.Deaths) + "</color>\n\n" +
                "Marks Gained: <color=yellow>" + SaveManager.instance.GetSavedStat(SaveManager.SavedStat.MarksGained) + "</color>\n" +
                "Marks Destroyed: <color=yellow>" + SaveManager.instance.GetSavedStat(SaveManager.SavedStat.MarksDestroyed) + "</color>\n" +
                "Game-points Gained: <color=yellow>" + SaveManager.instance.GetSavedStat(SaveManager.SavedStat.GamePointsGained) + "</color>\n" +
                "Mark Accuracy: <color=yellow>" + markAccuracy + "%</color>\n\n" +
                "Shots Fired: <color=yellow>" + SaveManager.instance.GetSavedStat(SaveManager.SavedStat.ShotsFired) + "</color>\n" +
                "Shots Hit: <color=yellow>" + SaveManager.instance.GetSavedStat(SaveManager.SavedStat.ShotsHit) + "</color>\n" +
                "Accuracy: <color=yellow>" + shotAccuracy + "%</color>\n\n" +
                "Pickups Collected: <color=yellow>" + SaveManager.instance.GetSavedStat(SaveManager.SavedStat.PickupsCollected) + "</color>";

            if (string.IsNullOrEmpty(leaderboardText.text))
            {
                List<ScoreboardManager.PlayerScore> latestPlayerGameStats = ScoreboardManager.instance.GetSortedPlayerScores();

                for (int i = 0; i < latestPlayerGameStats.Count; i++)
                {
                    leaderboardText.text += latestPlayerGameStats[i].playerName + ": <color=yellow>" + latestPlayerGameStats[i].playerGamePoints + "</color>\n";
                }
            }
        }
    }

    private void ToggleLeaderboardAndStats(bool toggle)
    {
        if (toggle != leaderboardAndStatsCanvas.activeInHierarchy)
        {
            leaderboardAndStatsCanvas.SetActive(toggle);
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
