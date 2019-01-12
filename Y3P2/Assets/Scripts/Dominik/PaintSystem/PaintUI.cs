using TMPro;
using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

public class PaintUI : MonoBehaviour
{

    private bool initialised;
    private Entity myEntity;
    // private PaintUIBar[] paintUIBars;
    [SerializeField]
    private List<PaintUIBar> paintUIBars = new List<PaintUIBar>();
    private Animator anim;

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

        anim = GetComponentInChildren<Animator>();

        InitEvents();
        InitUIBars();
    }

    private void InitEvents()
    {
        myEntity.paintController.OnPaintValueModified += PaintController_OnPaintValueModified;
        myEntity.paintController.OnPaintValueReset += PaintController_OnPaintValueReset;
        myEntity.paintController.OnPaintMarkActivated += PaintController_OnPaintMarkActivated;
        myEntity.paintController.OnPaintMarkDestroyed += PaintController_OnPaintMarkDestroyed;
        myEntity.paintController.OnToggleUI += TogglePaintUIBars;
    }

    private void InitUIBars()
    {
        //paintUIBars = GetComponentsInChildren<PaintUIBar>();
        for (int i = 0; i < myEntity.paintController.PaintValues.Count; i++)
        {
            paintUIBars[i].Initialise(myEntity.paintController.PaintValues[i], myEntity == PlayerManager.instance.entity ? true : false);
        }

        if (myEntity == PlayerManager.instance.entity)
        {
            TogglePaintUIBars(false);
        }
        else
        {
            PlayerManager player = transform.root.GetComponentInChildren<PlayerManager>();
            if (!player)
            {
                return;
            }

            switch (player.PlayerState)
            {
                case GameManager.GameState.Lobby:

                    TogglePaintUIBars(false);
                    break;
                case GameManager.GameState.Playing:

                    TogglePaintUIBars(true);
                    break;
                case GameManager.GameState.Respawning:

                    TogglePaintUIBars(false);
                    break;
            }
        }
    }

    private void PaintController_OnPaintValueModified(PaintController.PaintType paintType, float amount)
    {
        if (myEntity.paintController.CurrentPaintMark == null)
        {
            for (int i = 0; i < paintUIBars.Count; i++)
            {
                if (paintUIBars[i].BarType == paintType)
                {
                    paintUIBars[i].IncrementBar(amount);
                }
            }
        }
        else
        {
            markPercentageText.text = Mathf.Clamp(myEntity.paintController.CurrentPaintMark.markValue, 0, 100) + "%";
        }

        if (anim)
        {
            anim.SetTrigger("Hit");
        }
    }

    private void PaintController_OnPaintValueReset(PaintController.PaintType paintType)
    {
        for (int i = 0; i < paintUIBars.Count; i++)
        {
            if (paintUIBars[i].BarType == paintType)
            {
                paintUIBars[i].ResetBar();
            }
        }
    }

    private void PaintController_OnPaintMarkActivated(PaintController.PaintMark mark)
    {
        // No need to show a mark above our head when its ours.
        if (myEntity.photonView.IsMine)
        {
            return;
        }

        TogglePaintUIBars(false);
        markObject.SetActive(true);

        markImage.color = myEntity.paintController.GetPaintColor(mark.markType);
        markPercentageText.text = mark.markValue + "%";
    }

    private void PaintController_OnPaintMarkDestroyed()
    {
        if (myEntity.photonView.IsMine)
        {
            return;
        }

        TogglePaintUIBars(true);
        markObject.SetActive(false);
    }

    public void TogglePaintUIBars(bool toggle)
    {
        for (int i = 0; i < paintUIBars.Count; i++)
        {
            paintUIBars[i].ToggleUI(toggle);
        }
    }

    private void OnDisable()
    {
        if (initialised)
        {
            myEntity.paintController.OnPaintValueModified -= PaintController_OnPaintValueModified;
            myEntity.paintController.OnPaintValueReset -= PaintController_OnPaintValueReset;
            myEntity.paintController.OnPaintMarkActivated -= PaintController_OnPaintMarkActivated;
            myEntity.paintController.OnPaintMarkDestroyed -= PaintController_OnPaintMarkDestroyed;
            myEntity.paintController.OnToggleUI -= TogglePaintUIBars;
        }
    }
}
