using System.Collections.Generic;
using UnityEngine;

public class GameStateBasedEnable : MonoBehaviour
{

    [SerializeField] private List<Behaviour> componentsToToggle = new List<Behaviour>();
    [SerializeField] private List<ToggleSetting> settings = new List<ToggleSetting>();

    [System.Serializable]
    private struct ToggleSetting
    {
        public GameManager.GameState state;
        public bool toggle;
    }

    private void Awake()
    {
        GameManager.OnGameStateChanged += Toggle;
    }

    private void Toggle(GameManager.GameState newState)
    {
        for (int i = 0; i < settings.Count; i++)
        {
            if (settings[i].state == newState)
            {
                for (int ii = 0; ii < componentsToToggle.Count; ii++)
                {
                    componentsToToggle[ii].enabled = settings[i].toggle;
                }

                return;
            }
        }
    }

    private void OnDisable()
    {
        GameManager.OnGameStateChanged -= Toggle;
    }
}
