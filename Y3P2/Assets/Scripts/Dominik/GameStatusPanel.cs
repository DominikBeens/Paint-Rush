using System.Collections.Generic;
using UnityEngine;

public class GameStatusPanel : MonoBehaviour
{

    [SerializeField] private List<GameTimeStateText> gameTimeStateInfoPanels = new List<GameTimeStateText>();

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

    private void TimeManager_OnGameTimeStateChanged(TimeManager.GameTimeState newState)
    {
        for (int i = 0; i < gameTimeStateInfoPanels.Count; i++)
        {
            gameTimeStateInfoPanels[i].panel.SetActive(gameTimeStateInfoPanels[i].state == newState ? true : false);
        }
    }

    private void OnDisable()
    {
        TimeManager.OnGameTimeStateChanged -= TimeManager_OnGameTimeStateChanged;
    }
}
