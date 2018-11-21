using UnityEngine;
using TMPro;

public class Notification : MonoBehaviour
{

    [SerializeField] private TextMeshProUGUI notificationText;

    public void Initialise(string text)
    {
        notificationText.text = text;
    }

    public void AnimationEventReturnToPool()
    {
        ObjectPooler.instance.AddToPool("Notification", gameObject);
    }
}
