using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PaintUI : MonoBehaviour
{

    private bool initialised;
    private Entity myEntity;
    private PaintUIBar[] paintUIBars;
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
        entity.paintController.OnPaintMarkActivated += PaintController_OnPaintMarkActivated;
        entity.paintController.OnPaintMarkDestroyed += PaintController_OnPaintMarkDestroyed;
    }

    private void PaintController_OnPaintValueModified(PaintController.PaintType paintType, float amount)
    {
        if (amount <= 0)
        {
            return;
        }

        if (myEntity.paintController.CurrentPaintMark == null)
        {
            for (int i = 0; i < paintUIBars.Length; i++)
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
        for (int i = 0; i < paintUIBars.Length; i++)
        {
            if (paintUIBars[i].BarType == paintType)
            {
                paintUIBars[i].ResetBar();
            }
        }
    }

    private void PaintController_OnPaintMarkActivated(PaintController.PaintMark mark)
    {
        TogglePaintUIBars(false);
        markObject.SetActive(true);

        markImage.color = myEntity.paintController.GetPaintColor(mark.markType);
        markPercentageText.text = mark.markValue + "%";
    }

    private void PaintController_OnPaintMarkDestroyed()
    {
        TogglePaintUIBars(true);
        markObject.SetActive(false);
    }

    private void TogglePaintUIBars(bool toggle)
    {
        for (int i = 0; i < paintUIBars.Length; i++)
        {
            paintUIBars[i].gameObject.SetActive(toggle);
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
        }
    }
}
