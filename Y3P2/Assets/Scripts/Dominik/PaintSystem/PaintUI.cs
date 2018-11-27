using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PaintUI : MonoBehaviour
{

    private bool initialised;
    private Entity myEntity;
    private PaintUIBar[] paintUIBars;
    private Animator anim;

    private PaintController.PaintType markType;
    private float markPercentage;

    [SerializeField] private GameObject markObject;
    [SerializeField] private Image markImage;
    [SerializeField] private TextMeshProUGUI markPercentageText;

    private void Start()
    {
        if (!initialised)
        {
            Initialise(transform.root.GetComponentInChildren<Entity>());
        }
    }

    public void Initialise(Entity entity)
    {
        if (!entity)
        {
            return;
        }

        initialised = true;
        myEntity = entity;

        if (entity == PlayerManager.instance.entity)
        {
            for (int i = 0; i < entity.paintController.PaintValues.Count; i++)
            {
                UIManager.instance.PaintUILocalPlayer[i].Initialise(entity.paintController.PaintValues[i]);
            }
        }

        paintUIBars = GetComponentsInChildren<PaintUIBar>();
        anim = GetComponentInChildren<Animator>();

        for (int i = 0; i < entity.paintController.PaintValues.Count; i++)
        {
            paintUIBars[i].Initialise(entity.paintController.PaintValues[i], entity == PlayerManager.instance.entity ? true : false);
        }

        entity.paintController.OnPaintValueModified += PaintController_OnPaintValueModified;
        entity.paintController.OnPaintValueReset += PaintController_OnPaintValueReset;
        entity.paintController.OnPaintStateChanged += PaintController_OnPaintStateChanged;
    }

    private void PaintController_OnPaintValueModified(PaintController.PaintType paintType, float amount)
    {
        if (amount <= 0)
        {
            return;
        }

        switch (myEntity.paintController.CurrentPaintState)
        {
            case PaintController.PaintState.Free:
                for (int i = 0; i < paintUIBars.Length; i++)
                {
                    if (paintUIBars[i].BarType == paintType)
                    {
                        paintUIBars[i].IncrementBar(amount);
                    }
                }
                break;

            case PaintController.PaintState.Mark:

                if (paintType == markType)
                {
                    return;
                }

                markPercentage -= amount;
                markPercentageText.text = markPercentage + "%";

                if (markPercentage <= 0)
                {
                    myEntity.paintController.SetPaintState(PaintController.PaintState.Free, paintType);

                    if (myEntity == PlayerManager.instance.entity)
                    {
                        NotificationManager.instance.NewNotification("<color=#" + GameManager.personalColorString + "> " + Photon.Pun.PhotonNetwork.NickName + "'s</color> mark has been destroyed!");
                    }
                }
                break;
        }

        if (anim)
        {
            anim.SetTrigger("Hit");
        }
    }

    private void PaintController_OnPaintValueReset(PaintController.PaintType paintType)
    {
        for (int i = 0; i < paintUIBars.Length; i++)
        {
            if (paintUIBars[i].BarType == paintType)
            {
                paintUIBars[i].ResetBar();
            }
        }
    }

    private void PaintController_OnPaintStateChanged(PaintController.PaintState newState, PaintController.PaintType caller)
    {
        for (int i = 0; i < paintUIBars.Length; i++)
        {
            paintUIBars[i].gameObject.SetActive(newState == PaintController.PaintState.Free ? true : false);
        }

        markObject.SetActive(newState == PaintController.PaintState.Mark ? true : false);

        if (newState == PaintController.PaintState.Mark)
        {
            markImage.color = myEntity.paintController.GetPaintColor(caller);

            markType = caller;

            markPercentage = 100;
            markPercentageText.text = markPercentage + "%";
        }
    }

    private void OnDisable()
    {
        if (initialised)
        {
            myEntity.paintController.OnPaintValueModified -= PaintController_OnPaintValueModified;
            myEntity.paintController.OnPaintValueReset -= PaintController_OnPaintValueReset;
            myEntity.paintController.OnPaintStateChanged -= PaintController_OnPaintStateChanged;
        }
    }
}
