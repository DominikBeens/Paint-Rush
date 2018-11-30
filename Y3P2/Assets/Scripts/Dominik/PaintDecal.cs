using System.Collections.Generic;
using UnityEngine;

public class PaintDecal : MonoBehaviour
{

    private Projector projector;

    [SerializeField] private List<DecalData> decalData = new List<DecalData>();

    [System.Serializable]
    public struct DecalData
    {
        public Material decalMat;
        public Color decalColor;
    }

    private void Awake()
    {
        projector = GetComponentInChildren<Projector>();
    }

    public void Initialise(Color color)
    {
        projector.material = GetDecalMat(color);

        RaycastHit hit;
        if (Physics.Raycast(projector.transform.position, projector.transform.forward, out hit, 0.3f))
        {
            transform.SetParent(hit.transform);
        }
    }

    private Material GetDecalMat(Color32 color)
    {
        for (int i = 0; i < decalData.Count; i++)
        {
            if (decalData[i].decalColor == color)
            {
                return decalData[i].decalMat;
            }
        }

        Debug.LogWarning("Couldnt find decal material.");
        return null;
    }
}
