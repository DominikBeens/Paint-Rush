using UnityEngine;
using System;

public class PortalTeleporter : MonoBehaviour
{

    private const int FOV_KICK = 120;

    private Camera mainCam;
    private Transform connectedPortalCam;

    public static event Action OnUsePortal = delegate { };

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
            OnUsePortal();

            PlayerManager.instance.Teleport(new Vector3(connectedPortalCam.position.x, PlayerManager.localPlayer.position.y, connectedPortalCam.position.z));
            mainCam.fieldOfView = FOV_KICK;
        }
    }
}
