using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class MarkCapturePoint : MonoBehaviour
{

    private CapturingPlayer capturingPlayer;
    private float captureProgress;

    [SerializeField] private float captureDuration = 3f;
    [SerializeField] private GameObject canvas;
    [SerializeField] private Image captureProgressFill;
    [SerializeField] private TextMeshProUGUI capturePercentageText;
    [Range(0, 100)] [SerializeField] private float hitProgressLoss = 15f;

    public class CapturingPlayer
    {
        public Collider colCapturing;
        public Entity entityCapturing;
    }

    private void Awake()
    {
        canvas.SetActive(false);
    }

    private void Start()
    {
        PlayerManager.instance.entity.paintController.OnPaintValueModified += PaintController_OnPaintValueModified;
    }

    private void PaintController_OnPaintValueModified(PaintController.PaintType paintType, float amount)
    {
        if (capturingPlayer != null)
        {
            captureProgress -= (hitProgressLoss / 100);
        }
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
        capturingPlayer.entityCapturing.photonView.RPC("SyncCaptureMark", Photon.Pun.RpcTarget.All, transform.localPosition);
        StopCapturing(capturingPlayer);
    }

    private void Update()
    {
        if (capturingPlayer != null)
        {
            captureProgress += Time.deltaTime;
            captureProgressFill.fillAmount = captureProgress / captureDuration;
            capturePercentageText.text = (captureProgressFill.fillAmount * 100).ToString("F0") + "%";

            if (capturingPlayer.entityCapturing.paintController.CurrentPaintMark == null)
            {
                StopCapturing(capturingPlayer);
            }

            if (captureProgress > captureDuration)
            {
                FinishCapturing();
            }
        }

        if (Input.GetKeyDown(KeyCode.G))
        {
            PlayerManager.instance.entity.photonView.RPC("SyncCaptureMark", Photon.Pun.RpcTarget.All, transform.localPosition);
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

    private void OnDisable()
    {
        PlayerManager.instance.entity.paintController.OnPaintValueModified -= PaintController_OnPaintValueModified;
    }
}
