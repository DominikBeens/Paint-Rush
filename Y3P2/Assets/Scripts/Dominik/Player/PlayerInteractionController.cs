using UnityEngine;

public class PlayerInteractionController : MonoBehaviour
{

    private bool initialised;
    private Camera mainCam;
    private Interactable lastHitInteractable;

    [SerializeField] private LayerMask interactMask;
    [SerializeField] private float interactDistance = 5f;
    [SerializeField] private KeyCode interactKey = KeyCode.E;

    public void Initialise(bool local)
    {
        if (!local)
        {
            enabled = false;
            return;
        }

        mainCam = Camera.main;
        initialised = true;
    }

    private void Update()
    {
        if (GameManager.CurrentGameSate != GameManager.GameState.Lobby)
        {
            return;
        }

        RaycastHit hit;
        if (Physics.Raycast(mainCam.transform.position, mainCam.transform.forward, out hit, interactDistance, interactMask))
        {
            Interactable interactable = hit.transform.GetComponent<Interactable>();
            if (interactable)
            {
                if (interactable != lastHitInteractable)
                {
                    if (lastHitInteractable)
                    {
                        lastHitInteractable.Hide();
                    }
                    lastHitInteractable = interactable;
                }

                interactable.Show();

                if (Input.GetKeyDown(interactKey))
                {
                    interactable.Interact();
                }
            }
        }
        else
        {
            if (lastHitInteractable)
            {
                lastHitInteractable.Hide();
            }
        }
    }
}
