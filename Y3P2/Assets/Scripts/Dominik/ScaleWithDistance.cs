using UnityEngine;

public class ScaleWithDistance : MonoBehaviour
{

    private Transform target;
    private Vector3 targetFixedPos;
    private float originalSize;

    [SerializeField] private float scaleModifier = 1f;

    private void Start()
    {
        target = PlayerManager.instance.transform;
        originalSize = transform.localScale.x;
    }

    private void LateUpdate()
    {
        if (!target)
        {
            return;
        }

        Vector3 directionToTarget = target.position - transform.position;
        float dSqrToTarget = directionToTarget.sqrMagnitude;

        transform.localScale = new Vector3(Mathf.Clamp((dSqrToTarget * scaleModifier / 1000), originalSize, 100), Mathf.Clamp((dSqrToTarget * scaleModifier / 1000), originalSize, 100), 1);
    }
}
