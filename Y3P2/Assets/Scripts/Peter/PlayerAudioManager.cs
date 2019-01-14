using UnityEngine;
using Photon.Pun;
using System.Collections.Generic;

public class PlayerAudioManager : MonoBehaviourPunCallbacks
{

    private AudioSource source;
    private CustomizationTerminal terminal;
    private bool isPlayingMusic;
    public bool IsPlayingMusic { get { return isPlayingMusic; } }

    private AudioClip winMusic;
    public AudioClip WinMusic { get { return winMusic; } }

    private int currentWinMusicIndex;

    [SerializeField] private List<PlayableClip> playableAudioClips = new List<PlayableClip>();

    [System.Serializable]
    public struct PlayableClip
    {
        public AudioClip clip;
        public string trigger;
    }

	private void Awake ()
    {
        terminal = FindObjectOfType<CustomizationTerminal>();
        source = GetComponent<AudioSource>();
        source.loop = true;

        WeaponSlot.OnChangeAmmoType += WeaponSlot_OnChangeAmmoType;
	}

    public AudioClip GetClip(string trigger)
    {
        for (int i = 0; i < playableAudioClips.Count; i++)
        {
            if (playableAudioClips[i].trigger == trigger)
            {
                return playableAudioClips[i].clip;
            }
        }

        return null;
    }

    public void PlayClipOnce(AudioClip clip)
    {
        if (!clip || isPlayingMusic)
        {
            return;
        }

        source.loop = false;
        source.clip = clip;
        source.Play();
    }

    private void WeaponSlot_OnChangeAmmoType(Color color)
    {
        PlayClipOnce(GetClip("change_ammo"));
    }

    public void SetWinMusic(AudioClip music, int index)
    {
        winMusic = music;
        currentWinMusicIndex = index;

        if (source != null)
        {
            source.clip = winMusic;
            source.loop = true;
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
            source.Play();
            isPlayingMusic = true;
        }
        else
        {
            source.Stop();
            isPlayingMusic = false;
        }
    }

    public override void OnDisable()
    {
        WeaponSlot.OnChangeAmmoType -= WeaponSlot_OnChangeAmmoType;
    }
}
