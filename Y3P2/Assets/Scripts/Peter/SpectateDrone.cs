using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
public class SpectateDrone : MonoBehaviour {
    [SerializeField]
    private float moveSpeed = 20;

    [SerializeField]
    private float rotSpeed = 20;

    
    // Update is called once per frame
    void FixedUpdate () {
        float x = Input.GetAxis("Horizontal");
        float y = Input.GetAxis("Vertical");

        transform.Translate(new Vector3(x, 0 , y) * moveSpeed * Time.deltaTime);

     

        float xx = Input.GetAxis("Mouse X");
        float yy = Input.GetAxis("Mouse Y");

        transform.Rotate(new Vector3(-yy, 0, 0) * rotSpeed * Time.deltaTime);
        transform.Rotate(new Vector3(0, xx, 0) * rotSpeed * Time.deltaTime, Space.World);
        if (Input.GetKey(KeyCode.Space))
        {
            transform.Translate(Vector3.up * 1 * moveSpeed * Time.deltaTime);
        }
        else if (Input.GetKey(KeyCode.LeftControl))
        {
            transform.Translate(Vector3.up * -1 * moveSpeed * Time.deltaTime);

        }
    }

    private void Update()
    {
        if (PhotonNetwork.LocalPlayer.IsLocal)
        {
            if (Input.GetKeyDown("e"))
            {
                PhotonNetwork.Destroy(gameObject);
            }
        }
           
    }
}
