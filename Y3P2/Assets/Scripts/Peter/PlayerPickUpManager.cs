using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
public class PlayerPickUpManager : MonoBehaviour {

    

    private void Start()
    {
        UIManager.instance.JumpCooldownIcon.SetActive(false);
    }

    public IEnumerator JumpCooldownIcon(float coolDowntime)
    {
        UIManager.instance.JumpCooldownIcon.SetActive(true);
        yield return new WaitForSeconds(coolDowntime);
        UIManager.instance.JumpCooldownIcon.SetActive(false);
    }
}
