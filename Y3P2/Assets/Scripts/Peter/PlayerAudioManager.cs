using UnityEngine;
using Photon.Pun;
using System.Collections.Generic;

public class PlayerAudioManager : MonoBehaviourPunCallbacks
{

    private CustomizationTerminal terminal;
    private bool isPlayingMusic;
    public bool IsPlayingMusic { get { return isPlayingMusic; } }
    private float defaultVolume;

    private AudioClip winMusic;
    public AudioClip WinMusic { get { return winMusic; } }

    private int currentWinMusicIndex;

    [SerializeField] private AudioSource mainSource;
    [SerializeField] private AudioSource secondarySource;

    [Space]

    [SerializeField] private List<PlayableClip> playableAudioClips = new List<PlayableClip>();

    [System.Serializable]
    public struct PlayableClip
    {
        public string trigger;
        public AudioClip clip;

        [Space]

        public bool overrideVolume;
        [Range(0, 1)] public float volume;

        [Space]

        public bool useSecondarySource;
    }

	private void Awake ()
    {
        terminal = FindObjectOfType<CustomizationTerminal>();
        mainSource.loop = true;
        defaultVolume = mainSource.volume;

        WeaponSlot.OnFireWeapon += WeaponSlot_OnFireWeapon;
        WeaponSlot.OnHitEntity += WeaponSlot_OnHitEntity;
        WeaponSlot.OnChangeAmmoType += WeaponSlot_OnChangeAmmoType;

        PlayerManager.instance.playerController.OnJump += PlayerController_OnJump;
	}

    public PlayableClip GetClip(string trigger)
    {
        for (int i = 0; i < playableAudioClips.Count; i++)
        {
            if (playableAudioClips[i].trigger == trigger)
            {
                return playableAudioClips[i];
            }
        }

        Debug.LogWarning("Couldn't find clip.");
        return new PlayableClip();
    }

    public void PlayClipOnce(PlayableClip playable)
    {
        if (!playable.clip || isPlayingMusic)
        {
            return;
        }

        AudioSource source = playable.useSecondarySource ? secondarySource : mainSource;

        source.loop = false;
        source.clip = playable.clip;
        source.volume = playable.overrideVolume ? playable.volume : defaultVolume;
        source.Play();
    }

    private void WeaponSlot_OnFireWeapon()
    {
        AudioController audioController = ObjectPooler.instance.GrabFromPool("Audio_GunFire", transform.position, Quaternion.identity).GetComponent<AudioController>();
        audioController.Play(transform);
    }

    private void WeaponSlot_OnHitEntity()
    {
        PlayClipOnce(GetClip("hitmark"));
    }

    private void WeaponSlot_OnChangeAmmoType(Color color)
    {
        PlayClipOnce(GetClip("change_ammo"));
    }

    private void PlayerController_OnJump()
    {
        PlayClipOnce(GetClip("jump"));
    }

    public void SetWinMusic(AudioClip music, int index)
    {
        winMusic = music;
        currentWinMusicIndex = index;

        if (mainSource != null)
        {
            mainSource.clip = winMusic;
            mainSource.loop = true;
        }
    }

    public void PlayWinMusic(bool toggle)
    {
        photonView.RPC("ToggleWinMusic", RpcTarget.All, toggle, currentWinMusicIndex);
    }

    [PunRPC]
    private void ToggleWinMusic(bool toggle, int musicIndex)
    {
        SetWinMusic(terminal.Music[musicIndex], musicIndex);

        if (toggle)
        {
            mainSource.Play();
            isPlayingMusic = true;
        }
        else
        {
            mainSource.Stop();
            isPlayingMusic = false;
        }
    }

    public override void OnDisable()
    {
        WeaponSlot.OnFireWeapon -= WeaponSlot_OnFireWeapon;
        WeaponSlot.OnHitEntity -= WeaponSlot_OnHitEntity;
        WeaponSlot.OnChangeAmmoType -= WeaponSlot_OnChangeAmmoType;

        PlayerManager.instance.playerController.OnJump -= PlayerController_OnJump;
    }
}
