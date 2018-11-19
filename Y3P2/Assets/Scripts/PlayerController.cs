using UnityEngine;

public class PlayerController : MonoBehaviour
{

    private bool initialised;

    [SerializeField]
    private float moveSpeed = 20;
    [SerializeField]
    private float myCamRotateSpeed = 80;

    private Camera myCamera;
    private Rigidbody rb;

    public void Inititalise(bool local)
    {
        if (!local)
        {
            enabled = false;
            return;
        }
    }

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
        float y = Input.GetAxis("Mouse Y") * myCamRotateSpeed * Time.deltaTime;

        transform.Rotate(new Vector3(0, x, 0));

        myCamera.transform.Rotate(new Vector3(-y, 0, 0));
    }
}
