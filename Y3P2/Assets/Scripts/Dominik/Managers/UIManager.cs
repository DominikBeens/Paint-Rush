﻿using System.Collections;
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

    [SerializeField] private Image markImage;

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

        markImage.enabled = false;
        hitPlayerPanel.SetActive(false);
    }

    public void Initialise(Color crosshairColor)
    {
        mainCam = Camera.main;
        WeaponSlot_OnChangeAmmoType(crosshairColor);
        PlayerManager.instance.entity.paintController.OnPaintMarkActivated += PaintController_OnPaintMarkActivated;
        PlayerManager.instance.entity.paintController.OnPaintMarkDestroyed += PaintController_OnPaintMarkDestroyed;
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
        markImage.enabled = true;
    }

    private void PaintController_OnPaintMarkDestroyed()
    {
        markImage.enabled = false;
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

        PlayerManager.instance.entity.paintController.OnPaintMarkActivated -= PaintController_OnPaintMarkActivated;
        PlayerManager.instance.entity.paintController.OnPaintMarkDestroyed -= PaintController_OnPaintMarkDestroyed;
    }
}
