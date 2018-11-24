using UnityEngine;

public class BulletTrail : MonoBehaviour
{

    private LineRenderer lineRenderer;
    private Material lineMat;

    [SerializeField] private string myPoolName;
    [SerializeField] private float alphaFadeSpeed = 10f;

    private void Awake()
    {
        lineRenderer = GetComponent<LineRenderer>();
        lineMat = lineRenderer.material;
    }

    public void Initialise(Vector3 start, Vector3 end, Color color)
    {
        lineRenderer.SetPosition(0, start);
        lineRenderer.SetPosition(1, end);

        // Reset alpha.
        lineMat.color = new Color(lineMat.color.r, lineMat.color.g, lineMat.color.b, 255);

        lineRenderer.startColor = color;
        lineRenderer.endColor = color;
        //lineMat.color = color;

        Invoke("ReturnToPool", 1);
    }

    private void Update()
    {
        if (gameObject.activeInHierarchy)
        {
            lineMat.color = new Color(lineMat.color.r, lineMat.color.g, lineMat.color.b, lineMat.color.a - Time.deltaTime * alphaFadeSpeed);
        }
    }

    private void ReturnToPool()
    {
        ObjectPooler.instance.AddToPool(myPoolName, gameObject);
    }
}
