using System.Collections.Generic;
using UnityEngine;
using TMPro;

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

        public bool HasOpenUI()
        {
            return settingsPanel.activeInHierarchy;
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
            dropdown_VSync.value = QualitySettings.vSyncCount;

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

            //List<string> antiAliasingOptions = new List<string>(QualitySettings.antiAliasing);

            List<string> antiAliasingOptions = new List<string>
        {
            "Off",
            "2x MSAA",
            "4x MSAA",
            "8x MSAA"
        };
            dropdown_AntiAliasing.AddOptions(antiAliasingOptions);
            dropdown_AntiAliasing.value = QualitySettings.antiAliasing;

            dropdown_AntiAliasing.onValueChanged.AddListener((int i) => QualitySettings.antiAliasing = i);
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
            dropdown_ShadowQuality.value = (int)QualitySettings.shadowResolution;

            dropdown_ShadowQuality.onValueChanged.AddListener((int i) => OnShadowQualityValueChanged(i));
        }

        private void OnShadowQualityValueChanged(int i)
        {
            // Reverse i because shadowQualityOptions in SetupShadowQualityDropdown() gets reversed so that the highest quality is at the top of the dropdown menu.
            switch (i)
            {
                case 0:

                    i = 3;
                    break;
                case 1:

                    i = 2;
                    break;
                case 2:

                    i = 1;
                    break;
                case 3:

                    i = 0;
                    break;
            }

            QualitySettings.shadowResolution = (ShadowResolution)i;
            print(QualitySettings.shadowResolution);
        }
    }
}
