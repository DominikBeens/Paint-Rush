using UnityEngine;

public class PlayerAnimationController : MonoBehaviour
{

    private Animator anim;
    private bool initialised;

    private void Awake()
    {
        anim = GetComponentInChildren<Animator>();
    }

    public void Initialise(bool local)
    {
        if (!local)
        {
            enabled = false;
            return;
        }

        initialised = true;
        PlayerManager.instance.playerController.OnJump += PlayerController_OnJump;
    }

    private void PlayerController_OnJump()
    {
        anim.SetBool("Jump", true);
        Invoke("ResetJump", 0.5f);
    }

    private void ResetJump()
    {
        anim.SetBool("Jump", false);
    }

    private void Update()
    {
        if (!initialised)
        {
            return;
        }

        if (anim.GetBool("Run") != PlayerManager.instance.playerController.IsMoving)
        {
            anim.SetBool("Run", PlayerManager.instance.playerController.IsMoving);
        }
    }

    private void OnDisable()
    {
        if (initialised)
        {
            PlayerManager.instance.playerController.OnJump -= PlayerController_OnJump;
        }
    }
}
