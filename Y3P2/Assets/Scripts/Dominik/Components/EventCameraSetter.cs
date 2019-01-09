using UnityEngine;

public class EventCameraSetter : MonoBehaviour
{

    private Camera cam;

    private void Start()
    {
        cam = Camera.main;
        if (!cam)
        {
            cam = FindObjectOfType<Camera>();
        }

        if (cam)
        {
            GetComponent<Canvas>().worldCamera = cam;
        }
    }
}
