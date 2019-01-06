using UnityEngine;
using TMPro;

namespace DB.MenuPack
{
    public class Setting : MonoBehaviour
    {

        [SerializeField] private string settingTitle;
        [SerializeField] private TextMeshProUGUI settingTitleText;

        public virtual void Init()
        {
            settingTitleText.text = settingTitle;
        }

        // Set this object's name to match the setting title.
        private void OnValidate()
        {
            if (string.IsNullOrEmpty(settingTitle))
            {
                if (gameObject.name != "Setting")
                {
                    gameObject.name = "Setting";
                }
            }
            else
            {
                if (gameObject.name != "Setting " + settingTitle)
                {
                    gameObject.name = "Setting " + settingTitle;
                }
            }
        }
    }
}
