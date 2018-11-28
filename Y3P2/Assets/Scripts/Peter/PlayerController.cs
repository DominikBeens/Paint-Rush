using UnityEngine;
using System.Collections;
using System;
public class PlayerController : MonoBehaviour
{
    
    private bool initialised;

    [SerializeField]
    private float moveSpeed = 20;
    [SerializeField]
    private float fovNotMoving = 75;
    [SerializeField]
    private float fovSprintBoostLerpSpeed = 0.5F;
    private float defaultFOV;
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

    private bool topBob;
    [SerializeField]
    private float bobSpeed;
    [SerializeField]
    private float bobLimit = 0.7F;
    [SerializeField]
    private float bobRestingPoint = 0.7F;
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

    [SerializeField] private GameObject headObject;

    private Rigidbody rb;
    private bool grounded;
    private bool forceGravity = true;
    private float xMove;
    private float yMove;
    private bool canUseCam = true;

    private bool isMoving;
    public bool IsMoving { get { return isMoving; } }
    public event Action OnJump = delegate { };

    private bool infinJet;

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
        fovNotMoving = 75;

        rb = GetComponent<Rigidbody>();
        defaultFOV = GetComponentInChildren<Camera>().fieldOfView;
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
    }

    private void FixedUpdate()
    {
        RaycastHit hit;

        if (Physics.Raycast(transform.position, -Vector3.up, out hit, 1))
        {
            if (!grounded)
            {
                grounded = true;
            }
        }
        else if (hit.transform == null)
        {
            if (grounded)
            {
                grounded = false;
            }
            if (forceGravity)
            {
                rb.AddForce(Physics.gravity * gravityModifier);
            }
        }

        if (!sliding)
        {
            Movement();
        }
        HeadBob();
    }

    private void Update()
    {
        if(xMove != 0 && yMove != 0)
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

        if (canUseCam)
        {
            CameraRotation();
        }

        if (Input.GetButtonDown("Jump") && canJump)
        {
            if (sliding)
            {
                sliding = false;
            }
            JumpJet();
        }

        if (Input.GetButton("Jump") && !grounded && !sliding)
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
            if (!sliding)
            {
                Slide();
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
            rb.velocity = Vector3.zero;
            if (GetComponentInChildren<Camera>().fieldOfView != fovNotMoving)
            {
                GetComponentInChildren<Camera>().fieldOfView = Mathf.Lerp(GetComponentInChildren<Camera>().fieldOfView, fovNotMoving, fovSprintBoostLerpSpeed * Time.deltaTime);
            }
        }
        else if (GetComponentInChildren<Camera>().fieldOfView != defaultFOV && xMove != 0 || yMove != 0)
        {
            GetComponentInChildren<Camera>().fieldOfView = Mathf.Lerp(GetComponentInChildren<Camera>().fieldOfView, defaultFOV, fovSprintBoostLerpSpeed * Time.deltaTime);
        }
        rb.AddRelativeForce(movement * moveSpeed);
       
    }

    /// <summary>
    /// The cameras rotation
    /// </summary>
    private void CameraRotation()
    {
        float x = Input.GetAxis("Mouse X") * myCamRotateSpeed * Time.deltaTime * GetSettingsManagerMouseSens();
        float y = Input.GetAxis("Mouse Y") * myCamRotateSpeed * Time.deltaTime * GetSettingsManagerMouseSens();

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

        headObject.transform.localEulerAngles = new Vector3(-currentAngle, 0, 0); 
        
    }

    /// <summary>
    /// The players jetpack movement
    /// </summary>
    private void JumpJet()
    {
        if (!infinJet)
        {
            canJump = false;
        }

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
            StartCoroutine(JumpCooldown());
        }

    }
    private IEnumerator JumpCooldown()
    {
        jumpCooldown = true;
        yield return new WaitForSeconds(jumpCooldownTime);
        jumpCooldown = false;
        canJump = true;
    }


    private void HeadBob()
    {
       
        float x = Input.GetAxis("Horizontal");
        float y = Input.GetAxis("Vertical");

        if(x != 0 || y != 0)
        {
            if(headObject.transform.localPosition.y >= maxBob)
            {
                topBob = true;
            }
            else if(headObject.transform.localPosition.y <= minBob)
            {
                topBob = false;
            }
           

            if (topBob)
            {
                headObject.transform.localPosition = Vector3.Lerp(headObject.transform.localPosition, headObject.transform.localPosition + new Vector3(0,  -bobLimit, 0), bobSpeed * Time.deltaTime);
            }
            else if (!topBob)
            {
                headObject.transform.localPosition = Vector3.Lerp(headObject.transform.localPosition, headObject.transform.localPosition + new Vector3(0, bobLimit, 0), bobSpeed * Time.deltaTime);
            }
        }
        else
        {
            headObject.transform.localPosition = Vector3.Lerp(headObject.transform.localPosition, new Vector3(0, bobRestingPoint, 0), bobSpeed * Time.deltaTime);
        }


    }

    private void WallRun()
    {

        if (forceGravity)
        {
            forceGravity = false;
        }
        RaycastHit hit;
        rb.useGravity = false;
        if (Physics.Raycast(transform.position, -transform.right, out hit, wallRunRayDist) || Physics.Raycast(transform.position, -transform.forward, out hit, wallRunRayDist) && Input.GetKey("a"))
        {
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
            wallrunning = false;
            wallrunningBack = false;
        }
    }

    private void Slide()
    {
        sliding = true;
        //Change player collider

        //Play animation

        rb.AddRelativeForce(Vector3.forward * slideForce, ForceMode.Impulse);


        StartCoroutine(SlideDuration());
    }

    private IEnumerator SlideDuration()
    {
        yield return new WaitForSeconds(.5F); //Make slide animation duration
        sliding = false;
    }

    private float GetSettingsManagerMouseSens()
    {
        return DB.MenuPack.SettingsManager.instance ? DB.MenuPack.Setting_MouseSens.mouseSensitivity : 0;
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
