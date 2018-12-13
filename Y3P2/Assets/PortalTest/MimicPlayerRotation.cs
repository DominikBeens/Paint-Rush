using UnityEngine;

public class MimicPlayerRotation : MonoBehaviour
{

    private Transform target;

    private void Start()
    {
        target = Camera.main.transform;
    }

    private void Update()
    {
        if (!target)
        {
            return;
        }

        transform.rotation = target.rotation;
    }
}
