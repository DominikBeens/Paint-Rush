using System.Collections.Generic;
using TMPro;
using UnityEngine;

namespace DB.MenuPack
{
    public class SettingsManager : MonoBehaviour
    {

        public static SettingsManager instance;

        [SerializeField] private GameObject settingsPanel;
        [SerializeField] private GameObject defaultOpenSettingPanel;
        private GameObject openSettingPanel;

        [Header("Default Settings")]
        [SerializeField] private TMP_Dropdown dropdown_VSync;
        [SerializeField] private TMP_Dropdown dropdown_AntiAliasing;
        [SerializeField] private TMP_Dropdown dropdown_TextureQuality;
        [SerializeField] private TMP_Dropdown dropdown_ShadowQuality;

        private void Awake()
        {
            if (!instance)
            {
                instance = this;
            }
            else if (instance && instance != this)
            {
                Destroy(this);
            }

            SetupDefaultSettings();
            InitSettings();
        }

        private void InitSettings()
        {
            Setting[] settings = GetComponentsInChildren<Setting>(true);
            for (int i = 0; i < settings.Length; i++)
            {
                settings[i].Init();
            }
        }

        public void ToggleSettingsPanel(bool b)
        {
            settingsPanel.SetActive(b);

            if (b && defaultOpenSettingPanel)
            {
                ToggleSettingPanel(defaultOpenSettingPanel);
            }
        }

        public void ToggleSettingPanel(GameObject panel)
        {
            if (openSettingPanel)
            {
                openSettingPanel.SetActive(false);
            }

            panel.SetActive(true);
            openSettingPanel = panel;
        }

        private void SetupDefaultSettings()
        {
            SetupVSyncDropdown();
            SetupAADropdown();
            SetupTextureQualityDropdown();
            SetupShadowQualityDropdown();
        }

        private void SetupVSyncDropdown()
        {
            if (!dropdown_VSync)
            {
                Debug.LogWarning("VSync gamesetting dropdown isn't assigned in the SettingsManager!");
                return;
            }

            dropdown_VSync.ClearOptions();

            List<string> vSyncOptions = new List<string>
            {
                "Off",
                "On"
            };
            dropdown_VSync.AddOptions(vSyncOptions);
            dropdown_VSync.value = 0;

            QualitySettings.vSyncCount = 0;

            dropdown_VSync.onValueChanged.AddListener((int i) => QualitySettings.vSyncCount = i);
        }

        private void SetupAADropdown()
        {
            if (!dropdown_AntiAliasing)
            {
                Debug.LogWarning("Anti-aliasing gamesetting dropdown isn't assigned in the SettingsManager!");
                return;
            }

            dropdown_AntiAliasing.ClearOptions();

            List<string> antiAliasingOptions = new List<string>
            {
                "Off",
                "2x MSAA",
                "4x MSAA",
                "8x MSAA"
            };
            dropdown_AntiAliasing.AddOptions(antiAliasingOptions);

            switch (QualitySettings.antiAliasing)
            {
                case 0:
                    dropdown_AntiAliasing.value = 0;
                    break;

                case 2:
                    dropdown_AntiAliasing.value = 1;
                    break;

                case 4:
                    dropdown_AntiAliasing.value = 2;
                    break;

                case 8:
                    dropdown_AntiAliasing.value = 3;
                    break;
            }

            dropdown_AntiAliasing.onValueChanged.AddListener((int i) => OnAAValueChanged(i));
        }

        private void OnAAValueChanged(int i)
        {
            // Switch i which is dropdown_AntiAliasing.value.
            switch (i)
            {
                case 0:
                    i = 0;
                    break;

                case 1:
                    i = 2;
                    break;

                case 2:
                    i = 4;
                    break;

                case 3:
                    i = 8;
                    break;
            }

            QualitySettings.antiAliasing = i;
        }

        private void SetupTextureQualityDropdown()
        {
            if (!dropdown_TextureQuality)
            {
                Debug.LogWarning("Texture Quality gamesetting dropdown isn't assigned in the SettingsManager!");
                return;
            }

            dropdown_TextureQuality.ClearOptions();

            List<string> textureQualityOptions = new List<string>
            {
                "High",
                "Medium",
                "Low",
                "Very Low"
            };
            dropdown_TextureQuality.AddOptions(textureQualityOptions);
            dropdown_TextureQuality.value = QualitySettings.masterTextureLimit;

            dropdown_TextureQuality.onValueChanged.AddListener((int i) => QualitySettings.masterTextureLimit = i);
        }

        private void SetupShadowQualityDropdown()
        {
            if (!dropdown_ShadowQuality)
            {
                Debug.LogWarning("Shadow Quality gamesetting dropdown isn't assigned in the SettingsManager!");
                return;
            }

            dropdown_ShadowQuality.ClearOptions();

            List<string> shadowQualityOptions = new List<string>
            {
                "Low",
                "Medium",
                "High",
                "Very High"
            };
            shadowQualityOptions.Reverse();
            dropdown_ShadowQuality.AddOptions(shadowQualityOptions);

            // Adjust value because we reversed the shadowQualityOptions list.
            int dropdownValue = Mathf.Abs((int)QualitySettings.shadowResolution - (dropdown_ShadowQuality.options.Count - 1));
            dropdown_ShadowQuality.value = dropdownValue;

            dropdown_ShadowQuality.onValueChanged.AddListener((int i) => OnShadowQualityValueChanged(i));
        }

        private void OnShadowQualityValueChanged(int i)
        {
            // Reverse i because shadowQualityOptions in SetupShadowQualityDropdown() gets reversed so that the highest quality is at the top of the dropdown menu.
            i = Mathf.Abs(i - (dropdown_ShadowQuality.options.Count - 1));

            QualitySettings.shadowResolution = (ShadowResolution)i;
            print(QualitySettings.shadowResolution);
        }
    }
}
