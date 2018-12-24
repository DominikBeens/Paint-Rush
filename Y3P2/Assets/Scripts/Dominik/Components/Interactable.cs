using UnityEngine;
using UnityEngine.Events;

public class Interactable : MonoBehaviour
{

    private bool visible;

    [SerializeField] private UnityEvent OnInteract;

    [Space]

    [SerializeField] private UnityEvent OnShow;
    [SerializeField] private UnityEvent OnHide;

    public void Interact()
    {
        OnInteract.Invoke();
    }

    public void Show()
    {
        if (!visible)
        {
            visible = true;
            OnShow.Invoke();
        }
    }

    public void Hide()
    {
        if (visible)
        {
            visible = false;
            OnHide.Invoke();
        }
    }
}
