using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PaintUILocalPlayer : MonoBehaviour
{

    private PaintController.PaintType myPaintType;
    private float paintFillAmount;
    private float paintFillTextValue;

    [SerializeField] private Image colorImage;
    [SerializeField] private TextMeshProUGUI paintFillText;
    [SerializeField] private float textUpdateSpeed = 10f;

    public void Initialise(PaintController.PaintValue paintValue)
    {
        myPaintType = paintValue.paintType;
        paintFillAmount = 0;
        colorImage.color = paintValue.paintColor;

        paintFillTextValue = 20;

        PlayerManager.instance.entity.paintController.OnPaintValueModified += PaintController_OnPaintValueModified;
        PlayerManager.instance.entity.paintController.OnPaintValueReset += PaintController_OnPaintValueReset;
    }

    private void Update()
    {
        if (paintFillTextValue != paintFillAmount)
        {
            paintFillTextValue += paintFillAmount > paintFillTextValue ? Time.deltaTime * textUpdateSpeed : -Time.deltaTime * textUpdateSpeed;

            if (Mathf.Abs(paintFillTextValue - paintFillAmount) < 1f)
            {
                paintFillTextValue = paintFillAmount;
            }

            paintFillText.text = paintFillTextValue.ToString("F0") + "%";
        }
    }

    private void PaintController_OnPaintValueModified(PaintController.PaintType paintType, float amount)
    {
        if (amount <= 0 || paintType != myPaintType || PlayerManager.instance.entity.paintController.CurrentPaintMark != null)
        {
            return;
        }

        paintFillAmount += amount;
    }

    private void PaintController_OnPaintValueReset(PaintController.PaintType paintType)
    {
        if (paintType == myPaintType)
        {
            paintFillAmount = 0;
        }
    }

    private void OnDisable()
    {
        PlayerManager.instance.entity.paintController.OnPaintValueModified -= PaintController_OnPaintValueModified;
        PlayerManager.instance.entity.paintController.OnPaintValueReset -= PaintController_OnPaintValueReset;
    }
}
