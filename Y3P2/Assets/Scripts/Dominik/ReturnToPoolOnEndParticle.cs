using UnityEngine;

public class ReturnToPoolOnEndParticle : MonoBehaviour 
{

    [SerializeField] private string myPoolName;

    private void OnParticleSystemStopped()
    {
        ObjectPooler.instance.AddToPool(myPoolName, transform.root.gameObject);
    }
}
