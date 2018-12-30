using UnityEngine;

public class LookAtCamera : MonoBehaviour
{

    private Transform target;
    private Vector3 targetFixedPos;

    private Transform mainCam;
    private Transform respawnCam;

    [SerializeField] private bool lockY = true;

    private void Start()
    {
        mainCam = Camera.main.transform;
        respawnCam = FindObjectOfType<RespawnCam>().RespawnCamObject.transform;
    }

    private void LateUpdate()
    {
        target = GameManager.CurrentGameSate == GameManager.GameState.Playing ? mainCam : respawnCam;

        if (!target || GameManager.CurrentGameSate == GameManager.GameState.Lobby)
        {
            return;
        }

        targetFixedPos = target.position;
        if (lockY)
        {
            targetFixedPos.y = transform.position.y;
        }

        transform.LookAt(targetFixedPos);
        transform.Rotate(0, 180, 0);
    }
}
