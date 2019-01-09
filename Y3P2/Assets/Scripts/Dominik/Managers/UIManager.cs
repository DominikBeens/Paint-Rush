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

    [SerializeField] private List<Image> crosshair = new List<Image>();
    [SerializeField] private Animator crosshairAnim;

    [Space]

    [SerializeField] private LayerMask playerLayerMask;
    [SerializeField] private GameObject hitPlayerPanel;
    [SerializeField] private TextMeshProUGUI hitPlayerText;

    [Space]

    [SerializeField] private GameObject leaderboardAndStatsCanvas;

    [Space]

    [SerializeField]
    private GameObject jumpCooldownIcon;
    public GameObject JumpCooldownIcon { get { return jumpCooldownIcon; } }

    [Space]

    [SerializeField] private GameObject jumpCooldown;
    [SerializeField] private List<Image> jumpCDBars = new List<Image>();

    [Space]

    private float markHealth;
    [SerializeField] private Image markImage;
    [SerializeField] private TextMeshProUGUI markHealthText;

    [Space]

    [SerializeField] private Animator portalEffectAnim;

    [Space]

    [SerializeField] private Image pickUpImage;
    [SerializeField] private Image pickUpImageParent;
    public Image PickUpImageParent { get { return pickUpImageParent; } }

    [Space]

    [SerializeField] private Animator screenHitAnim;
    [SerializeField] private Image screenHitImage;

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
        TimeManager.OnGameTimeStateChanged += TimeManager_OnGameTimeStateChanged;
        PortalTeleporter.OnUsePortal += PortalTeleporter_OnUsePortal;

        ToggleLeaderboardAndStats(false);

        markImage.enabled = false;
        markHealthText.enabled = false;
        hitPlayerPanel.SetActive(false);
    }

    public void Initialise(Color crosshairColor)
    {
        mainCam = Camera.main;
        WeaponSlot_OnChangeAmmoType(crosshairColor);
        PlayerManager.instance.entity.paintController.OnPaintMarkActivated += PaintController_OnPaintMarkActivated;
        PlayerManager.instance.entity.paintController.OnPaintMarkDestroyed += PaintController_OnPaintMarkDestroyed;
        PlayerManager.instance.entity.paintController.OnPaintValueModified += PaintController_OnPaintValueModified;
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
        if (!afterUse)
        {
            pickUpImage.sprite = pickUpSprite;
            pickUpImage.color = new Color(pickUpImage.color.r, pickUpImage.color.g, pickUpImage.color.b, 1);
        }
        else
        {
            pickUpImage.sprite = null;
            pickUpImage.color = new Color(pickUpImage.color.r, pickUpImage.color.g, pickUpImage.color.b, 0);
        }
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

    private void TimeManager_OnGameTimeStateChanged(TimeManager.GameTimeState newState)
    {
        switch (newState)
        {
            case TimeManager.GameTimeState.Ending:
                ToggleCrosshair(false);
                break;
        }
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

    private void PaintController_OnPaintMarkActivated(PaintController.PaintMark mark)
    {
        markImage.color = PlayerManager.instance.entity.paintController.GetPaintColor(mark.markType);
        markHealthText.color = PlayerManager.instance.entity.paintController.GetPaintColor(mark.markType);
        markHealth = 100;
        markHealthText.text = markHealth + "%";

        markHealthText.enabled = true;
        markImage.enabled = true;
    }

    private void PaintController_OnPaintMarkDestroyed()
    {
        markImage.enabled = false;
        markHealthText.enabled = false;
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

    private void OnDisable()
    {
        WeaponSlot.OnChangeAmmoType -= WeaponSlot_OnChangeAmmoType;
        WeaponSlot.OnHitEntity -= WeaponSlot_OnHit;

        DB.MenuPack.SceneManager.OnGamePaused -= SceneManager_OnGamePaused;
        GameManager.OnGameStateChanged -= GameManager_OnGameStateChanged;
        TimeManager.OnGameTimeStateChanged -= TimeManager_OnGameTimeStateChanged;
        PortalTeleporter.OnUsePortal -= PortalTeleporter_OnUsePortal;

        PlayerManager.instance.entity.paintController.OnPaintMarkActivated -= PaintController_OnPaintMarkActivated;
        PlayerManager.instance.entity.paintController.OnPaintMarkDestroyed -= PaintController_OnPaintMarkDestroyed;
        PlayerManager.instance.entity.paintController.OnPaintValueModified -= PaintController_OnPaintValueModified;
    }
}
