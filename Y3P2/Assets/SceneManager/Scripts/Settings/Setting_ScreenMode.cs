using System.Collections.Generic;
using UnityEngine;
using TMPro;

namespace DB.MenuPack
{
    public class Setting_ScreenMode : Setting
    {

        [SerializeField] private TMP_Dropdown screenModeDropdown;
        [SerializeField] private List<ScreenMode> screenModes;

        [System.Serializable]
        private struct ScreenMode
        {
            public string name;
            public FullScreenMode mode;
        }

        public override void Init()
        {
            base.Init();

            int currentSettingDropdownIndex = 0;
            List<string> screenModeDropdownSettings = new List<string>();
            for (int i = 0; i < screenModes.Count; i++)
            {
                screenModeDropdownSettings.Add(screenModes[i].name);

                if (screenModes[i].mode == Screen.fullScreenMode)
                {
                    currentSettingDropdownIndex = i;
                }
            }

            screenModeDropdown.ClearOptions();
            screenModeDropdown.AddOptions(screenModeDropdownSettings);
            screenModeDropdown.value = currentSettingDropdownIndex;
            screenModeDropdown.onValueChanged.AddListener(ChangeSetting);
        }

        private void ChangeSetting(int setting)
        {
            FullScreenMode newSetting = screenModes[setting].mode;

            if (newSetting != Screen.fullScreenMode)
            {
                Screen.fullScreenMode = newSetting;

                if (newSetting == FullScreenMode.ExclusiveFullScreen || newSetting == FullScreenMode.FullScreenWindow)
                {
                    Screen.fullScreen = true;
                    Resolution resolution = Screen.currentResolution;
                    Screen.SetResolution(resolution.width, resolution.height, true);
                }
                else
                {
                    Screen.fullScreen = false;
                    Resolution resolution = Screen.currentResolution;
                    Screen.SetResolution(resolution.width, resolution.height, false);
                }
            }
        }
    }
}
