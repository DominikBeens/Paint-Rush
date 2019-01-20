using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class AudioController : MonoBehaviour
{

    private AudioSource audioSource;
    private float defaultPitch;
    private Transform target;

    [SerializeField] private bool randomizePitch = true;
    [Range(-3, 3)] [SerializeField] private float minPitch = 0.9f;
    [Range(-3, 3)] [SerializeField] private float maxPitch = 1.1f;

    [Space]

    [SerializeField] private string myPoolName;
    [SerializeField] private float disableTimer = 2f;

    private void Awake()
    {
        audioSource = GetComponent<AudioSource>();
        defaultPitch = audioSource.pitch;
    }

    public void Play(Transform target = null)
    {
        RandomizePitch();
        audioSource.Play();
        this.target = target ? target : null;
    }

    private void OnEnable()
    {
        if (audioSource.playOnAwake)
        {
            RandomizePitch();
        }

        if (disableTimer != 0 || string.IsNullOrEmpty(myPoolName))
        {
            Invoke("ReturnToPool", disableTimer);
        }
    }


    private void LateUpdate()
    {
        if (target)
        {
            transform.position = target.position;
        }
    }

    private void RandomizePitch()
    {
        audioSource.pitch = randomizePitch ? Random.Range(minPitch, maxPitch) : defaultPitch;
    }

    private void ReturnToPool()
    {
        target = null;
        ObjectPooler.instance.AddToPool(myPoolName, gameObject);
    }
}
