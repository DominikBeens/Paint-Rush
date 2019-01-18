using UnityEngine;

public class PlayerAnimationController : MonoBehaviour
{

    private Animator anim;
    private bool initialised;

    private AnimationClip winEmote;
    public AnimationClip WinEmote { get { return winEmote; } }

    private int emoteIndex;

    [Header("Arm Sway")]
    [SerializeField] private bool sway = true;
    [SerializeField] private float horizontalSwaySpeed = 3f;
    [SerializeField] private float verticalSwaySpeed = 2f;
    [SerializeField] private float leanSpeed = 2f;

    private float horizontalSway;
    private float verticalSway;
    private float lean;

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
        //PlayerManager.instance.playerController.OnJump += PlayerController_OnJump;
        PlayerManager.instance.playerController.OnSlide += PlayerController_OnSlide;
        WeaponSlot.OnFireWeapon += WeaponSlot_OnFireWeapon;
    }

    private void PlayerController_OnJump()
    {
        anim.SetBool("Jump", true);
        Invoke("ResetJump", 0.5f);
    }

    private void PlayerController_OnSlide(bool b)
    {
        anim.SetBool("Slide", b);
    }

    public float GetSlideDuration()
    {
        AnimationClip[] clips = anim.runtimeAnimatorController.animationClips;
        for (int i = 0; i < clips.Length; i++)
        {
            if (clips[i].name == "Character_Slide")
            {
                return clips[i].length - 0.75f;
            }
        }

        Debug.LogWarning("Couldn't find slide animation in animator. Clip length returned is zero.");
        return 0;
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

        anim.SetFloat("Horizontal", CanAnimateMovement() ? Input.GetAxis("Horizontal") : 0);
        anim.SetFloat("Vertical", CanAnimateMovement() ? Input.GetAxis("Vertical") : 0);

        ArmSway();

        if (Input.GetKeyDown("l")) ////////////////////////////////////////////////////////////////////////////////////////////PLACEHOLDER RIGHT HERE
        {
            ToggleWinEmote(anim.GetCurrentAnimatorStateInfo(0).IsName("Emote") ? false : true);
        }
    }

    private void ArmSway()
    {
        if (!sway)
        {
            return;
        }

        horizontalSway = Mathf.Lerp(horizontalSway, Input.GetAxis("Mouse X") * 0.5f, Time.deltaTime * horizontalSwaySpeed);
        verticalSway = Mathf.Lerp(verticalSway, Input.GetAxis("Mouse Y") * 0.5f, Time.deltaTime * verticalSwaySpeed);
        lean = Mathf.Lerp(lean, Input.GetAxis("Horizontal"), Time.deltaTime * leanSpeed);

        anim.SetFloat("SwayHor", horizontalSway);
        anim.SetFloat("SwayVer", verticalSway);
        anim.SetFloat("LeanHor", lean);
    }

    private void WeaponSlot_OnFireWeapon()
    {
        anim.SetTrigger("Recoil");
    }

    private bool CanAnimateMovement()
    {
        return PlayerManager.instance.playerController.IsGrounded && TimeManager.CurrentGameTimeState != TimeManager.GameTimeState.Ending;
    }

    public void SetWinEmote(int index , CustomizationTerminal terminal)
    {
        winEmote = terminal.Emotes[index];
        emoteIndex = index;
    }

    public void ToggleWinEmote(bool toggle)
    {
        if (toggle)
        {
            if (winEmote != null && !anim.GetCurrentAnimatorStateInfo(0).IsName("Emote"))
            {
                //anim.Play(winEmote.name, 0);
                anim.SetBool("Emote", true);
                anim.SetFloat("EmoteIndex", emoteIndex);
            }
        }
        else
        {
            if (anim.GetCurrentAnimatorStateInfo(0).IsName("Emote"))
            {
                // anim.Play("Locomotion", 0);
                anim.SetBool("Emote", false);
            }
        }
    }

    private void OnDisable()
    {
        if (initialised)
        {
            PlayerManager.instance.playerController.OnSlide -= PlayerController_OnSlide;
            WeaponSlot.OnFireWeapon -= WeaponSlot_OnFireWeapon;
        }
    }
}
