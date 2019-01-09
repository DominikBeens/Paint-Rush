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
    private AudioSource terminalAudioSource;

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

    public void SetNameColor(Image value)
    {
        GameManager.personalColor = value.color;
        GameManager.personalColorString = ColorUtility.ToHtmlStringRGBA(value.color);
        nameText.color = value.color;
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
        paudio.SetWinMusic(music[index]);
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

}
