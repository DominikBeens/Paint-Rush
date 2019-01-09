using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAudioManager : MonoBehaviour {

    private AudioSource source;

    private AudioClip winMusic;
    public AudioClip WinMusic { get { return winMusic; } }

	// Use this for initialization
	void Awake () {
        source = GetComponent<AudioSource>();
        source.loop = true;
	}

    public void SetWinMusic(AudioClip music)
    {
        winMusic = music;
        if(source != null)
        {
            source.clip = winMusic;
        }
    }

    public void ToggleWinMusic(bool toggle)
    {
        if (toggle)
        {
            source.Play();
        }
        else
        {
            source.Stop();
        }
    }
}
