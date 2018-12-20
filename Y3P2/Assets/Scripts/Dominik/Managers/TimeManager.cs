using Photon.Pun;
using Photon.Realtime;
using System;
using UnityEngine;

public class TimeManager : MonoBehaviourPunCallbacks
{

    public static TimeManager instance;

    public const int GAME_TIME_IN_SECONDS = 600;

    private static bool gameInProgress;
    private static float currentGameTime;

    public event Action OnStartGame = delegate { };
    public event Action OnEndGame = delegate { };

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(this);
        }
    }

    private void Update()
    {
        if (gameInProgress)
        {
            currentGameTime -= Time.deltaTime;

            if (currentGameTime <= 0)
            {
                EndGame();
            }
        }

        if (Input.GetKeyDown(KeyCode.N))
        {
            StartGame();
        }
        if (Input.GetKeyDown(KeyCode.M))
        {
            EndGame();
        }
    }

    private void StartGame()
    {
        OnStartGame();

        NotificationManager.instance.NewNotification("<color=red>Game Started!");

        currentGameTime = GAME_TIME_IN_SECONDS;
        gameInProgress = true;
    }

    private void EndGame()
    {
        OnEndGame();

        NotificationManager.instance.NewNotification("<color=red>Game Ended!");

        currentGameTime = 0;
        gameInProgress = false;
    }

    public string GetFormattedGameTime()
    {
        string minutes = Mathf.Floor(currentGameTime / 60).ToString("00");
        string seconds = Mathf.Floor(currentGameTime % 60).ToString("00");

        return minutes + ":" + seconds;
    }

    [PunRPC]
    private void SyncTime(bool gameInProgress, float currentGameTime)
    {
        TimeManager.currentGameTime = currentGameTime;
        TimeManager.gameInProgress = gameInProgress;
    }

    public override void OnPlayerEnteredRoom(Player newPlayer)
    {
        if (PhotonNetwork.IsMasterClient)
        {
            photonView.RPC("SyncTime", RpcTarget.Others, gameInProgress, currentGameTime);
        }
    }
}
