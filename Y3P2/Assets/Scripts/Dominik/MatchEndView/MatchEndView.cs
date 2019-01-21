using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class MatchEndView : MonoBehaviour
{

    private bool viewActive;
    private List<MatchEndViewPlayerPos> playerPositions = new List<MatchEndViewPlayerPos>();

    [SerializeField] private Transform playerPositionsParent;
    [SerializeField] private GameObject matchEndCam;
    [SerializeField] private GameObject countdownObject;
    [SerializeField] private TextMeshProUGUI countdownText;

    private void Awake()
    {
        for (int i = 0; i < playerPositionsParent.childCount; i++)
        {
            playerPositions.Add(playerPositionsParent.GetChild(i).GetComponent<MatchEndViewPlayerPos>());
        }

        matchEndCam.SetActive(false);
        countdownObject.SetActive(false);
        ToggleAllPlayerPositions(false);

        TimeManager.OnEndMatch += TimeManager_OnEndMatch;
        TimeManager.OnGameTimeStateChanged += TimeManager_OnGameTimeStateChanged;
    }

    private void Update()
    {
        if (countdownText)
        {
            countdownText.text = TimeManager.countdownTime.ToString("F2");
        }
    }

    private void TimeManager_OnEndMatch()
    {
        viewActive = true;
        countdownObject.SetActive(true);
        SetupPlayerPositions();

        if (IsCorrectGameState())
        {
            matchEndCam.SetActive(true);
        }
    }

    private void TimeManager_OnGameTimeStateChanged(TimeManager.GameTimeState newState)
    {
        // Returning to lobby.
        if (viewActive)
        {
            viewActive = false;
            matchEndCam.SetActive(false);
            countdownObject.SetActive(false);
            ToggleAllPlayerPositions(false);
            PlayerManager.instance.playerAnimController.ToggleWinEmote(false);

            if (PlayerManager.instance.playerAudioManager.IsPlayingMusic)
            {
                PlayerManager.instance.playerAudioManager.PlayWinMusic(false);
            }

            if (IsCorrectGameState())
            {
                GameManager.CurrentGameSate = GameManager.GameState.Lobby;
            }
        }
    }

    private void SetupPlayerPositions()
    {
        List<ScoreboardManager.PlayerScore> scores = ScoreboardManager.instance.GetSortedPlayerScores();

        for (int i = 0; i < scores.Count; i++)
        {
            PlayerManager player = Photon.Pun.PhotonView.Find(scores[i].playerPhotonViewID).GetComponent<PlayerManager>();

            if (IsCorrectPlayerState(player))
            {
                TogglePlayerPosition(i, true);
                playerPositions[i].SetupVisual(scores[i]);
                player.entity.paintController.ToggleUI(false);

                if (scores[i].playerPhotonViewID == PlayerManager.instance.photonView.ViewID)
                {
                    PlayerManager.instance.Teleport(playerPositions[i].transform.position);
                    PlayerManager.instance.playerAnimController.ToggleWinEmote(true);

                    // If we're the one with the highest score.
                    if (i == 0)
                    {
                        PlayerManager.instance.playerAudioManager.PlayWinMusic(true);
                    }
                }
            }
        }
    }

    private void ToggleAllPlayerPositions(bool toggle)
    {
        for (int i = 0; i < playerPositions.Count; i++)
        {
            playerPositions[i].ToggleVisuals(toggle);
        }
    }

    private void TogglePlayerPosition(int index, bool toggle)
    {
        playerPositions[index].ToggleVisuals(toggle);
    }

    private bool IsCorrectGameState()
    {
        return GameManager.CurrentGameSate == GameManager.GameState.Playing || GameManager.CurrentGameSate == GameManager.GameState.Respawning;
    }

    private bool IsCorrectPlayerState(PlayerManager player)
    {
        return player.PlayerState == GameManager.GameState.Playing || player.PlayerState == GameManager.GameState.Respawning;
    }

    private void OnDisable()
    {
        TimeManager.OnEndMatch -= TimeManager_OnEndMatch;
        TimeManager.OnGameTimeStateChanged -= TimeManager_OnGameTimeStateChanged;
    }
}
