using UnityEngine;
using Photon.Pun;

public class PlayerAudioManager : MonoBehaviourPunCallbacks
{

    private AudioSource source;
    private CustomizationTerminal terminal;
    private bool isPlayingMusic;
    public bool IsPlayingMusic { get { return isPlayingMusic; } }

    private AudioClip winMusic;
    public AudioClip WinMusic { get { return winMusic; } }

    private int currentWinMusicIndex;

	private void Awake ()
    {
        terminal = FindObjectOfType<CustomizationTerminal>();
        source = GetComponent<AudioSource>();
        source.loop = true;
	}

    public void SetWinMusic(AudioClip music, int index)
    {
        winMusic = music;
        currentWinMusicIndex = index;

        if (source != null)
        {
            source.clip = winMusic;
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
}
