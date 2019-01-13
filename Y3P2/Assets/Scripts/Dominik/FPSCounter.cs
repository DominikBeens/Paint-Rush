using UnityEngine;

public class FPSCounter
{

    private const float REFRESH_TIME = 0.5f;

    private int frameCount;
    private float timeCount;

    private float frameRate;
    public float FrameRate { get { return frameRate; } }

    public void Update()
    {
        if (timeCount < REFRESH_TIME)
        {
            timeCount += Time.deltaTime;
            frameCount++;
        }
        else
        {
            frameRate = (float)frameCount / timeCount;
            frameCount = 0;
            timeCount = 0.0f;
        }
    }
}
