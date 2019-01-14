using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RegularTerminal : MonoBehaviour {
    [SerializeField]
    private GameObject terminalCamera;
    public GameObject TerminalCamera { get { return terminalCamera; } }

    private bool interacting;
    // Use this for initialization
    void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
        if (interacting)
        {
            if (Input.GetKeyDown("e"))
            {
                interacting = false;
                Cursor.visible = false;
                Cursor.lockState = CursorLockMode.Locked;

                terminalCamera.SetActive(false);
              
                UIManager.instance.ToggleCrosshair(true);

            }
        }
    }

    public void ReleaseMouse()
    {
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;
        interacting = true;

        terminalCamera.SetActive(true);

        UIManager.instance.ToggleCrosshair(false);
    }
}
