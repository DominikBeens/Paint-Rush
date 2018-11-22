using UnityEngine;
using System.Collections;
public class PlayerController : MonoBehaviour
{

    private bool initialised;

    [SerializeField]
    private float moveSpeed = 20;
    [SerializeField]
    private float sprintSpeed = 120;
    [SerializeField]
    private float fovSprintBoost = 35;
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
    [SerializeField] private GameObject headObject;

    private Rigidbody rb;
    private bool grounded;


    public void Inititalise(bool local)
    {
        if (!local)
        {
            enabled = false;
            return;
        }
    }
    private bool canJump = true;
    private bool jumpCooldown;

    [SerializeField]
    private float jumpCooldownTime = 6;

    private void Start()
    {
        rb = GetComponent<Rigidbody>();
        defaultFOV = GetComponentInChildren<Camera>().fieldOfView;
        fovSprintBoost += defaultFOV;
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
            Vector3 v = rb.velocity;
            v.y += Physics.gravity.y * gravityModifier * Time.fixedDeltaTime;
            v.z += Physics.gravity.z * gravityModifier * Time.fixedDeltaTime;
            v.x += Physics.gravity.x * gravityModifier * Time.fixedDeltaTime;

            rb.velocity = v;
            rb.AddRelativeForce(Vector3.down * -Physics.gravity.y * gravityModifier);
        }

        Movement();
        HeadBob();
    }

    private void Update()
    {
        CameraRotation();

        if (Input.GetButtonDown("Jump") && canJump)
        {
            JumpJet();
        }
      
       

    }

    /// <summary>
    /// The players regular movement
    /// </summary>
    private void Movement()
    {
        float x = Input.GetAxis("Horizontal");
        float y = Input.GetAxis("Vertical");


        Vector3 movement = new Vector3(x, 0, y);

        if(x == 0 && y == 0 && grounded)
        {
            rb.velocity = Vector3.zero;
        }

        if (!Input.GetKey(KeyCode.LeftShift))
        {
            rb.AddRelativeForce(movement * moveSpeed);
            if(GetComponentInChildren<Camera>().fieldOfView != defaultFOV)
            {
                GetComponentInChildren<Camera>().fieldOfView = Mathf.Lerp(GetComponentInChildren<Camera>().fieldOfView, defaultFOV, fovSprintBoostLerpSpeed * Time.deltaTime);
            }
        }
        else
        {
            rb.AddRelativeForce(movement * sprintSpeed);
            if (GetComponentInChildren<Camera>().fieldOfView != fovSprintBoost)
            {
                GetComponentInChildren<Camera>().fieldOfView = Mathf.Lerp(GetComponentInChildren<Camera>().fieldOfView, fovSprintBoost, fovSprintBoostLerpSpeed * Time.deltaTime);
            }
        }
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
        canJump = false;

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
        else if (y > 0)
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

    private float GetSettingsManagerMouseSens()
    {
        if (!DB.MenuPack.SettingsManager.instance)
        {
            return 0;
        }

        return DB.MenuPack.Setting_MouseSens.mouseSensitivity;
    }
}
