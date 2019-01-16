using Photon.Pun;
using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class PaintController
{

    private Entity myEntity;
    private PlayerManager myPlayerManager;

    public enum PaintType { Cyan, Purple, Green, Yellow };

    [Serializable]
    public class PaintValue
    {
        public PaintType paintType;
        public float paintValue;
        public Color paintColor;
        public float resetFinish;

        public bool CanIncrement()
        {
            return Time.time > resetFinish;
        }
    }

    [Serializable]
    public class PaintMark
    {
        public PaintType markType;
        public float markValue;
    }

    [SerializeField] private List<PaintValue> paintValues = new List<PaintValue>();
    public List<PaintValue> PaintValues { get { return paintValues; } }

    private PaintMark paintMark;
    public PaintMark CurrentPaintMark
    {
        get
        {
            return paintMark;
        }
        set
        {
            paintMark = value;
            if (paintMark != null)
            {
                OnPaintMarkActivated(paintMark);
            }
            else
            {
                OnPaintMarkDestroyed();
            }
        }
    }

    public event Action<PaintType, float> OnPaintValueModified = delegate { };
    public event Action<PaintType> OnPaintValueReset = delegate { };
    public event Action<PaintMark> OnPaintMarkActivated = delegate { };
    public event Action OnPaintMarkDestroyed = delegate { };
    public event Action<bool> OnToggleUI = delegate { };

    public void Initialise(Entity entity)
    {
        myEntity = entity;
        myPlayerManager = myEntity.transform.root.GetComponent<PlayerManager>();
        SetDefaultPaintValues();

        if (myPlayerManager)
        {
            myPlayerManager.OnPlayerStateChanged += MyPlayerManager_OnPlayerStateChanged;
            TimeManager.OnEndMatch += TimeManager_OnEndMatch;
        }
    }

    private void SetDefaultPaintValues()
    {
        paintValues.Clear();
        paintValues.Add(new PaintValue { paintType = PaintType.Cyan, paintColor = new Color(0, 255, 255, 255) });
        paintValues.Add(new PaintValue { paintType = PaintType.Purple, paintColor = new Color(255, 0, 255, 255) });
        paintValues.Add(new PaintValue { paintType = PaintType.Green, paintColor = new Color(0, 255, 0, 255) });
        paintValues.Add(new PaintValue { paintType = PaintType.Yellow, paintColor = new Color(255, 255, 0, 255) });

        // If this is our PaintController, also initialise the UIManager.
        if (myPlayerManager && myPlayerManager.photonView.IsMine && WeaponSlot.currentWeapon)
        {
            UIManager.instance.Initialise(GetPaintColor(WeaponSlot.currentPaintType));
        }
    }

    public void ModifyPaint(PaintType color, float amount, int attackerID)
    {
        //if (myPlayerManager && myPlayerManager.PlayerState != GameManager.GameState.Playing && attackerID != PlayerManager.instance.photonView.ViewID)
        //{
        //    return;
        //}

        if (myEntity == PlayerManager.instance.entity && amount > 0)
        {
            UIManager.instance.ScreenHitEffect(GetPaintColor(color));
        }

        if (CurrentPaintMark == null)
        {
            for (int i = 0; i < paintValues.Count; i++)
            {
                if (paintValues[i].paintType == color && paintValues[i].CanIncrement())
                {
                    paintValues[i].paintValue += amount;
                    paintValues[i].paintValue = Mathf.Clamp(paintValues[i].paintValue, 0, 100);

                    OnPaintValueModified(paintValues[i].paintType, amount);

                    if (paintValues[i].paintValue == 100)
                    {
                        PaintFilled(paintValues[i].paintType, attackerID);
                    }
                    return;
                }
            }
        }
        else
        {
            if (color != CurrentPaintMark.markType)
            {
                CurrentPaintMark.markValue -= amount * 2;
                CurrentPaintMark.markValue = Mathf.Clamp(CurrentPaintMark.markValue, 0, 100);
                OnPaintValueModified(color, amount * 2);

                if (CurrentPaintMark.markValue <= 0)
                {
                    MarkDestroyed();

                    if (attackerID == PlayerManager.instance.photonView.ViewID)
                    {
                        SaveManager.instance.SaveStat(SaveManager.SavedStat.MarksDestroyed);
                    }
                }
            }
        }
    }

    public void ResetPaint(PaintType? color = null)
    {
        for (int i = 0; i < paintValues.Count; i++)
        {
            if (color != null)
            {
                if (paintValues[i].paintType == color)
                {
                    ResetPaintValue(paintValues[i]);
                    return;
                }
            }
            else
            {
                ResetPaintValue(paintValues[i]);
            }
        }
    }

    private void ResetPaintValue(PaintValue paintValue)
    {
        paintValue.paintValue = 0;
        paintValue.resetFinish = Time.time + 0.5f;
        OnPaintValueReset(paintValue.paintType);
    }

    public Color GetPaintColor(PaintType paintType)
    {
        for (int i = 0; i < paintValues.Count; i++)
        {
            if (paintValues[i].paintType == paintType)
            {
                return paintValues[i].paintColor;
            }
        }

        return Color.white;
    }

    public string GetAllPaintValuesText()
    {
        string[] values = new string[paintValues.Count];

        for (int i = 0; i < paintValues.Count; i++)
        {
            values[i] = "<color=#" + ColorUtility.ToHtmlStringRGBA(paintValues[i].paintColor) + ">" + paintValues[i].paintValue + "</color>";
        }

        return string.Join(" <=> ", values);
    }

    // Used for syncing paint values when new players join.
    public void SetRawValues(List<float> values)
    {
        float difference;
        for (int i = 0; i < paintValues.Count; i++)
        {
            difference = Mathf.Abs(paintValues[i].paintValue - values[i]);

            paintValues[i].paintValue += difference;
            paintValues[i].paintValue = Mathf.Clamp(paintValues[i].paintValue, 0, 100);

            OnPaintValueModified(paintValues[i].paintType, difference);
        }
    }

    private void PaintFilled(PaintType color, int attackerID)
    {
        ResetPaint(color);
        ObjectPooler.instance.GrabFromPool("TeleportParticle", myEntity.transform.position, myEntity.transform.rotation);

        if (attackerID == PlayerManager.instance.photonView.ViewID)
        {
            if (myEntity == PlayerManager.instance.entity)
            {
                NotificationManager.instance.NewNotification("<color=#" + GameManager.personalColorString + "> " + PhotonNetwork.NickName + "</color> has killed himself!");
            }
            else
            {
                if (PlayerManager.instance.entity.paintController.CurrentPaintMark == null)
                {
                    PlayerManager.instance.entity.paintController.CurrentPaintMark = new PaintMark { markType = color, markValue = 100 };
                    NotificationManager.instance.NewNotification("<color=#" + GameManager.personalColorString + "> " + PhotonNetwork.NickName + "</color> has claimed a mark!", NotificationManager.NotificationType.MarkGained);
                    SaveManager.instance.SaveStat(SaveManager.SavedStat.MarksGained);
                }

                SaveManager.instance.SaveStat(SaveManager.SavedStat.Kills);
            }
        }
        else
        {
            PhotonView pv = PhotonView.Find(attackerID);
            if (pv)
            {
                PlayerManager playerManager = pv.GetComponent<PlayerManager>();
                if (playerManager && playerManager.entity.paintController.CurrentPaintMark == null)
                {
                    playerManager.entity.paintController.CurrentPaintMark = new PaintMark { markType = color, markValue = 100 };
                }
            }
        }

        if (myEntity == PlayerManager.instance.entity)
        {
            GameManager.CurrentGameSate = GameManager.GameState.Respawning;
        }
    }

    private void MarkDestroyed()
    {
        CurrentPaintMark = null;

        if (PlayerManager.instance.entity == myEntity)
        {
            NotificationManager.instance.NewNotification("<color=#" + GameManager.personalColorString + "> " + PhotonNetwork.NickName + "'s</color> mark has been destroyed!", NotificationManager.NotificationType.MarkDestroyed);
        }
    }

    public void MarkCaptured()
    {
        CurrentPaintMark = null;

        if (PlayerManager.instance.entity == myEntity)
        {
            SaveManager.instance.SaveStat(SaveManager.SavedStat.GamePointsGained);
            NotificationManager.instance.NewNotification("<color=#" + GameManager.personalColorString + "> " + PhotonNetwork.NickName + "</color> has gained a game-point!", NotificationManager.NotificationType.MarkCaptured);
        }

        ScoreboardManager.instance.RegisterPlayerGamePoint(myPlayerManager.photonView.ViewID);
    }

    public void ToggleUI(bool toggle)
    {
        OnToggleUI(toggle);
    }

    private void ResetPaintAndMark()
    {
        ResetPaint();
        CurrentPaintMark = null;

        ToggleUI(false);
    }

    private void MyPlayerManager_OnPlayerStateChanged(GameManager.GameState newState)
    {
        ResetPaintAndMark();
    }

    private void TimeManager_OnEndMatch()
    {
        ResetPaintAndMark();
    }

    public void UnsubscribeEvents()
    {
        if (myPlayerManager)
        {
            myPlayerManager.OnPlayerStateChanged -= MyPlayerManager_OnPlayerStateChanged;
            TimeManager.OnEndMatch -= TimeManager_OnEndMatch;
        }
    }
}
