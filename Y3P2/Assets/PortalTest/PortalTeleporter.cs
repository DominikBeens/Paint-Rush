using UnityEngine;

public class PortalTeleporter : MonoBehaviour
{

    private const float FOV_KICK = 100f;

    private Camera mainCam;
    private Transform connectedPortalCam;

    public void Init(Transform connectedPortalCam)
    {
        mainCam = Camera.main;
        this.connectedPortalCam = connectedPortalCam;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!connectedPortalCam)
        {
            return;
        }

        if (other.transform.root == PlayerManager.localPlayer)
        {
            PlayerManager.localPlayer.position = connectedPortalCam.position;
            mainCam.fieldOfView = FOV_KICK;
        }
    }
}
