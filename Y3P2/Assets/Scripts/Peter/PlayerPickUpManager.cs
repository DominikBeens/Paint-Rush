using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
public class PlayerPickUpManager : MonoBehaviour {

    [SerializeField]
    private Material cloakShader;
    public Material CloakShader { get { return cloakShader; } }

    [SerializeField]
    public List<GameObject> objectsToCloak = new List<GameObject>();
    public List<GameObject> ObjectsToCloak { get { return ObjectsToCloak; } }


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

    public void CheckChildren()
    {
        GetDefaultMat[] mats = transform.GetComponentsInChildren<GetDefaultMat>();

        foreach (GetDefaultMat df in mats)
        {
            if (df != null)
            {
                if (!objectsToCloak.Contains(df.gameObject))
                {
                    objectsToCloak.Add(df.gameObject);
                }
            }
        }
    }
   
}
