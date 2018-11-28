using UnityEngine;
using UnityEngine.UI;

public class MarkCapturePoint : MonoBehaviour
{

    private CapturingPlayer capturingPlayer;
    private float captureProgress;

    [SerializeField] private float captureDuration = 3f;
    [SerializeField] private GameObject canvas;
    [SerializeField] private Image captureProgressFill;

    public class CapturingPlayer
    {
        public Collider colCapturing;
        public Entity entityCapturing;
    }

    private void Awake()
    {
        canvas.SetActive(false);
    }

    private void StartCapturing(CapturingPlayer player)
    {
        canvas.SetActive(true);
        captureProgress = 0;
        capturingPlayer = player;
    }

    private void StopCapturing(CapturingPlayer player)
    {
        canvas.SetActive(false);
        capturingPlayer = null;
    }

    private void FinishCapturing()
    {
        capturingPlayer.entityCapturing.photonView.RPC("SyncCaptureMark", Photon.Pun.RpcTarget.All);
        StopCapturing(capturingPlayer);
    }

    private void Update()
    {
        if (capturingPlayer != null)
        {
            captureProgress += Time.deltaTime;
            captureProgressFill.fillAmount = captureProgress / captureDuration;

            if (capturingPlayer.entityCapturing.paintController.CurrentPaintMark == null)
            {
                StopCapturing(capturingPlayer);
            }

            if (captureProgress > captureDuration)
            {
                FinishCapturing();
            }
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.transform.root.tag == "Player")
        {
            Entity entity = other.transform.root.GetComponentInChildren<Entity>();
            if (entity && entity == PlayerManager.instance.entity)
            {
                if (PlayerManager.instance.entity.paintController.CurrentPaintMark != null)
                {
                    StartCapturing(new CapturingPlayer { colCapturing = other, entityCapturing = entity });
                }
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (capturingPlayer != null && other == capturingPlayer.colCapturing)
        {
            StopCapturing(capturingPlayer);
        }
    }
}
