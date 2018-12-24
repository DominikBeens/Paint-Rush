using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class GameStatusPanel : MonoBehaviour
{

    [SerializeField] private List<GameTimeStateText> gameTimeStateInfoPanels = new List<GameTimeStateText>();
    [SerializeField] private TextMeshProUGUI countdownText;

    [System.Serializable]
    public struct GameTimeStateText
    {
        public GameObject panel;
        public TimeManager.GameTimeState state;
    }

    private void Awake()
    {
        TimeManager.OnGameTimeStateChanged += TimeManager_OnGameTimeStateChanged;
    }

    private void Update()
    {
        if (TimeManager.countdownTime > 0)
        {
            if (countdownText)
            {
                countdownText.text = TimeManager.countdownTime.ToString("F2");
            }
        }
        else
        {
            if (countdownText && !string.IsNullOrEmpty(countdownText.text))
            {
                countdownText.text = "";
            }
        }
    }

    private void TimeManager_OnGameTimeStateChanged(TimeManager.GameTimeState newState)
    {
        for (int i = 0; i < gameTimeStateInfoPanels.Count; i++)
        {
            gameTimeStateInfoPanels[i].panel.SetActive(gameTimeStateInfoPanels[i].state == newState ? true : false);
        }
    }

    public void StartMatchButton()
    {
        TimeManager.instance.StartMatchButton();
    }

    public void EnterArenaButton()
    {
        TimeManager.instance.EnterArenaButton();
    }

    private void OnDisable()
    {
        TimeManager.OnGameTimeStateChanged -= TimeManager_OnGameTimeStateChanged;
    }
}
