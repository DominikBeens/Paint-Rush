using System.Collections.Generic;
using UnityEngine;

public class PaintDecal : MonoBehaviour
{

    private Projector projector;
    private float defaultSize;

    [SerializeField] private float randomSizeRange = 0.25f;
    [SerializeField] private List<DecalData> decalData = new List<DecalData>();

    [Space]

    [SerializeField] private LayerMask environmentMask;
    [SerializeField] private LayerMask otherPlayerMask;
    [SerializeField] private LayerMask playerMask;

    [System.Serializable]
    public struct DecalData
    {
        public Material decalMat;
        public Color decalColor;
    }

    private void Awake()
    {
        projector = GetComponentInChildren<Projector>();
        defaultSize = transform.localScale.x;
    }

    public void Initialise(Color color)
    {
        projector.material = GetDecalMat(color);
        RandomizeSize();
        RandomizeRotation();

        RaycastHit hit;
        if (Physics.Raycast(projector.transform.position, projector.transform.forward, out hit, 0.3f))
        {
            transform.SetParent(hit.transform);

            // TEMP
            projector.ignoreLayers = otherPlayerMask;

            //switch (transform.gameObject.layer)
            //{
            //    case 0:
            //        projector.ignoreLayers = environmentMask;
            //        break;

            //    case 10:
            //        projector.ignoreLayers = otherPlayerMask;
            //        break;

            //    default:
            //        projector.ignoreLayers = playerMask;
            //        break;
            //}
        }
        else
        {
            //ObjectPooler.instance.AddToPool("PaintDecal", gameObject);
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

    private void RandomizeSize()
    {
        float randomSize = Random.Range(defaultSize - randomSizeRange, defaultSize + randomSizeRange);
        transform.localScale = new Vector3(randomSize, randomSize, randomSize);
    }

    private void RandomizeRotation()
    {
        transform.localEulerAngles = new Vector3(transform.localEulerAngles.x, transform.localEulerAngles.y, Random.Range(0, 360));
    }
}
