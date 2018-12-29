using TMPro;
using UnityEngine;

public class MatchEndViewPlayerPos : MonoBehaviour
{

    [SerializeField] private TextMeshProUGUI playerNameText;
    [SerializeField] private TextMeshProUGUI playerScoreText;

    public void SetupVisual(ScoreboardManager.PlayerScore playerScore)
    {
        playerNameText.text = playerScore.playerName;
        playerScoreText.text = playerScore.playerGamePoints.ToString();
    }

    public void ToggleVisuals(bool toggle)
    {
        for (int i = 0; i < transform.childCount; i++)
        {
            transform.GetChild(i).gameObject.SetActive(toggle);
        }
    }
}
