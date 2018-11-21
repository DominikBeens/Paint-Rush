using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class PaintUIBar : MonoBehaviour
{

    private PaintController.PaintType barType;
    public PaintController.PaintType BarType { get { return barType; } }

    [SerializeField] private Image barColorFill;

    public void Initialise(PaintController.PaintValue paintValue)
    {
        barType = paintValue.paintType;
        barColorFill.fillAmount = 0;
        barColorFill.color = paintValue.paintColor;
    }

    public void IncrementBar(float amount)
    {
        float amountFixed = amount / 100;
        float percentage = barColorFill.fillAmount + amountFixed;
        StartCoroutine(LerpBar(percentage));
    }

    public void ResetBar()
    {
        StartCoroutine(LerpBar(0));
    }

    private IEnumerator LerpBar(float percentage)
    {
        float startPercentage = barColorFill.fillAmount;
        float progress = 0f;

        while (progress < 0.2f)
        {
            progress += Time.deltaTime;
            barColorFill.fillAmount = Mathf.Lerp(startPercentage, percentage, progress / 0.2f);
            yield return null;
        }

        barColorFill.fillAmount = percentage;
    }
}
