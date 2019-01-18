using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Sway : MonoBehaviour {

    private Animator myanim;

    float x;
    float y;
    float lean;
    public float lerpSpeed = 3f;
    public float leanLerpSpeed = 3f;

    // Use this for initialization
    void Start () {
        myanim = GetComponent<Animator>();

        WeaponSlot.OnFireWeapon += WeaponSlot_OnFireWeapon;
	}

    private void WeaponSlot_OnFireWeapon()
    {
        myanim.SetTrigger("Recoil");
    }

    // Update is called once per frame
    void Update() {

        x = Mathf.Lerp(x, Input.GetAxis("Mouse X") * 0.5f, Time.deltaTime * lerpSpeed);
        y = Mathf.Lerp(y, Input.GetAxis("Mouse Y") * 0.5f, Time.deltaTime * lerpSpeed);
        lean = Mathf.Lerp(lean, Input.GetAxis("Horizontal"), Time.deltaTime * leanLerpSpeed);

        myanim.SetFloat("SwayHor", x);
        myanim.SetFloat("SwayVer", y);

        myanim.SetFloat("LeanHor", lean);

        if (Input.GetButtonDown("Jump"))
        {
           // y = -1;
        }

    }
}
