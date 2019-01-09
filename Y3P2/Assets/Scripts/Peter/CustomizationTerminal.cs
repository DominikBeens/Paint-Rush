using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using UnityEngine.UI;
using TMPro;
public class CustomizationTerminal : MonoBehaviourPunCallbacks
{
    [SerializeField]
    private TextMeshProUGUI nameText;
    [SerializeField]
    private Animator previewCharacterAnimator;
    private Transform localPlayer;
    private PlayerAnimationController pcontroller;
    private PlayerAudioManager paudio;
    public static bool customizing;

    [SerializeField]
    private List<AnimationClip> emotes = new List<AnimationClip>();
    public List<AnimationClip> Emotes { get { return emotes; } }

    [SerializeField]
    private List<AudioClip> music = new List<AudioClip>();
    public List<AudioClip> Music { get { return music; } }

    [SerializeField]
    private List<Material> skins = new List<Material>();
    public List<Material> Skins { get { return skins; } }
    private int selectedSkin;
    [SerializeField]
    private AudioSource terminalAudioSource;

    [SerializeField]
    private GameObject terminalCamera;
    public GameObject TerminalCamera { get { return terminalCamera; } }

    [SerializeField]
    private GameObject audioVisualizer;
    public GameObject AudioVisualizer { get { return audioVisualizer; } }

    [SerializeField]
    private Renderer previewCharRenderer;

    private SyncPlayerSkin skinSyncer;
    private void Start()
    {
        localPlayer = PlayerManager.localPlayer;
        pcontroller = localPlayer.GetComponentInChildren<PlayerAnimationController>();
        paudio = localPlayer.GetComponent<PlayerAudioManager>();
        nameText.color = GameManager.personalColor;

        terminalAudioSource = GetComponent<AudioSource>();

        SetVictoryEmote(0);
        SetVictoryMusic(0);

        terminalAudioSource.clip = paudio.WinMusic;
        audioVisualizer.SetActive(false);

        skinSyncer = localPlayer.GetComponent<SyncPlayerSkin>();

    }

    private void Update()
    {
        if (customizing)
        {
            if (Input.GetKeyDown("e"))
            {
                customizing = false;
                Cursor.visible = false;
                Cursor.lockState = CursorLockMode.Locked;

                terminalCamera.SetActive(false);
                audioVisualizer.SetActive(false);
                UIManager.instance.ToggleCrosshair(true);

                //if (!previewCharacterAnimator.GetCurrentAnimatorStateInfo(0).IsName("Locomotion"))
                //{
                //    previewCharacterAnimator.Play("Locomotion", 0);
                //}

                if (terminalAudioSource.isPlaying)
                {
                    terminalAudioSource.Stop();
                }
            }
        }
    }

    public void SetNameColor(Color value)
    {
        GameManager.personalColor = value;
        GameManager.personalColorString = ColorUtility.ToHtmlStringRGBA(value);
        nameText.color = value;
    }

    public void SetRandomNameColor()
    {
        GameManager.personalColor = new Color(UnityEngine.Random.value, UnityEngine.Random.value, UnityEngine.Random.value, 1);
        GameManager.personalColorString = ColorUtility.ToHtmlStringRGBA(GameManager.personalColor);
        nameText.color = GameManager.personalColor;
    }

    public void ReleaseMouse()
    {
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;
        customizing = true;

        terminalCamera.SetActive(true);
        audioVisualizer.SetActive(true);

        UIManager.instance.ToggleCrosshair(false);
    }

    public void SetVictoryEmote(int index)
    {
        pcontroller.SetWinEmote(index, this);
        if (!previewCharacterAnimator.GetCurrentAnimatorStateInfo(0).IsName("Locomotion"))
        {
            previewCharacterAnimator.Play("Locomotion", 0);
        }
    }

    public void SetVictoryMusic(int index)
    {
        paudio.SetWinMusic(music[index], index);
        terminalAudioSource.clip = paudio.WinMusic;

        if (terminalAudioSource.isPlaying)
        {
            terminalAudioSource.Stop();
        }
    }

    public void PreviewEmote()
    {
        if(previewCharacterAnimator.GetCurrentAnimatorStateInfo(0).IsName("Locomotion"))
        {
            previewCharacterAnimator.Play(pcontroller.WinEmote.name, 0);
        }
        else
        {
            previewCharacterAnimator.Play("Locomotion", 0);
        }
    }

    public void PreviewMusic()
    {
        if (!terminalAudioSource.isPlaying)
        {
            terminalAudioSource.Play();
        }
        else
        {
            terminalAudioSource.Stop();
        }
    }

    public void SetSkin(int i)
    {

       
        if(selectedSkin < skins.Count - 1)
        {
            selectedSkin += i;
        }
        else if(selectedSkin == skins.Count -1)
        {
            selectedSkin = 0;
        }
        if(selectedSkin < 0)
        {
            selectedSkin = skins.Count - 1;
        }

       previewCharRenderer.material = skins[selectedSkin];
        skinSyncer.SyncThisPlayerSkin(selectedSkin);
    }

  

}
