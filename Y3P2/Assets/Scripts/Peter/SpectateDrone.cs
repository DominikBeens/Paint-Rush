using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
public class SpectateDrone : MonoBehaviourPunCallbacks {
    [SerializeField]
    private float moveSpeed = 20;

    [SerializeField]
    private float rotSpeed = 20;

    [SerializeField]
    private Rigidbody rb;
    
    // Update is called once per frame
    void FixedUpdate () {

        if (PhotonNetwork.LocalPlayer.IsLocal)
        {
            float x = Input.GetAxis("Horizontal");
            float z = Input.GetAxis("Vertical");



            //transform.Translate(new Vector3(x, 0 , y) * moveSpeed * Time.deltaTime);
            Vector3 hMove = transform.right * x * moveSpeed * Time.deltaTime;
            Vector3 vMove = transform.forward * z * moveSpeed * Time.deltaTime;
            rb.MovePosition(rb.position + vMove + hMove);

            float xx = Input.GetAxis("Mouse X");
            float yy = Input.GetAxis("Mouse Y");

            transform.Rotate(new Vector3(-yy, 0, 0) * rotSpeed * Time.deltaTime);
            transform.Rotate(new Vector3(0, xx, 0) * rotSpeed * Time.deltaTime, Space.World);
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

    private void Update()
    {
        if (PhotonNetwork.LocalPlayer.IsLocal)
        {
            if (Input.GetKeyDown("e"))
            {
                PhotonNetwork.Destroy(gameObject);
                GameManager.CurrentGameSate = GameManager.GameState.Lobby;
            }
        }
           
    }

    public void DisableCam()
    {
        photonView.RPC("DisableCamOthers", RpcTarget.Others);
    }

    [PunRPC]
    private void DisableCamOthers()
    {
        GetComponentInChildren<Camera>().enabled = false;
    }
}
