using System.Collections.Generic;
using TMPro;
using UnityEngine;

namespace DB.MenuPack
{

    public class Setting_GameStatsDisplay : Setting
    {

        private const string settingPrefKey = "GameStatsDisplayToggle";

        [SerializeField] private TMP_Dropdown dropdown;

        public static bool settingValue;

        public override void Init()
        {
            base.Init();

            int initialValue = PlayerPrefs.HasKey(settingPrefKey) ? PlayerPrefs.GetInt(settingPrefKey) : 0;
            settingValue = initialValue == 0;

            dropdown.ClearOptions();

            List<string> options = new List<string>
            {
                "On",
                "Off"
            };
            dropdown.AddOptions(options);
            dropdown.onValueChanged.AddListener(ChangeSetting);
            dropdown.value = initialValue;
        }

        private void ChangeSetting(int setting)
        {
            if (UIManager.instance)
            {
                UIManager.instance.ToggleGameTechStatsCanvas(setting == 0);
                PlayerPrefs.SetInt(settingPrefKey, setting);
                settingValue = setting == 0;
            }
        }
    }
}
