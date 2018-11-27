using UnityEngine;

public class PaintDecal : MonoBehaviour
{

    private Projector projector;
    private Material mat;

    private void Awake()
    {
        projector = GetComponentInChildren<Projector>();
        mat = Instantiate(projector.material);
        projector.material = mat;
    }

    public void Initialise(Color color)
    {
        mat.color = color;

        RaycastHit hit;
        if (Physics.Raycast(projector.transform.position, projector.transform.forward, out hit, 0.3f))
        {
            transform.SetParent(hit.transform);
        }
    }
}
