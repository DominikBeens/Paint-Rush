using TMPro;
using UnityEngine;

public class GameTimeTextUpdater : MonoBehaviour
{

    [SerializeField] private TextMeshProUGUI gameTimeText;

    private void Update()
    {
        if (TimeManager.instance)
        {
            gameTimeText.text = TimeManager.instance.GetFormattedGameTime();
        }
    }
}
