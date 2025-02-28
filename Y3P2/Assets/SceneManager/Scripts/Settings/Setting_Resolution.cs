﻿using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using TMPro;
using System;

namespace DB.MenuPack
{
    public class Setting_Resolution : Setting
    {

        private Resolution[] availableResolutions;
        [SerializeField] private TMP_Dropdown resolutionDropdown;

        public static event Action OnResolutionChanged = delegate { };

        public override void Init()
        {
            base.Init();

            // Get all available resolutions and filter out the duplicates. Also reverse it because we want to show the highest resolution first and convert it to an array.
            availableResolutions = Screen.resolutions.Select(resolution => new Resolution { width = resolution.width, height = resolution.height }).Distinct().Reverse().ToArray();
            List<string> resolutionDropdownOptions = new List<string>();
            // Get our default dropdown value by finding the index of our current resolution in the array of available resolutions.
            int currentResolutionDropdownIndex = Array.FindIndex(availableResolutions, i => i.width == Screen.width && i.height == Screen.height);

            for (int i = 0; i < availableResolutions.Length; i++)
            {
                resolutionDropdownOptions.Add(availableResolutions[i].width + "x" + availableResolutions[i].height);
            }

            resolutionDropdown.ClearOptions();
            resolutionDropdown.AddOptions(resolutionDropdownOptions);
            resolutionDropdown.value = currentResolutionDropdownIndex;
            resolutionDropdown.onValueChanged.AddListener(ChangeSetting);
        }

        private void ChangeSetting(int setting)
        {
            Resolution newResolution = availableResolutions[setting];

            if (newResolution.width != Screen.width && newResolution.height != Screen.height)
            {
                Screen.SetResolution(newResolution.width, newResolution.height, Screen.fullScreenMode);
                OnResolutionChanged();
            }
        }
    }
}
