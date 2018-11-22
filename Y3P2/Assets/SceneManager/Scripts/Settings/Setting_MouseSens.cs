using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace DB.MenuPack
{
    public class Setting_MouseSens : Setting
    {

        public static float mouseSensitivity = 1f;
        [SerializeField] private float minSensitivity = 0.01f;
        [SerializeField] private float maxSensitivity = 1f;
        [SerializeField] private Slider mouseSensSlider;
        [SerializeField] private TextMeshProUGUI mouseSensText;

        public override void Awake()
        {
            base.Awake();

            mouseSensSlider.minValue = minSensitivity;
            mouseSensSlider.maxValue = maxSensitivity;
            mouseSensSlider.value = mouseSensitivity;
            mouseSensSlider.onValueChanged.AddListener(SetMouseSensitivity);

            mouseSensText.text = mouseSensitivity.ToString();
        }

        private void SetMouseSensitivity(float value)
        {
            mouseSensitivity = value;
            mouseSensText.text = mouseSensitivity.ToString("F2");
        }
    }
}