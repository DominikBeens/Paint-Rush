using UnityEngine;

public class PaintUI : MonoBehaviour
{

    private bool initialised;
    private Entity myEntity;
    private PaintUIBar[] paintUIBars;
    private Animator anim;

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

            gameObject.SetActive(false);
            return;
        }

        paintUIBars = GetComponentsInChildren<PaintUIBar>();
        anim = GetComponentInChildren<Animator>();

        for (int i = 0; i < entity.paintController.PaintValues.Count; i++)
        {
            paintUIBars[i].Initialise(entity.paintController.PaintValues[i]);
        }

        entity.paintController.OnPaintValueModified += PaintController_OnPaintValueModified;
        entity.paintController.OnPaintValueReset += PaintController_OnPaintValueReset;
    }

    private void PaintController_OnPaintValueModified(PaintController.PaintType paintType, float amount)
    {
        if (amount <= 0)
        {
            return;
        }

        for (int i = 0; i < paintUIBars.Length; i++)
        {
            if (paintUIBars[i].BarType == paintType)
            {
                paintUIBars[i].IncrementBar(amount);
            }
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

    private void OnDisable()
    {
        if (initialised)
        {
            myEntity.paintController.OnPaintValueModified -= PaintController_OnPaintValueModified;
            myEntity.paintController.OnPaintValueReset -= PaintController_OnPaintValueReset;
        }
    }
}
