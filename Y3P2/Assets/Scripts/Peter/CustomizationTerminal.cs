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
    private List<AnimationClip> normalEmotes = new List<AnimationClip>();
    public List<AnimationClip> NormalEmotes { get { return normalEmotes; } }

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
    public Renderer PreviewCharRenderer { get { return previewCharRenderer; } }

    private SyncPlayerSkin skinSyncer;

    [SerializeField]
    private GameObject colorPicker;
    [SerializeField]
    private GameObject previewCamRenderTexture;

    [SerializeField]
    private Material secretSkin;
    public Material SecretSkin { get { return secretSkin; } }
    
    private bool waitingEmote = false;

    private string secretCode;

    [SerializeField]
    private Animator anim;
    private void Start()
    {
        localPlayer = PlayerManager.localPlayer;
        pcontroller = localPlayer.GetComponentInChildren<PlayerAnimationController>();
        paudio = localPlayer.GetComponent<PlayerAudioManager>();
        nameText.color = GameManager.personalColor;

        terminalAudioSource = GetComponent<AudioSource>();

        SetVictoryEmote(0);
        SetVictoryMusic(0);
        SetEmote(0);

        terminalAudioSource.clip = paudio.WinMusic;
        audioVisualizer.SetActive(false);

        skinSyncer = localPlayer.GetComponent<SyncPlayerSkin>();

        GameManager.OnGameStateChanged += DisableEnableOnStateChange;


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

    private void DisableEnableOnStateChange(GameManager.GameState gameState)
    {
        if(gameState == GameManager.GameState.Lobby)
        {
            colorPicker.SetActive(true);
            previewCamRenderTexture.SetActive(true);
        }
        else
        {
            colorPicker.SetActive(false);
            previewCamRenderTexture.SetActive(false);
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
        anim.SetBool("Customize", true);
    }

    public void SetVictoryEmote(int index)
    {
        pcontroller.SetWinEmote(index, this);
        if (!previewCharacterAnimator.GetCurrentAnimatorStateInfo(0).IsName("Locomotion"))
        {
            previewCharacterAnimator.Play("Locomotion", 0);
        }
    }

    public void SetEmote(int index)
    {
        pcontroller.SetEmote(index, this);

        if (!waitingEmote)
        {
            StartCoroutine(ResetEmote());
        }
    }

    private IEnumerator ResetEmote()
    {
        waitingEmote = true;
        previewCharacterAnimator.Play(pcontroller.NormalEmote.name, 0);
      //  yield return new WaitForSeconds(previewCharacterAnimator.GetCurrentAnimatorStateInfo(0).length);
        yield return new WaitForSeconds(.5F);
        previewCharacterAnimator.Play("Locomotion", 0);
        waitingEmote = false;

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
        if (!waitingEmote)
        {
            if (previewCharacterAnimator.GetCurrentAnimatorStateInfo(0).IsName("Locomotion"))
            {
                previewCharacterAnimator.Play(pcontroller.WinEmote.name, 0);
            }
            else
            {
                previewCharacterAnimator.Play("Locomotion", 0);
            }
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

    //public void SetSkin(int i)
    //{

       
    //    if(selectedSkin < skins.Count - 1)
    //    {
    //        selectedSkin += i;
    //    }
       
    //    if(selectedSkin < 0)
    //    {
    //        selectedSkin = skins.Count - 1;
    //    }

    //   previewCharRenderer.material = skins[selectedSkin];
    //    skinSyncer.SyncThisPlayerSkin(selectedSkin);
    //}

   public void ScrollSkinR()
    {
        if (selectedSkin < skins.Count - 1)
        {
            selectedSkin += 1;
        }
        else if( selectedSkin == skins.Count - 1)
        {
            selectedSkin = 0;
        }

        previewCharRenderer.material = skins[selectedSkin];
        skinSyncer.SyncThisPlayerSkin(selectedSkin);

    }

    public void ScrollSkinL()
    {
        if (selectedSkin > 0)
        {
            selectedSkin -= 1;
        }
        else
        {
            selectedSkin = skins.Count - 1;
        }

        previewCharRenderer.material = skins[selectedSkin];
        skinSyncer.SyncThisPlayerSkin(selectedSkin);
    }

    public void SecretButton(string s)
    {
        secretCode += s;

       
        if(secretCode.Length > 4)
        {
            secretCode = "";
        }
        else if(secretCode == "pepe")
        {
            skinSyncer.Pepe();
            NotificationManager.instance.NewLocalNotification("FeelsGoodMan.jpg");
            secretCode = "";
        }

    }

    public void ToggleColorUI(bool toggle)
    {
        anim.SetBool("Color", toggle);
        anim.SetBool("SubMenu", toggle);
    }

    public void ToggleMusicUI(bool toggle)
    {
        anim.SetBool("Music", toggle);
        anim.SetBool("SubMenu", toggle);
    }

    public void ToggleEmoteUI(bool toggle)
    {
        anim.SetBool("Emote", toggle);
        anim.SetBool("SubMenu", toggle);
    }

}
