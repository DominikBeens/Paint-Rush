using Photon.Pun;
using Photon.Realtime;
using System;
using UnityEngine;

public class TimeManager : MonoBehaviourPunCallbacks, IPunObservable
{

    public static TimeManager instance;

    private const int GAME_TIME_IN_SECONDS = 600;
    private const int PEOPLE_NEEDED_TO_START_GAME = 2;

    private static float currentGameTime;

    public event Action OnStartGame = delegate { };
    public event Action OnEndGame = delegate { };

    public enum GameTimeState { WaitingForPlayers, Starting, InProgress, Ending };
    private GameTimeState gameTimeState;
    public GameTimeState CurrentGameTimeState
    {
        get
        {
            return gameTimeState;
        }
        private set
        {
            gameTimeState = value;
            OnGameTimeStateChanged(gameTimeState);
        }
    }
    public static event Action<GameTimeState> OnGameTimeStateChanged = delegate { };

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

    private void Start()
    {
        // First one to join.
        if (PhotonNetwork.IsMasterClient)
        {
            CurrentGameTimeState = GameTimeState.WaitingForPlayers;
        }
    }

    private void Update()
    {
        if (CurrentGameTimeState == GameTimeState.InProgress)
        {
            currentGameTime -= Time.deltaTime;

            if (currentGameTime <= 0)
            {
                EndGame();
            }
        }
        else
        {
            TryStartGame();
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

    [PunRPC]
    private void StartCountdown()
    {

    }

    private void StartGame()
    {
        if (CurrentGameTimeState == GameTimeState.InProgress)
        {
            return;
        }

        OnStartGame();
        CurrentGameTimeState = GameTimeState.InProgress;
        currentGameTime = GAME_TIME_IN_SECONDS;

        if (PhotonNetwork.IsMasterClient)
        {
            NotificationManager.instance.NewNotification("<color=red>Game Started!");
        }
    }

    private void EndGame()
    {
        if (CurrentGameTimeState == GameTimeState.Ending)
        {
            return;
        }

        OnEndGame();
        CurrentGameTimeState = GameTimeState.Ending;
        currentGameTime = 0;

        if (PhotonNetwork.IsMasterClient)
        {
            NotificationManager.instance.NewNotification("<color=red>Game Ended!");
        }
    }

    private void TryStartGame()
    {
        int peopleOnline = PhotonNetwork.CurrentRoom.PlayerCount;
        if (peopleOnline >= PEOPLE_NEEDED_TO_START_GAME)
        {
            if (CurrentGameTimeState != GameTimeState.Starting)
            {
                CurrentGameTimeState = GameTimeState.Starting;
            }

            if (PhotonNetwork.IsMasterClient)
            {
                // Start countdown to start the game.
                // Send RPC to start game.
            }
        }
        else
        {
            if (CurrentGameTimeState != GameTimeState.WaitingForPlayers)
            {
                CurrentGameTimeState = GameTimeState.WaitingForPlayers;
            }
        }
    }

    public string GetFormattedGameTime()
    {
        string minutes = Mathf.Floor(currentGameTime / 60).ToString("00");
        string seconds = Mathf.Floor(currentGameTime % 60).ToString("00");

        return minutes + ":" + seconds;
    }

    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        if (stream.IsWriting)
        {
            if (PhotonNetwork.IsMasterClient)
            {
                stream.SendNext(currentGameTime);
                stream.SendNext((int)CurrentGameTimeState);
            }
        }
        else
        {
            if (!PhotonNetwork.IsMasterClient)
            {
                currentGameTime = (float)stream.ReceiveNext();

                GameTimeState syncedState = (GameTimeState)stream.ReceiveNext();
                if (syncedState != CurrentGameTimeState)
                {
                    CurrentGameTimeState = syncedState;
                }
            }
        }
    }
}
