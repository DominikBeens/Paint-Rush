using UnityEngine;

[CreateAssetMenu(menuName ="Items/PickUp")]
public class PickUp : Item {

    public enum PickUpType
    {
        InfiniteJetpack,
        Cloak
    }

    [SerializeField]
    private PickUpType pickUpType;
    public PickUpType Type { get { return pickUpType; } }
    [SerializeField]
    private int duration;
    public int Duration { get { return duration; } }
}
