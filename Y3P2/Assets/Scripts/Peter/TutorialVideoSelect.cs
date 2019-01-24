using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Video;

public class TutorialVideoSelect : MonoBehaviour {


    private int selected;

    [SerializeField]
    private List<VideoClip> videoClips = new List<VideoClip>();

    [SerializeField]
    private VideoPlayer vidPlayer;
	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    public void ScrollR()
    {
        if (selected < videoClips.Count - 1)
        {
            selected += 1;
        }
        else if (selected == videoClips.Count - 1)
        {
            selected = 0;
        }

        vidPlayer.clip = videoClips[selected];
    }

    public void ScrollL()
    {
        if (selected > 0)
        {
            selected -= 1;
        }
        else
        {
            selected = videoClips.Count - 1;
        }

        vidPlayer.clip = videoClips[selected];

    }
}
