using UnityEngine;

public class PlayerController : MonoBehaviour
{

    private bool initialised;

    [SerializeField]
    private float moveSpeed = 20;
    [SerializeField]
    private float jumpForce = 2000;
    [SerializeField]
    private float myCamRotateSpeed = 80;
<<<<<<< HEAD
=======
    private float angleLimit = 70;
    private float currentAngle;
>>>>>>> playercontroller

    private Camera myCamera;
    private Rigidbody rb;

<<<<<<< HEAD
    public void Inititalise(bool local)
    {
        if (!local)
        {
            enabled = false;
            return;
        }
    }
=======
    private bool canJump = true;
    private bool jumpCooldown;

    [SerializeField]
    private float jumpCooldownTime = 6;
>>>>>>> playercontroller

    private void Start()
    {
        rb = GetComponent<Rigidbody>();
        myCamera = GetComponentInChildren<Camera>();


        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
    }

    private void FixedUpdate()
    {
        Movement();
      
    }

    private void Update()
    {
        CameraRotation();

        if (Input.GetButtonDown("Jump") && canJump)
        {
            JumpJet();
        }
        RaycastHit hit;

        if (Physics.Raycast(transform.position, -Vector3.up, out hit, 1))
        {
            
        }
        else if(hit.transform == null)
        {
            rb.AddRelativeForce(Vector3.down *- Physics.gravity.y);
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

        rb.AddRelativeForce(movement * moveSpeed);
    }

    /// <summary>
    /// The cameras rotation
    /// </summary>
    private void CameraRotation()
    {
        float x = Input.GetAxis("Mouse X") * myCamRotateSpeed * Time.deltaTime;
<<<<<<< HEAD
        float y = Input.GetAxis("Mouse Y") * myCamRotateSpeed * Time.deltaTime;
=======
>>>>>>> playercontroller

        transform.Rotate(new Vector3(0, x, 0));


        currentAngle += Input.GetAxis("Mouse Y") * myCamRotateSpeed * Time.deltaTime;
        if(currentAngle >= angleLimit)
        {
            currentAngle = angleLimit;
        }
        else if(currentAngle <= -angleLimit)
        {
            currentAngle = -angleLimit;
        }

        myCamera.transform.localEulerAngles = new Vector3(-currentAngle, 0, 0); 
        
    }

    /// <summary>
    /// The players jetpack movement
    /// </summary>
    private void JumpJet()
    {
            canJump = false;
       
            float x = Input.GetAxis("Horizontal");
            float y = Input.GetAxis("Vertical");

            float upForce = jumpForce / 2;
            if (x < 0)
            {
                //Going left
                rb.AddRelativeForce(Vector3.up * upForce);
                rb.AddRelativeForce(Vector3.left * jumpForce);

            }
            else if(x > 0)
            {
                //Going right
                rb.AddRelativeForce(Vector3.up * upForce);
                rb.AddRelativeForce(Vector3.right * jumpForce);
            }

            if(y < 0)
            {
                //Going back
                rb.AddRelativeForce(Vector3.up * upForce);
                rb.AddRelativeForce(Vector3.back * jumpForce);
            }
            else if(y > 0)
            {
                //Going forward
                rb.AddRelativeForce(Vector3.up * upForce);
                rb.AddRelativeForce(Vector3.forward * jumpForce);
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
}
