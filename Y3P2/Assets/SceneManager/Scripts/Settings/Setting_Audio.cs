using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.Audio;

namespace DB.MenuPack
{
    public class Setting_Audio : Setting
    {

        private const string volumePrefKey = "AudioMainVolume";

        [Space(10)]

        [SerializeField] private AudioMixer mixer;
        [SerializeField] private string volumeParameter;
        [SerializeField] [Range(0, 100)] private float defaultVolume = 75f;

        [Space(10)]

        [SerializeField] private Slider audioSlider;
        [SerializeField] private TextMeshProUGUI audioPercentageText;

        public override void Awake()
        {
            base.Awake();

            audioSlider.minValue = 0;
            audioSlider.maxValue = 100;
            audioSlider.value = PlayerPrefs.HasKey(volumePrefKey) ? PlayerPrefs.GetFloat(volumePrefKey) : defaultVolume;
            audioSlider.onValueChanged.AddListener(SetMixerVolume);

            if (mixer)
            {
                mixer.SetFloat(volumeParameter, (audioSlider.value - 80));
            }
            audioPercentageText.text = Mathf.Round(audioSlider.value) + "%";
        }

        private void SetMixerVolume(float value)
        {
            if (mixer)
            {
                mixer.SetFloat(volumeParameter, (value - 80));
            }
            audioPercentageText.text = Mathf.Round(audioSlider.value) + "%";

            PlayerPrefs.SetFloat(volumePrefKey, value);
        }
    }
}
