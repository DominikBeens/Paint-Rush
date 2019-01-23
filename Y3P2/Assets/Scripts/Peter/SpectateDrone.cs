using Photon.Pun;
using UnityEngine;

public class SpectateDrone : MonoBehaviourPunCallbacks
{

    private Camera cam;
    private bool canRotate = true;

    [SerializeField]
    private float moveSpeed = 20;

    [SerializeField]
    private float rotSpeed = 20;

    [SerializeField]
    private Rigidbody rb;

    private void Awake()
    {
        cam = GetComponentInChildren<Camera>();
        cam.enabled = photonView.IsMine;

        if (photonView.IsMine)
        {
            GameManager.OnGameStateChanged += GameManager_OnGameStateChanged;
            DB.MenuPack.SceneManager.OnGamePaused += SceneManager_OnGamePaused;
        }
    }

    private void Update()
    {
        if (photonView.IsMine)
        {
            if (Input.GetKeyDown("e"))
            {
                GameManager.CurrentGameSate = GameManager.GameState.Lobby;
            }
        }
    }

    private void FixedUpdate()
    {
        if (photonView.IsMine)
        {
            float x = Input.GetAxis("Horizontal");
            float z = Input.GetAxis("Vertical");

            //transform.Translate(new Vector3(x, 0 , y) * moveSpeed * Time.deltaTime);
            Vector3 hMove = transform.right * x * moveSpeed * Time.deltaTime;
            Vector3 vMove = transform.forward * z * moveSpeed * Time.deltaTime;
            rb.MovePosition(rb.position + vMove + hMove);

            float xx = Input.GetAxis("Mouse X");
            float yy = Input.GetAxis("Mouse Y");

            if (canRotate)
            {
                transform.Rotate(new Vector3(-yy, 0, 0) * rotSpeed * Time.deltaTime);
                transform.Rotate(new Vector3(0, xx, 0) * rotSpeed * Time.deltaTime, Space.World);
            }

            if (Input.GetKey(KeyCode.Space))
            {
                //transform.Translate(Vector3.up * 1 * moveSpeed * Time.deltaTime);
                rb.MovePosition(transform.localPosition + transform.up * 1 * moveSpeed * Time.deltaTime);
            }
            else if (Input.GetKey(KeyCode.LeftControl))
            {
                // transform.Translate(Vector3.up * -1 * moveSpeed * Time.deltaTime);
                rb.MovePosition(transform.localPosition + transform.up * -1 * moveSpeed * Time.deltaTime);
            }
        }
    }

    private void GameManager_OnGameStateChanged(GameManager.GameState newState)
    {
        if (newState == GameManager.GameState.Lobby)
        {
            PhotonNetwork.Destroy(gameObject);
        }
    }

    private void SceneManager_OnGamePaused(bool b)
    {
        canRotate = !b;
    }

    public override void OnDisable()
    {
        if (photonView.IsMine)
        {
            GameManager.OnGameStateChanged -= GameManager_OnGameStateChanged;
            DB.MenuPack.SceneManager.OnGamePaused -= SceneManager_OnGamePaused;
        }
    }
}
