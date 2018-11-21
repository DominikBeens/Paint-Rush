using UnityEngine;

public class LookAtCamera : MonoBehaviour
{

    private Transform target;
    private Vector3 targetFixedPos;

    private void Start()
    {
        target = Camera.main.transform;
    }

    private void LateUpdate()
    {
        if (!target)
        {
            return;
        }

        targetFixedPos = target.position;
        targetFixedPos.y = transform.position.y;

        transform.LookAt(targetFixedPos);
        transform.Rotate(0, 180, 0);
    }
}
