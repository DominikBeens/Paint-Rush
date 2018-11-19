using UnityEngine;
using TMPro;

namespace DB.MenuPack
{
    public class Setting : MonoBehaviour
    {

        [SerializeField] private string settingTitle;
        [SerializeField] private TextMeshProUGUI settingTitleText;

        public virtual void Awake()
        {
            settingTitleText.text = settingTitle;
        }
    }
}
