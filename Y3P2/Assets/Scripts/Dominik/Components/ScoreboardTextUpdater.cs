using UnityEngine;
using TMPro;

public class ScoreboardTextUpdater : MonoBehaviour
{

    [SerializeField] private TextMeshProUGUI text;

    private void Awake()
    {
        ScoreboardManager.OnScoreboardUpdated += ScoreboardManager_OnScoreboardUpdated;
    }

    private void ScoreboardManager_OnScoreboardUpdated()
    {
        ScoreboardManager.instance.GetScoreBoardToText(text);
    }

    private void OnDisable()
    {
        ScoreboardManager.OnScoreboardUpdated -= ScoreboardManager_OnScoreboardUpdated;
    }
}
