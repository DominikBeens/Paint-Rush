using System;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

[Serializable]
public class PaintController 
{

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

    public void Initialise()
    {
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
            UIManager.instance.Initialise(GetPaintColor(WeaponSlot.currentWeapon.paintType));
        }
    }

    public void AddPaint(PaintType color, float amount, int attackerID)
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

    private void PaintFilled(PaintType color, int attackerID)
    {
        if (PhotonNetwork.IsMasterClient)
        {
            string attackerName = "";
            PhotonView pv = PhotonView.Find(attackerID);
            if (pv)
            {
                attackerName = pv.Owner.NickName;
            }

            if (!string.IsNullOrEmpty(attackerName))
            {
                NotificationManager.instance.NewNotification("<color=red>" + attackerName + "</color> has claimed a mark!");
            }
        }

        ResetPaint(color);
    }
}
