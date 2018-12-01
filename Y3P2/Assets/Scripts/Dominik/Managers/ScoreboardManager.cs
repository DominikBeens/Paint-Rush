using Photon.Pun;
using Photon.Realtime;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class ScoreboardManager : MonoBehaviourPunCallbacks
{

    public static ScoreboardManager instance;

    public class PlayerScore
    {
        public int playerPhotonViewID;
        public string playerName;
        public int playerGamePoints;
    }
    private List<PlayerScore> playerScores = new List<PlayerScore>();

    private void Awake()
    {
        if (!instance)
        {
            instance = this;
        }
        else if (instance && instance != this)
        {
            Destroy(this);
        }
    }

    private void Start()
    {
        PlayerManager[] players = FindObjectsOfType<PlayerManager>();
        for (int i = 0; i < players.Length; i++)
        {
            AddPlayer(players[i].photonView.ViewID, players[i].photonView.Owner.NickName);
        }

        photonView.RPC("SendGameStats", RpcTarget.Others);
    }

    public void AddPlayer(int viewID, string name)
    {
        playerScores.Add(new PlayerScore { playerPhotonViewID = viewID, playerName = name, playerGamePoints = 0 });
    }

    private void RemovePlayer(string name)
    {
        for (int i = 0; i < playerScores.Count; i++)
        {
            if (playerScores[i].playerName == name)
            {
                playerScores.RemoveAt(i);
                return;
            }
        }
    }

    private int GetScore()
    {
        for (int i = 0; i < playerScores.Count; i++)
        {
            if (playerScores[i].playerPhotonViewID == PlayerManager.instance.photonView.ViewID)
            {
                return playerScores[i].playerGamePoints;
            }
        }

        return 0;
    }

    public List<PlayerScore> GetSortedPlayerScores()
    {
        List<PlayerScore> sorted = new List<PlayerScore>(playerScores);
        sorted = sorted.OrderBy(x => x.playerGamePoints).Reverse().ToList();

        return sorted;
    }

    public void RegisterPlayerGamePoint(int playerViewID)
    {
        for (int i = 0; i < playerScores.Count; i++)
        {
            if (playerScores[i].playerPhotonViewID == playerViewID)
            {
                playerScores[i].playerGamePoints++;
                return;
            }
        }
    }

    [PunRPC]
    private void SendGameStats()
    {
        photonView.RPC("ReceiveGameStats", RpcTarget.Others, instance.photonView.ViewID, instance.GetScore());
    }

    [PunRPC]
    private void ReceiveGameStats(int viewID, int gamePoints)
    {
        for (int i = 0; i < playerScores.Count; i++)
        {
            if (playerScores[i].playerPhotonViewID == viewID)
            {
                playerScores[i].playerGamePoints += gamePoints;
            }
        }
    }

    public override void OnPlayerLeftRoom(Player otherPlayer)
    {
        RemovePlayer(otherPlayer.NickName);
    }
}
