using UnityEngine;

public class ScaleWithDistance : MonoBehaviour
{

    private Transform target;
    private Vector3 targetFixedPos;
    private float originalSize;

    private Transform mainCam;
    private Transform respawnCam;

    [SerializeField] private float scaleModifier = 1f;

    private void Start()
    {
        mainCam = Camera.main.transform;
        respawnCam = FindObjectOfType<RespawnCam>().RespawnCamObject.transform;

        originalSize = transform.localScale.x;
    }

    private void LateUpdate()
    {
        target = GameManager.CurrentGameSate == GameManager.GameState.Playing ? mainCam : respawnCam;

        if (!target)
        {
            return;
        }

        Vector3 directionToTarget = target.position - transform.position;
        float dSqrToTarget = directionToTarget.sqrMagnitude;

        transform.localScale = new Vector3(Mathf.Clamp((dSqrToTarget * scaleModifier / 1000), originalSize, 100), Mathf.Clamp((dSqrToTarget * scaleModifier / 1000), originalSize, 100), 1);
    }
}
