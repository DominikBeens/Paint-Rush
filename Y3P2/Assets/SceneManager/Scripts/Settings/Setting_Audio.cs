using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.Audio;

namespace DB.MenuPack
{
    public class Setting_Audio : Setting
    {

        [Space(10)]

        [SerializeField] private string audioPrefKey = "AudioMainVolume";

        [SerializeField] private AudioMixer mixer;
        [SerializeField] private string volumeParameter;
        [SerializeField] [Range(0, 100)] private float defaultVolume = 75f;

        [Space(10)]

        [SerializeField] private Slider audioSlider;
        [SerializeField] private TextMeshProUGUI audioPercentageText;

        public override void Init()
        {
            base.Init();

            audioSlider.minValue = 0;
            audioSlider.maxValue = 100;
            audioSlider.value = !string.IsNullOrEmpty(audioPrefKey) && PlayerPrefs.HasKey(audioPrefKey) ? PlayerPrefs.GetFloat(audioPrefKey) : defaultVolume;
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

            if (!string.IsNullOrEmpty(audioPrefKey))
            {
                PlayerPrefs.SetFloat(audioPrefKey, value);
            }
        }
    }
}
