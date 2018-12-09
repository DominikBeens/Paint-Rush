using UnityEngine;

public class PositionUpdater : MonoBehaviour
{

    [SerializeField] private Transform target;

    private void LateUpdate()
    {
        if (!target)
        {
            return;
        }

        transform.position = new Vector3(target.position.x, target.position.y, target.position.z);
    }
}
