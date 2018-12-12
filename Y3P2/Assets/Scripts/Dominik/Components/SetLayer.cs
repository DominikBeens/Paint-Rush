using UnityEngine;

public class SetLayer : MonoBehaviour
{

    [SerializeField] private int setToLayer;
    [SerializeField] private bool recursive;
    [SerializeField] private bool onlyChangeIfIsNotMine = true;

    private void Awake()
    {
        if (onlyChangeIfIsNotMine)
        {
            Photon.Pun.PhotonView pv = transform.root.GetComponentInChildren<Photon.Pun.PhotonView>();
            if (pv && !pv.IsMine)
            {
                SetLayerTo(transform, setToLayer);
            }
        }
        else
        {
            SetLayerTo(transform, setToLayer);
        }
    }

    private void SetLayerTo(Transform transform, int layer)
    {
        transform.gameObject.layer = layer;

        if (recursive)
        {
            foreach (Transform child in transform)
            {
                SetLayerTo(child, layer);
            }
        }
    }
}
