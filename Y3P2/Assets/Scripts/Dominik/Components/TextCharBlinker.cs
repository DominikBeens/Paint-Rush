using TMPro;
using UnityEngine;

public class TextCharBlinker : MonoBehaviour
{

    private TextMeshProUGUI text;
    private float nextBlinkTime;
    private bool add = true;

    [SerializeField] private string character = ".";
    [SerializeField] private float blinkSpeed = 1f;

    private void Awake()
    {
        text = GetComponent<TextMeshProUGUI>();
    }

    private void Update()
    {
        if (isActiveAndEnabled)
        {
            if (Time.time >= nextBlinkTime)
            {
                nextBlinkTime = Time.time + blinkSpeed;

                text.text = add ? text.text + character : text.text.Substring(0, text.text.Length - 1);
                add = !add;
            }
        }
    }
}
