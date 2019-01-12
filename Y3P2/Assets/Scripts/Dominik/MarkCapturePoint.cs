using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class MarkCapturePoint : MonoBehaviour
{

    private bool isActive;
    public bool IsActive { get { return isActive; } }
    private CapturingPlayer capturingPlayer;
    private float captureProgress;
    private bool indicatorActive;

    [Header("Unique ID")]
    [SerializeField] private int capturePointID;
    public int CapturePointID { get { return capturePointID; } }

    [Space]

    [SerializeField] private float captureDuration = 3f;
    [SerializeField] private Canvas catureCanvas;
    [SerializeField] private Image captureProgressFill;
    [SerializeField] private TextMeshProUGUI capturePercentageText;
    [Range(0, 100)] [SerializeField] private float hitProgressLoss = 15f;
    [SerializeField] private Animator captureUIAnim;
    [SerializeField] private GameObject effects;
    [SerializeField] private Canvas captureIndicatorCanvas;

    public class CapturingPlayer
    {
        public Collider colCapturing;
        public Entity entityCapturing;
    }

    private void Awake()
    {
        catureCanvas.enabled = false;
        captureIndicatorCanvas.enabled = false;
    }

    public void Init()
    {
        MarkCapturePointManager.OnCapturePointChanged += ToggleCapturePointAccesibility;

        PlayerManager.instance.entity.paintController.OnPaintValueModified += PaintController_OnPaintValueModified;
        PlayerManager.instance.entity.paintController.OnPaintMarkActivated += PaintController_OnPaintMarkActivated;
        PlayerManager.instance.entity.paintController.OnPaintMarkDestroyed += PaintController_OnPaintMarkDestroyed;
    }

    private void PaintController_OnPaintValueModified(PaintController.PaintType paintType, float amount)
    {
        if (capturingPlayer != null && amount > 0)
        {
            captureProgress -= (hitProgressLoss / 100 * captureDuration);
            captureProgress = Mathf.Clamp(captureProgress, 0, captureDuration);
            captureUIAnim.SetTrigger("Hit");
        }
    }

    private void PaintController_OnPaintMarkActivated(PaintController.PaintMark mark)
    {
        indicatorActive = true;
        captureIndicatorCanvas.enabled = isActive;
    }

    private void PaintController_OnPaintMarkDestroyed()
    {
        indicatorActive = false;
        captureIndicatorCanvas.enabled = false;
    }

    private void StartCapturing(CapturingPlayer player)
    {
        catureCanvas.enabled = true;
        captureProgress = 0;
        capturingPlayer = player;
        captureIndicatorCanvas.enabled = !indicatorActive;
    }

    private void StopCapturing(CapturingPlayer player)
    {
        catureCanvas.enabled = false;
        capturingPlayer = null;
        captureIndicatorCanvas.enabled = indicatorActive;
    }

    private void FinishCapturing()
    {
        capturingPlayer.entityCapturing.photonView.RPC("SyncCaptureMark", Photon.Pun.RpcTarget.All, transform.localPosition);
        StopCapturing(capturingPlayer);
        MarkCapturePointManager.instance.PointCaptured();
    }

    private void Update()
    {
        if (capturingPlayer != null)
        {
            captureProgress += Time.deltaTime;
            captureProgressFill.fillAmount = captureProgress / captureDuration;
            capturePercentageText.text = (captureProgressFill.fillAmount * 100).ToString("F0") + "%";

            if (capturingPlayer.entityCapturing.paintController.CurrentPaintMark == null || !isActive)
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
        if (!isActive)
        {
            return;
        }

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

    public void ToggleCapturePointAccesibility(MarkCapturePoint newPoint)
    {
        isActive = newPoint == this;
        effects.SetActive(isActive);
        captureIndicatorCanvas.enabled = isActive && indicatorActive;
    }

    private void OnDisable()
    {
        MarkCapturePointManager.OnCapturePointChanged -= ToggleCapturePointAccesibility;

        PlayerManager.instance.entity.paintController.OnPaintValueModified -= PaintController_OnPaintValueModified;
        PlayerManager.instance.entity.paintController.OnPaintMarkActivated -= PaintController_OnPaintMarkActivated;
        PlayerManager.instance.entity.paintController.OnPaintMarkDestroyed -= PaintController_OnPaintMarkDestroyed;
    }
}
