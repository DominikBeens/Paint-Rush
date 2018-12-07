using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PaintUILocalPlayer : MonoBehaviour
{

    private PaintController.PaintType myPaintType;
    private float paintFillAmount;

    [SerializeField] private List<ImageColorData> imageBackgrounds = new List<ImageColorData>();
    [SerializeField] private Image imageFill;

    [System.Serializable]
    public struct ImageColorData
    {
        public Image image;
        [Range(0, 1)] public float alpha;
    }

    public void Initialise(PaintController.PaintValue paintValue)
    {
        myPaintType = paintValue.paintType;

        paintFillAmount = paintValue.paintValue;
        for (int i = 0; i < imageBackgrounds.Count; i++)
        {
            imageBackgrounds[i].image.color = new Color(paintValue.paintColor.r, paintValue.paintColor.g, paintValue.paintColor.b, imageBackgrounds[i].alpha);
        }
        imageFill.color = paintValue.paintColor;
        imageFill.fillAmount = paintFillAmount / 100;

        PlayerManager.instance.entity.paintController.OnPaintValueModified += PaintController_OnPaintValueModified;
        PlayerManager.instance.entity.paintController.OnPaintValueReset += PaintController_OnPaintValueReset;
    }

    private void PaintController_OnPaintValueModified(PaintController.PaintType paintType, float amount)
    {
        if (amount <= 0 || paintType != myPaintType || PlayerManager.instance.entity.paintController.CurrentPaintMark != null)
        {
            return;
        }

        paintFillAmount += amount;
        imageFill.fillAmount = paintFillAmount / 100;
    }

    private void PaintController_OnPaintValueReset(PaintController.PaintType paintType)
    {
        if (paintType == myPaintType)
        {
            paintFillAmount = 0;
            imageFill.fillAmount = 0;
        }
    }

    private void OnDisable()
    {
        PlayerManager.instance.entity.paintController.OnPaintValueModified -= PaintController_OnPaintValueModified;
        PlayerManager.instance.entity.paintController.OnPaintValueReset -= PaintController_OnPaintValueReset;
    }
}
