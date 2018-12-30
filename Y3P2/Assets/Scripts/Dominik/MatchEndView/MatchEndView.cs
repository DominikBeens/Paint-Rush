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

        if (GameManager.CurrentGameSate == GameManager.GameState.Playing)
        {
            matchEndCam.SetActive(true);
        }
    }

    private void TimeManager_OnGameTimeStateChanged(TimeManager.GameTimeState newState)
    {
        if (viewActive)
        {
            viewActive = false;
            matchEndCam.SetActive(false);
            countdownObject.SetActive(false);
            ToggleAllPlayerPositions(false);

            if (GameManager.CurrentGameSate != GameManager.GameState.Lobby)
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

            if (player.PlayerState != GameManager.GameState.Lobby)
            {
                TogglePlayerPosition(i, true);
                playerPositions[i].SetupVisual(scores[i]);
                player.entity.paintController.ToggleUI(false);

                if (scores[i].playerPhotonViewID == PlayerManager.instance.photonView.ViewID)
                {
                    PlayerManager.instance.Teleport(playerPositions[i].transform.position);
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

    private void OnDisable()
    {
        TimeManager.OnEndMatch -= TimeManager_OnEndMatch;
        TimeManager.OnGameTimeStateChanged += TimeManager_OnGameTimeStateChanged;
    }
}
