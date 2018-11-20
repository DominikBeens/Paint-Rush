using UnityEngine;

public class ReturnToPoolOnEndParticle : MonoBehaviour 
{

    [SerializeField] private string myPoolName;
    [SerializeField] private GameObject rootParent;

    private void OnParticleSystemStopped()
    {
        ObjectPooler.instance.AddToPool(myPoolName, rootParent);
    }
}
