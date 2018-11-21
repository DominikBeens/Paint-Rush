using System;
using System.Collections.Generic;
using UnityEngine;

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
    }

    public void AddPaint(PaintType color, float amount)
    {
        for (int i = 0; i < paintValues.Count; i++)
        {
            if (paintValues[i].paintType == color)
            {
                paintValues[i].paintValue += amount;
                paintValues[i].paintValue = Mathf.Clamp(paintValues[i].paintValue, 0, 100);

                if (paintValues[i].paintValue == 100)
                {
                    PaintFilled(paintValues[i].paintType);
                }

                OnPaintValueModified(paintValues[i].paintType, amount);
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
                OnPaintValueReset(paintValues[i].paintType);
                return;
            }
        }
    }

    private void PaintFilled(PaintType color)
    {
        ResetPaint(color);
    }
}
