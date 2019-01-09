using Photon.Pun;
using System;
using UnityEngine;

public class TimeManager : MonoBehaviourPunCallbacks, IPunObservable
{

    public static TimeManager instance;

    private const int GAME_TIME_IN_SECONDS = 600;
    private const int PEOPLE_NEEDED_TO_START_GAME = 2;

    private static float currentGameTime;
    public static float countdownTime;

    private bool notifiedCurrentGameTime;

    public static event Action OnStartMatch = delegate { };
    public static event Action OnEndMatch = delegate { };

    public enum GameTimeState { WaitingForPlayers, Ready, Starting, InProgress, Ending };
    private GameTimeState gameTimeState;
    public static GameTimeState CurrentGameTimeState
    {
        get
        {
            return instance.gameTimeState;
        }
        private set
        {
            instance.gameTimeState = value;
            OnGameTimeStateChanged(instance.gameTimeState);
        }
    }
    public static event Action<GameTimeState> OnGameTimeStateChanged = delegate { };

    [SerializeField] private float startGameCountdownTime = 10f;
    [SerializeField] private float endGameCountdownTime = 10f;

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
        switch (CurrentGameTimeState)
        {
            case GameTimeState.WaitingForPlayers:
                HandleState_WaitingForPlayers();
                break;

            case GameTimeState.Ready:
                HandleState_Ready();
                break;

            case GameTimeState.Starting:
                HandleState_Starting();
                break;

            case GameTimeState.InProgress:
                HandleState_InProgress();
                break;

            case GameTimeState.Ending:
                HandleState_Ending();
                break;
        }

        if (Input.GetKeyDown(KeyCode.N))
        {
            StartMatch();
        }
        if (Input.GetKeyDown(KeyCode.M))
        {
            currentGameTime = 5f;
        }
    }

    private void HandleState_WaitingForPlayers()
    {
        if (EnoughPeopleToStart())
        {
            CurrentGameTimeState = GameTimeState.Ready;
        }
    }

    private void HandleState_Ready()
    {
        if (!EnoughPeopleToStart())
        {
            CurrentGameTimeState = GameTimeState.WaitingForPlayers;
        }
    }

    private void HandleState_Starting()
    {
        countdownTime -= Time.deltaTime;
        if (countdownTime <= 0)
        {
            StartMatch();
        }

        if (!EnoughPeopleToStart())
        {
            CurrentGameTimeState = GameTimeState.WaitingForPlayers;
            countdownTime = 0;
        }
    }

    private void HandleState_InProgress()
    {
        currentGameTime -= Time.deltaTime;

        // Displaying game time notifications. Ugly but I'm tired okay.
        if (PhotonNetwork.IsMasterClient)
        {
            if (currentGameTime <= 61 && currentGameTime > 60 && !notifiedCurrentGameTime)
            {
                NotificationManager.instance.NewNotification("<color=red>Sixty seconds remaining!");
                notifiedCurrentGameTime = true;
            }
            else if (currentGameTime <= 51 && currentGameTime > 50 && notifiedCurrentGameTime)
            {
                NotificationManager.instance.NewNotification("<color=red>Fifty seconds remaining!");
                notifiedCurrentGameTime = false;
            }
            else if (currentGameTime <= 41 && currentGameTime > 40 && !notifiedCurrentGameTime)
            {
                NotificationManager.instance.NewNotification("<color=red>Fourty seconds remaining!");
                notifiedCurrentGameTime = true;
            }
            else if (currentGameTime <= 31 && currentGameTime > 30 && notifiedCurrentGameTime)
            {
                NotificationManager.instance.NewNotification("<color=red>Thirty seconds remaining!");
                notifiedCurrentGameTime = false;
            }
            else if (currentGameTime <= 21 && currentGameTime > 20 && !notifiedCurrentGameTime)
            {
                NotificationManager.instance.NewNotification("<color=red>Twenty seconds remaining!");
                notifiedCurrentGameTime = true;
            }
            else if (currentGameTime <= 11 && currentGameTime > 10 && notifiedCurrentGameTime)
            {
                NotificationManager.instance.NewNotification("<color=red>Ten seconds remaining!");
                notifiedCurrentGameTime = false;
            }
        }

        if (currentGameTime <= 0)
        {
            EndMatch();
        }
    }

    private void HandleState_Ending()
    {
        countdownTime -= Time.deltaTime;
        if (countdownTime <= 0)
        {
            CurrentGameTimeState = GameTimeState.WaitingForPlayers;
        }
    }

    [PunRPC]
    private void StartMatchRPC()
    {
        TryStartMatch();
    }

    private void StartMatch()
    {
        if (CurrentGameTimeState == GameTimeState.InProgress)
        {
            return;
        }

        CurrentGameTimeState = GameTimeState.InProgress;
        currentGameTime = GAME_TIME_IN_SECONDS;
        OnStartMatch();

        if (GameManager.CurrentGameSate == GameManager.GameState.Lobby)
        {
            GameManager.CurrentGameSate = GameManager.GameState.Playing;
        }

        if (PhotonNetwork.IsMasterClient)
        {
            NotificationManager.instance.NewNotification("<color=red>Match Started!");
        }
    }

    private void EndMatch()
    {
        if (CurrentGameTimeState == GameTimeState.Ending)
        {
            return;
        }

        CurrentGameTimeState = GameTimeState.Ending;
        currentGameTime = 0;
        countdownTime = endGameCountdownTime;
        OnEndMatch();

        // TEMP.
        //NotificationManager.instance.NewLocalNotification("TEMP: END MATCH");

        if (PhotonNetwork.IsMasterClient)
        {
            NotificationManager.instance.NewNotification("<color=red>Match Ended!");
        }
    }

    private void TryStartMatch()
    {
        if (EnoughPeopleToStart())
        {
            if (CurrentGameTimeState != GameTimeState.Starting)
            {
                CurrentGameTimeState = GameTimeState.Starting;
            }

            if (PhotonNetwork.IsMasterClient)
            {
                countdownTime = startGameCountdownTime;
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

    public void StartMatchButton()
    {
        switch (CurrentGameTimeState)
        {
            case GameTimeState.WaitingForPlayers:
                NotificationManager.instance.NewLocalNotification("<color=red>Not enough players to start");
                break;

            case GameTimeState.Ready:
                photonView.RPC("StartMatchRPC", RpcTarget.MasterClient);
                NotificationManager.instance.NewNotification("<color=red>Match starting soon...");
                break;

            case GameTimeState.Starting:
                NotificationManager.instance.NewLocalNotification("<color=red>Match is already starting...");
                break;

            case GameTimeState.InProgress:
                NotificationManager.instance.NewLocalNotification("<color=red>Can't start match when there's one in progress!");
                break;

            case GameTimeState.Ending:
                NotificationManager.instance.NewLocalNotification("<color=red>Can't start match when there's one ending!");
                break;
        }
    }

    public void EnterArenaButton()
    {
        if (CurrentGameTimeState == GameTimeState.InProgress)
        {
            GameManager.CurrentGameSate = GameManager.GameState.Playing;
        }
        else
        {
            NotificationManager.instance.NewLocalNotification("<color=red>Can't enter the arena at this time.");
        }
    }

    private bool EnoughPeopleToStart()
    {
        return PhotonNetwork.CurrentRoom.PlayerCount >= PEOPLE_NEEDED_TO_START_GAME;
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
                stream.SendNext(countdownTime);
                stream.SendNext(notifiedCurrentGameTime);
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

                countdownTime = (float)stream.ReceiveNext();
                notifiedCurrentGameTime = (bool)stream.ReceiveNext();
            }
        }
    }
}
