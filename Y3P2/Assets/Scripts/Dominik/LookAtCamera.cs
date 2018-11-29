using UnityEngine;

public class LookAtCamera : MonoBehaviour
{

    private Transform target;
    private Vector3 targetFixedPos;

    private Transform mainCam;
    private Transform respawnCam;

    private void Start()
    {
        mainCam = Camera.main.transform;
        respawnCam = FindObjectOfType<RespawnCam>().RespawnCamObject.transform;
    }

    private void LateUpdate()
    {
        target = GameManager.CurrentGameSate == GameManager.GameState.Playing ? mainCam : respawnCam;

        if (!target)
        {
            return;
        }

        targetFixedPos = target.position;
        targetFixedPos.y = transform.position.y;

        transform.LookAt(targetFixedPos);
        transform.Rotate(0, 180, 0);
    }
}
