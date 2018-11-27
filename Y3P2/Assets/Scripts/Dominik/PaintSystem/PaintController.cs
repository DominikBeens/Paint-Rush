using Photon.Pun;
using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class PaintController
{

    private Entity myEntity;

    public enum PaintState { Free, Mark };
    private PaintState paintState;
    public PaintState CurrentPaintState { get { return paintState; } }

    private PaintType lastHitPaintType;
    public PaintType LastHitPaintType { get { return lastHitPaintType; } }

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

    [SerializeField] private List<PaintValue> paintValues = new List<PaintValue>();
    public List<PaintValue> PaintValues { get { return paintValues; } }

    public event Action<PaintType, float> OnPaintValueModified = delegate { };
    public event Action<PaintType> OnPaintValueReset = delegate { };
    public event Action<PaintState, PaintType> OnPaintStateChanged = delegate { };

    public void Initialise(Entity entity)
    {
        myEntity = entity;
        SetDefaultPaintValues();
    }

    private void SetDefaultPaintValues()
    {
        paintValues.Clear();
        paintValues.Add(new PaintValue { paintType = PaintType.Cyan, paintColor = new Color(0, 255, 255, 255) });
        paintValues.Add(new PaintValue { paintType = PaintType.Green, paintColor = new Color(0, 255, 0, 255) });
        paintValues.Add(new PaintValue { paintType = PaintType.Purple, paintColor = new Color(255, 0, 255, 255) });
        paintValues.Add(new PaintValue { paintType = PaintType.Yellow, paintColor = new Color(255, 255, 0, 255) });

        if (WeaponSlot.currentWeapon)
        {
            UIManager.instance.Initialise(GetPaintColor(WeaponSlot.currentPaintType));
        }
    }

    public void AddPaint(PaintType color, float amount, int attackerID)
    {
        lastHitPaintType = color;

        if (paintState == PaintState.Free)
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
            OnPaintValueModified(color, amount);
        }
    }

    private void ResetPaint(PaintType color)
    {
        for (int i = 0; i < paintValues.Count; i++)
        {
            if (paintValues[i].paintType == color)
            {
                paintValues[i].paintValue = 0;
                paintValues[i].resetFinish = Time.time + 0.5f;
                OnPaintValueReset(paintValues[i].paintType);
                return;
            }
        }
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

    public void SetPaintState(PaintState newState, PaintType caller)
    {
        if (newState == paintState)
        {
            return;
        }

        paintState = newState;
        OnPaintStateChanged(paintState, caller);
    }

    // Used for syncing paint values when new players join.
    //public void SetRawValues(List<float> values)
    //{
    //    float difference;
    //    for (int i = 0; i < paintValues.Count; i++)
    //    {
    //        difference = paintValues[i].paintValue - values[i];

    //        paintValues[i].paintValue += difference;
    //        paintValues[i].paintValue = Mathf.Clamp(paintValues[i].paintValue, 0, 100);

    //        OnPaintValueModified(paintValues[i].paintType, difference);
    //    }
    //}

    private void PaintFilled(PaintType color, int attackerID)
    {
        ResetPaint(color);

        if (attackerID == PlayerManager.instance.photonView.ViewID)
        {
            if (PlayerManager.instance.entity.paintController.CurrentPaintState == PaintState.Free)
            {
                PlayerManager.instance.entity.paintController.SetPaintState(PaintState.Mark, color);
                NotificationManager.instance.NewNotification("<color=#" + GameManager.personalColorString + "> " + PhotonNetwork.NickName + "</color> has claimed a mark!");
            }
        }
        else
        {
            PhotonView pv = PhotonView.Find(attackerID);
            if (pv)
            {
                pv.GetComponent<PlayerManager>().entity.paintController.SetPaintState(PaintState.Mark, color);
            }
        }

        if (PlayerManager.instance.entity == myEntity)
        {
            GameManager.CurrentGameSate = GameManager.GameState.Respawning;
        }
    }
}
