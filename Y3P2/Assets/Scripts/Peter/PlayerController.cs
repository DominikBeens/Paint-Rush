using UnityEngine;
using System.Collections;
using System;
public class PlayerController : MonoBehaviour
{

    [SerializeField]
    private float moveSpeed = 20;
    [SerializeField]
    private float wallRunSpeed = 144;
    [SerializeField]
    private float fovNotMoving = 75;
    [SerializeField]
    private float fovMoving = 85;
    [SerializeField]
    private float fovSprintBoostLerpSpeed = 0.5F;
    [SerializeField]
    private float directionalJumpForce = 2000;
    [SerializeField]
    private float verticalJumpForce = 2000;
    [SerializeField]
    private bool divideUpForce;
    [SerializeField]
    private float divideUpForceBy = 0;
    [SerializeField]
    private float myCamRotateSpeed = 80;
    private float angleLimit = 70;
    private float currentAngle;
    public static float defaultAngleX;


    private bool topBob;
    [SerializeField]
    private float bobSpeed;
    [SerializeField]
    private float bobLimit = 0.7F;
    private Vector3 bobRestingPoint;
    [SerializeField]
    private float maxBob;
    [SerializeField]
    private float minBob;
    [SerializeField]
    private float gravityModifier = 0;

    [SerializeField]
    private float wallRunRayDist = 1;
    [SerializeField]
    private float wallRunReleaseForce = 30;
    private bool wallRunningLeft;
    private bool wallrunning;
    private bool wallrunningBack;

    [SerializeField]
    private float slideForce = 91;
    private bool sliding;
    public event Action<bool> OnSlide = delegate { };

    [SerializeField]
    private float slopeAssistForce = 20;

    [SerializeField] private Transform headObject;
    [SerializeField] private Transform spine;

    private Rigidbody rb;
    private bool grounded;
    public bool IsGrounded { get { return grounded; } }
    private bool forceGravity = true;
    private float xMove;
    private float yMove;

    private Camera[] cameras;
    private bool canUseCam = true;

    private bool isMoving;
    public bool IsMoving { get { return isMoving; } }
    public event Action OnJump = delegate { };

    private bool infinJet;
    [SerializeField]
    private float jetBoostCooldownReduction = 1;

    public void Inititalise(bool local)
    {
        if (!local)
        {
            enabled = false;
            return;
        }

        DB.MenuPack.SceneManager.OnGamePaused += SceneManager_OnGamePaused;
    }

    private void SceneManager_OnGamePaused(bool obj)
    {
        canUseCam = !obj;
    }

    private bool canJump = true;
    private bool jumpCooldown;

    [SerializeField]
    private float jumpCooldownTime = 6;

    private void Start()
    {
        rb = GetComponent<Rigidbody>();
        cameras = GetComponentsInChildren<Camera>();

        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;

        bobRestingPoint = headObject.transform.localPosition;
        defaultAngleX = spine.eulerAngles.x;
    }

    private void FixedUpdate()
    {
        RaycastHit hit;
        if (Physics.Raycast(transform.position + Vector3.up * 0.05f, -Vector3.up, out hit, 0.15f))
        {
            if (!grounded)
            {
                grounded = true;
                if (!rb.useGravity)
                {
                    rb.useGravity = true;
                }
            }
        }
        else if (hit.transform == null)
        {
            if (grounded)
            {
                grounded = false;
            }
            if (forceGravity && !grounded)
            {
                rb.AddForce(Physics.gravity * gravityModifier);
            }
        }

        if (!sliding && TimeManager.CurrentGameTimeState != TimeManager.GameTimeState.Ending && !CustomizationTerminal.customizing)
        {
            Movement();
        }
        HeadBob();
    }

    private void Update()
    {
        if (TimeManager.CurrentGameTimeState == TimeManager.GameTimeState.Ending)
        {
            rb.velocity = Vector3.zero;
            return;
        }

        Debug.DrawRay(transform.position + new Vector3(0, .01F, 0), transform.forward, Color.red);

        RaycastHit hit;
        if (Physics.Raycast(transform.position + new Vector3(0, .01F, 0), transform.forward, out hit, .5F) || Physics.Raycast(transform.position + new Vector3(0, .01F, 0), -transform.forward, out hit, .5F) || Physics.Raycast(transform.position + new Vector3(0, .01F, 0), transform.right, out hit, .5F) || Physics.Raycast(transform.position + new Vector3(0, .01F, 0), -transform.right, out hit, .5F))
        {
            if (hit.transform.gameObject.isStatic && xMove != 0 || yMove !=  0)
            {
                if(hit.transform.gameObject.layer == 0)
                {
                    rb.AddRelativeForce(Vector3.up * slopeAssistForce, ForceMode.Impulse);
                }
            }
        }


            if (xMove != 0 && yMove != 0)
        {
            if (!isMoving)
            {
                isMoving = true;
            }
        }
        else
        {
            if (isMoving)
            {
                isMoving = false;
            }
        }

        if (Input.GetButtonDown("Jump") && canJump && TimeManager.CurrentGameTimeState != TimeManager.GameTimeState.Ending && !CustomizationTerminal.customizing)
        {
            if (sliding)
            {
                sliding = false;
            }
            JumpJet();
        }

        if (Input.GetButton("Jump") && !grounded && !sliding && !CustomizationTerminal.customizing)
        {
            WallRun();
        }
       if(Input.GetButtonUp("Jump") && !rb.useGravity && wallrunning)
        {
            rb.useGravity = true;
            forceGravity = true;
            wallrunning = false;

            if (wallRunningLeft)
            {
                rb.AddRelativeForce(Vector3.right * wallRunReleaseForce, ForceMode.Impulse);
            }
            else if (!wallRunningLeft)
            {
                rb.AddRelativeForce(Vector3.left * wallRunReleaseForce, ForceMode.Impulse);
            }
            if (wallrunningBack)
            {
                rb.AddRelativeForce(Vector3.forward * wallRunReleaseForce, ForceMode.Impulse);
            }

            rb.AddRelativeForce(Vector3.down * wallRunReleaseForce / 2, ForceMode.Impulse);
        }
        if (Input.GetKeyDown(KeyCode.LeftControl))
        {
            if (!sliding && TimeManager.CurrentGameTimeState != TimeManager.GameTimeState.Ending && !CustomizationTerminal.customizing)
            {
                Slide();
            }
        }

    }

    private void LateUpdate()
    {
        if(!CustomizationTerminal.customizing)
        {
            CameraRotation();
        }
        else
        {
            if (spine.eulerAngles != new Vector3(-15, spine.eulerAngles.y, spine.eulerAngles.z))
            {
                spine.eulerAngles = new Vector3(-15, spine.eulerAngles.y, spine.eulerAngles.z);
            }
        }
    }

    /// <summary>
    /// The players regular movement
    /// </summary>
    private void Movement()
    {
        xMove = Input.GetAxis("Horizontal");
        yMove = Input.GetAxis("Vertical");

        Vector3 movement = new Vector3(xMove, 0, yMove);

        if (xMove == 0 && yMove == 0 && grounded)
        {
            if (rb.velocity != Vector3.zero)
            {
                rb.velocity = Vector3.zero;
            }

            for (int i = 0; i < cameras.Length; i++)
            {
                if (cameras[i].fieldOfView != fovNotMoving)
                {
                    cameras[i].fieldOfView = Mathf.Lerp(cameras[i].fieldOfView, fovNotMoving, fovSprintBoostLerpSpeed * Time.deltaTime);
                }
            }
        }
        else
        {
            for (int i = 0; i < cameras.Length; i++)
            {
                if (cameras[i].fieldOfView != fovNotMoving && xMove != 0 || yMove != 0)
                {
                    cameras[i].fieldOfView = Mathf.Lerp(cameras[i].fieldOfView, fovMoving, fovSprintBoostLerpSpeed * Time.deltaTime);
                }
            }
        }

        rb.AddRelativeForce(movement * (wallrunning ? wallRunSpeed : moveSpeed));
    }

    /// <summary>
    /// The cameras rotation
    /// </summary>
    private void CameraRotation()
    {
        float x = Input.GetAxis("Mouse X") * myCamRotateSpeed * Time.deltaTime * GetSettingsManagerMouseSens();
        float y = Input.GetAxis("Mouse Y") * myCamRotateSpeed * Time.deltaTime * GetSettingsManagerMouseSens();

        if (canUseCam)
        {
            transform.Rotate(new Vector3(0, x, 0));

            currentAngle += Input.GetAxis("Mouse Y") * myCamRotateSpeed * Time.deltaTime * GetSettingsManagerMouseSens();
            if(currentAngle >= angleLimit)
            {
                currentAngle = angleLimit;
            }
            else if(currentAngle <= -angleLimit)
            {
                currentAngle = -angleLimit;
            }
        }

        if (TimeManager.CurrentGameTimeState == TimeManager.GameTimeState.Ending)
        {
            spine.eulerAngles = new Vector3(defaultAngleX, spine.eulerAngles.y, spine.eulerAngles.z);
        }
        else
        {
            spine.eulerAngles = new Vector3(-currentAngle, spine.eulerAngles.y, spine.eulerAngles.z);
        }

        // Old rotation stuff.
        //headObject.transform.localEulerAngles = new Vector3(-currentAngle, 0, 0);
        //spine.rotation = headObject.transform.rotation;
        //spine.localEulerAngles = headObject.transform.localEulerAngles;
        //spine.Rotate(headObject.transform.localEulerAngles);
    }

    /// <summary>
    /// The players jetpack movement
    /// </summary>
    private void JumpJet()
    {
        
        canJump = false;

        OnJump();

        float x = Input.GetAxis("Horizontal");
        float y = Input.GetAxis("Vertical");

        float upForce = verticalJumpForce;

        if (divideUpForce && upForce != upForce / divideUpForceBy)
        {
            upForce /= divideUpForceBy;
        }


        if (x < 0)
        {
            //Going left
            rb.AddRelativeForce(Vector3.up * upForce, ForceMode.Impulse);
            rb.AddRelativeForce(Vector3.left * directionalJumpForce, ForceMode.Impulse);


        }
        else if (x > 0)
        {
            //Going right
            rb.AddRelativeForce(Vector3.up * upForce, ForceMode.Impulse);
            rb.AddRelativeForce(Vector3.right * directionalJumpForce, ForceMode.Impulse);
        }
        else if (y < 0)
        {
            //Going back
            rb.AddRelativeForce(Vector3.up * upForce, ForceMode.Impulse);
            rb.AddRelativeForce(Vector3.back * directionalJumpForce, ForceMode.Impulse);
        }
        else if (y > 0 || y == 0 && x == 0)
        {
            //Going forward
            rb.AddRelativeForce(Vector3.up * upForce, ForceMode.Impulse);
            rb.AddRelativeForce(Vector3.forward * directionalJumpForce, ForceMode.Impulse);
        }



        if (!jumpCooldown)
        {
            if (!infinJet)
            {
                StartCoroutine(JumpCooldown(jumpCooldownTime));
            }
            else
            {
                StartCoroutine(JumpCooldown(jetBoostCooldownReduction));
            }
        }

    }
    private IEnumerator JumpCooldown(float f)
    {
        jumpCooldown = true;
    
        //GetComponent<PlayerPickUpManager>().StartCoroutine(GetComponent<PlayerPickUpManager>().JumpCooldownIcon(jumpCooldownTime));
        StartCoroutine(UIManager.instance.ShowJumpCooldownIcon(f));
        
        yield return new WaitForSeconds(f);
        jumpCooldown = false;
        canJump = true;

    }


    private void HeadBob()
    {
        float x = Input.GetAxis("Horizontal");
        float y = Input.GetAxis("Vertical");

        if(x != 0 || y != 0)
        {
            if(headObject.localPosition.y >= bobRestingPoint.y + maxBob)
            {
                topBob = true;
            }
            else if(headObject.localPosition.y <= bobRestingPoint.y + minBob)
            {
                topBob = false;
            }

            if (topBob)
            {
                headObject.localPosition = Vector3.Lerp(headObject.localPosition, new Vector3(headObject.localPosition.x, bobRestingPoint.y - bobLimit, headObject.localPosition.z), bobSpeed * Time.deltaTime);
            }
            else if (!topBob)
            {
                headObject.localPosition = Vector3.Lerp(headObject.localPosition, new Vector3(headObject.localPosition.x, bobRestingPoint.y + bobLimit, headObject.localPosition.z), bobSpeed * Time.deltaTime);
            }
        }
        else
        {
            headObject.localPosition = Vector3.Lerp(headObject.localPosition, bobRestingPoint, bobSpeed * Time.deltaTime);
        }
    }

    private void WallRun()
    {


        RaycastHit hit;
        if (Physics.Raycast(transform.position, -transform.right, out hit, wallRunRayDist) || Physics.Raycast(transform.position, -transform.forward, out hit, wallRunRayDist) && Input.GetKey("a"))
        {
            if (forceGravity)
            {
                forceGravity = false;
            }
            rb.useGravity = false;
            //  if (hit.transform.gameObject.isStatic)
            // {
            if (!wallrunning)
            {
                wallrunning = true;
            }

            if (!wallRunningLeft)
            {
                wallRunningLeft = true;
            }

            if (Physics.Raycast(transform.position, -transform.forward, out hit, wallRunRayDist * 2))
            {
                if (!wallrunningBack)
                {
                    wallrunningBack = true;
                }
            }
            else
            {
                if (wallrunningBack)
                {
                    wallrunningBack = false;
                }
            }

            rb.velocity = new Vector3(rb.velocity.x, 0, rb.velocity.z);
           // }
        }
        else if (Physics.Raycast(transform.position, transform.right, out hit, wallRunRayDist) || Physics.Raycast(transform.position, -transform.forward, out hit, wallRunRayDist) && Input.GetKey("d"))
        {
            if (forceGravity)
            {
                forceGravity = false;
            }
            rb.useGravity = false;
            //if (hit.transform.gameObject.isStatic)
            // {

            //if (hit.transform.gameObject.isStatic)
            // {
            if (!wallrunning)
            {
                wallrunning = true;
            }

            if (wallRunningLeft)
            {
                wallRunningLeft = false;
            }

            if(Physics.Raycast(transform.position, -transform.forward, out hit, wallRunRayDist * 2))
            {
                if (!wallrunningBack)
                {
                    wallrunningBack = true;
                }
            }
            else
            {
                if (wallrunningBack)
                {
                    wallrunningBack = false;
                }
            }

            rb.velocity = new Vector3(rb.velocity.x, 0, rb.velocity.z);
            // }
        }
        else if(hit.transform == null)
        {
            forceGravity = true;
            rb.useGravity = true;
            wallrunning = false;
            wallrunningBack = false;
        }
    }

    private void Slide()
    {
        sliding = true;
        //Change player collider

        OnSlide(true);

        rb.AddRelativeForce(Vector3.forward * slideForce, ForceMode.Impulse);


        StartCoroutine(SlideDuration());
    }

    private IEnumerator SlideDuration()
    {
        yield return new WaitForSeconds(PlayerManager.instance.playerAnimController.GetSlideDuration());
        sliding = false;
        OnSlide(false);
    }

    private float GetSettingsManagerMouseSens()
    {
        return DB.MenuPack.SettingsManager.instance ? DB.MenuPack.Setting_MouseSens.mouseSensitivity * 2 : 0;
    }

    public void ToggleInfiniteJetPack()
    {
        if (!infinJet)
        {
            infinJet = true;
        }
        else
        {
            infinJet = false;
        }
    }

   
}
