using TMPro;
using UnityEngine;

public class TextCharBlinker : MonoBehaviour
{

    private TextMeshProUGUI text;
    private float nextBlinkTime;
    private bool add = true;

    [SerializeField] private string character = ".";
    [SerializeField] private float blinkSpeed = 1f;

    //public enum BlinkType { Remove, Replace };
    //[SerializeField] private BlinkType blinkType;

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

                text.text = add ? text.text + character : GetStringWithoutLastChar();
                add = !add;

                //switch (blinkType)
                //{
                //    case BlinkType.Remove:

                //        text.text = add ? text.text + character : GetStringWithoutLastChar();
                //        break;
                //    case BlinkType.Replace:

                //        text.text = add ? GetStringWithoutLastChar() + character : GetStringWithoutLastChar() + " ";
                //        break;
                //}
            }
        }
    }

    private string GetStringWithoutLastChar()
    {
        return text.text.Substring(0, text.text.Length - 1);
    }
}
