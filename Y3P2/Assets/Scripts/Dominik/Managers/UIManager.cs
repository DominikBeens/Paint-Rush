using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{

    public static UIManager instance;
    private Camera mainCam;
    private Vector3 screenMiddle;

    private struct LastHitTransform
    {
        public Transform transform;
        public string name;
        public Entity entity;
    }
    private LastHitTransform lastHitTransform;

    [Header("Crosshair Canvas")]
    [SerializeField] private List<Image> crosshair = new List<Image>();
    [SerializeField] private Animator crosshairAnim;

    [Header("Hit Player Canvas")]
    [SerializeField] private LayerMask playerLayerMask;
    [SerializeField] private GameObject hitPlayerPanel;
    [SerializeField] private TextMeshProUGUI hitPlayerText;

    [Header("Tab Canvas")]
    [SerializeField] private Canvas leaderboardAndStatsCanvas;

    [Header("Jump Canvas")]
    [SerializeField] private GameObject jumpCooldownIcon;
    public GameObject JumpCooldownIcon { get { return jumpCooldownIcon; } }
    [SerializeField] private GameObject jumpCooldown;
    [SerializeField] private List<Image> jumpCDBars = new List<Image>();

    [Header("Mark Canvas")]
    [SerializeField] private GameObject markObject;
    [SerializeField] private Image markImage;
    [SerializeField] private TextMeshProUGUI markHealthText;
    private ParticleSystem[] markParticles;
    private float markHealth;

    [Header("Portal Effect Canvas")]
    [SerializeField] private Animator portalEffectAnim;

    [Header("Pickup Canvas")]
    [SerializeField] private Image pickUpImage;
    [SerializeField] private Image pickUpImageParent;
    public Image PickUpImageParent { get { return pickUpImageParent; } }

    [Header("Screen Hit Canvas")]
    [SerializeField] private Animator screenHitAnim;
    [SerializeField] private Image screenHitImage;

    [Header("Paint Values Canvas")]
    [SerializeField] private Canvas paintValuesCanvas;

    [Header("Game Tech Stats Canvas")]
    [SerializeField] private Canvas gameTechStatsCanvas;
    [SerializeField] private TextMeshProUGUI gameTechStatsText;
    private FPSCounter fpsCounter = new FPSCounter();

    [Header("Screen Hit Direction Canvas")]
    [SerializeField] private Transform screenHitDirectionMarker;

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

        SetupEvents();
        SetupUI();
    }

    public void Initialise(Color crosshairColor)
    {
        mainCam = Camera.main;
        WeaponSlot_OnChangeAmmoType(crosshairColor);
        PlayerManager.instance.entity.paintController.OnPaintMarkActivated += PaintController_OnPaintMarkActivated;
        PlayerManager.instance.entity.paintController.OnPaintMarkDestroyed += PaintController_OnPaintMarkDestroyed;
        PlayerManager.instance.entity.paintController.OnPaintValueModified += PaintController_OnPaintValueModified;

        SetupPaintValuesUI();
    }

    private void Update()
    {
        HandleTargetedPlayerPanel();
        HandleLeaderboardAndStatsPanel();

        fpsCounter.Update();
        ShowGameTechStats();
    }

    private void SetupEvents()
    {
        WeaponSlot.OnChangeAmmoType += WeaponSlot_OnChangeAmmoType;
        WeaponSlot.OnHitEntity += WeaponSlot_OnHit;

        DB.MenuPack.SceneManager.OnGamePaused += SceneManager_OnGamePaused;
        GameManager.OnGameStateChanged += GameManager_OnGameStateChanged;
        TimeManager.OnEndMatch += TimeManager_OnEndMatch;
        PortalTeleporter.OnUsePortal += PortalTeleporter_OnUsePortal;
    }

    private void SetupUI()
    {
        ToggleLeaderboardAndStats(false);

        markObject.SetActive(false);
        markParticles = markObject.GetComponentsInChildren<ParticleSystem>();

        hitPlayerPanel.SetActive(false);

        ToggleGameTechStatsCanvas(DB.MenuPack.Setting_GameStatsDisplay.settingValue);

        screenHitDirectionMarker.gameObject.SetActive(false);
    }

    public IEnumerator ShowJumpCooldownIcon(float cooldown)
    {
        jumpCooldown.SetActive(true);

        for (int i = 0; i < jumpCDBars.Count; i++)
        {
            jumpCDBars[i].fillAmount = 1;
        }

        float time = cooldown;
        while (jumpCooldown.activeInHierarchy)
        {
            time -= Time.deltaTime;

            for (int i = 0; i < jumpCDBars.Count; i++)
            {
                jumpCDBars[i].fillAmount -= Time.deltaTime / (cooldown * 2);
            }

            if (time <= 0)
            {
                jumpCooldown.SetActive(false);
            }

            yield return new WaitForEndOfFrame();
        }
    }

    public void SetPickUpImage(Sprite pickUpSprite, bool afterUse)
    {
        pickUpImage.sprite = !afterUse ? pickUpSprite : null;
        pickUpImage.color = new Color(pickUpImage.color.r, pickUpImage.color.g, pickUpImage.color.b, !afterUse ? 1 : 0);
    }

    private void GameManager_OnGameStateChanged(GameManager.GameState newState)
    {
        switch (newState)
        {
            case GameManager.GameState.Lobby:
                ToggleCrosshair(true);
                paintValuesCanvas.enabled = false;
                break;

            case GameManager.GameState.Playing:
                ToggleCrosshair(true);
                paintValuesCanvas.enabled = true;
                break;

            case GameManager.GameState.Respawning:
                ToggleCrosshair(false);
                paintValuesCanvas.enabled = false;
                break;

            case GameManager.GameState.Spectating:
                ToggleCrosshair(false);
                paintValuesCanvas.enabled = false;
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
                paintValuesCanvas.enabled = !b;
                break;
        }
    }

    private void TimeManager_OnEndMatch()
    {
        if (GameManager.CurrentGameSate == GameManager.GameState.Playing || GameManager.CurrentGameSate == GameManager.GameState.Respawning)
        {
            ToggleCrosshair(false);
        }
    }

    private void HandleTargetedPlayerPanel()
    {
        screenMiddle = new Vector3(Screen.width / 2f, Screen.height / 2f, 0);
        RaycastHit hit;
        if (Physics.Raycast(mainCam.ScreenPointToRay(screenMiddle), out hit, 500, playerLayerMask))
        {
            // If we hit someone new.
            if (hit.transform != lastHitTransform.transform)
            {
                // Save
                Entity entity = hit.transform.root.GetComponentInChildren<Entity>();
                lastHitTransform = entity ? 
                    new LastHitTransform { transform = hit.transform, name = entity.photonView.IsSceneView ? "" : entity.photonView.Owner.NickName, entity = entity } : 
                    new LastHitTransform { transform = hit.transform };

                // If its an entity
                if (lastHitTransform.entity)
                {
                    // If its an entity thats not from the scene (a player) show his name and paint values.
                    if (!string.IsNullOrEmpty(lastHitTransform.name))
                    {
                        hitPlayerText.text = lastHitTransform.entity.photonView.Owner.NickName;
                        hitPlayerText.text += "\n" + lastHitTransform.entity.paintController.GetAllPaintValuesText();
                    }
                    // Else if its something from the scene only show his paint values.
                    else
                    {
                        hitPlayerText.text = lastHitTransform.entity.paintController.GetAllPaintValuesText();
                    }
                }
                // If we hit something else, like a piece of terrain, display nothing.
                else
                {
                    hitPlayerPanel.SetActive(false);
                }
            }
            // If we hit the same transform as we hit last time.
            else
            {
                if (lastHitTransform.entity)
                {
                    // Last hit player has no name, show only his paint values.
                    if (string.IsNullOrEmpty(lastHitTransform.name))
                    {
                        hitPlayerText.text = lastHitTransform.entity.paintController.GetAllPaintValuesText();
                    }
                    // Otherwise show his name + paint values.
                    else
                    {
                        hitPlayerText.text = lastHitTransform.name;
                        hitPlayerText.text += "\n" + lastHitTransform.entity.paintController.GetAllPaintValuesText();
                    }

                    hitPlayerPanel.SetActive(true);
                }
                else
                {
                    hitPlayerPanel.SetActive(false);
                }
            }
        }
        else
        {
            hitPlayerPanel.SetActive(false);
        }
    }

    private void HandleLeaderboardAndStatsPanel()
    {
        if (Input.GetKey(KeyCode.Tab) && GameManager.CurrentGameSate == GameManager.GameState.Playing)
        {
            ToggleLeaderboardAndStats(true);
            ToggleCrosshair(false);
        }
        else if (Input.GetKeyUp(KeyCode.Tab))
        {
            ToggleLeaderboardAndStats(false);

            switch (GameManager.CurrentGameSate)
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
    }

    private void ToggleLeaderboardAndStats(bool toggle)
    {
        if (toggle != leaderboardAndStatsCanvas.enabled)
        {
            leaderboardAndStatsCanvas.enabled = toggle;
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

    private void PaintController_OnPaintMarkActivated(PaintController.PaintMark mark)
    {
        Color color = PlayerManager.instance.entity.paintController.GetPaintColor(mark.markType);

        markImage.color = color;
        markHealthText.color = color;

        for (int i = 0; i < markParticles.Length; i++)
        {
            ParticleSystem.MainModule particle = markParticles[i].main;
            particle.startColor = color;
        }

        markHealth = 100;
        markHealthText.text = markHealth + "%";

        markObject.SetActive(true);
    }

    private void PaintController_OnPaintMarkDestroyed()
    {
        markObject.SetActive(false);
    }

    private void PaintController_OnPaintValueModified(PaintController.PaintType paintType, float amount)
    {
        if (markHealthText.enabled)
        {
            markHealth -= amount;
            markHealth = Mathf.Clamp(markHealth, 0, 100);
            markHealthText.text = markHealth + "%";
        }
    }

    public void ToggleCrosshair(bool b)
    {
        for (int i = 0; i < crosshair.Count; i++)
        {
            crosshair[i].enabled = b;
        }
    }

    private void PortalTeleporter_OnUsePortal()
    {
        portalEffectAnim.SetTrigger("Trigger");
    }

    public void ScreenHitEffect(Color paintColor)
    {
        bool invert = Random.Range(0, 2) == 0 ? true : false;
        float randomScale = Random.Range(invert ? -1f : 1f, invert ? -1.1f : 1.1f);
        screenHitImage.transform.localScale = new Vector3(randomScale, randomScale, 1);

        screenHitImage.color = paintColor;
        screenHitAnim.SetTrigger("Hit");
    }

    public void ScreenHitDirectionMarker(Vector3 origin)
    {
        screenHitDirectionMarker.gameObject.SetActive(false);

        origin.y += 0.5f;
        Vector3 position = mainCam.WorldToViewportPoint(origin);
        Vector3 rotation = Vector3.zero;

        if (position.x > 0 && position.x < 1)
        {
            if (position.y > 0 && position.y < 1)
            {
                if (position.z > 0)
                {
                    // Origin is within view, don't show the marker.
                    return;
                }
            }
        }

        if (position.x < 0.5)
        {
            if (position.x < position.y)
            {
                // LEFT.
                position.x = 0.05f;
                position.y = 0.5f;
                rotation = new Vector3(0, 0, -90);
            }
            else
            {
                // DOWN.
                position.x = 0.5f;
                position.y = 0.2f;
                rotation = new Vector3(0, 0, 0);
            }
        }
        else
        {
            if (position.x > position.y)
            {
                // RIGHT.
                position.x = 0.95f;
                position.y = 0.5f;
                rotation = new Vector3(0, 0, -270);
            }
            else
            {
                // UP.
                position.x = 0.5f;
                position.y = 0.85f;
                rotation = new Vector3(0, 0, -180);
            }
        }

        // Origin is behind the player, flip values.
        if (position.z < 0)
        {
            position.x = Mathf.Abs(position.x - 1);
            position.y = Mathf.Abs(position.y - 1);

            rotation = new Vector3(0, 0, rotation.z - 180);
        }

        position.x *= Screen.width;
        position.y *= Screen.height;

        screenHitDirectionMarker.transform.position = position;
        screenHitDirectionMarker.transform.localEulerAngles = rotation;
        screenHitDirectionMarker.gameObject.SetActive(true);
    }

    private void SetupPaintValuesUI()
    {
        PaintUILocalPlayer[] paintDisplayBars = paintValuesCanvas.GetComponentsInChildren<PaintUILocalPlayer>();
        for (int i = 0; i < PlayerManager.instance.entity.paintController.PaintValues.Count; i++)
        {
            if (paintDisplayBars.Length > 0 && paintDisplayBars[i])
            {
                paintDisplayBars[i].Initialise(PlayerManager.instance.entity.paintController.PaintValues[i]);
            }
        }
    }

    public void ToggleGameTechStatsCanvas(bool toggle)
    {
        gameTechStatsCanvas.enabled = toggle;
    }

    private void ShowGameTechStats()
    {
        if (gameTechStatsCanvas.enabled)
        {
            gameTechStatsText.text = string.Format("FPS: <color=red>{0}</color> Ms: <color=red>{1}", fpsCounter.FrameRate.ToString("F0"), Photon.Pun.PhotonNetwork.GetPing());
        }
    }

    private void OnDisable()
    {
        WeaponSlot.OnChangeAmmoType -= WeaponSlot_OnChangeAmmoType;
        WeaponSlot.OnHitEntity -= WeaponSlot_OnHit;

        DB.MenuPack.SceneManager.OnGamePaused -= SceneManager_OnGamePaused;
        GameManager.OnGameStateChanged -= GameManager_OnGameStateChanged;
        TimeManager.OnEndMatch -= TimeManager_OnEndMatch;
        PortalTeleporter.OnUsePortal -= PortalTeleporter_OnUsePortal;

        PlayerManager.instance.entity.paintController.OnPaintMarkActivated -= PaintController_OnPaintMarkActivated;
        PlayerManager.instance.entity.paintController.OnPaintMarkDestroyed -= PaintController_OnPaintMarkDestroyed;
        PlayerManager.instance.entity.paintController.OnPaintValueModified -= PaintController_OnPaintValueModified;
    }
}
