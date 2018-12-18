using UnityEngine;

[CreateAssetMenu(menuName ="Items/PickUp")]
public class PickUp : Item {

    public enum PickUpType
    {
        InfiniteJetpack,
        Cloak,
        PulseRemote,
        ColorVac
    }

    [SerializeField]
    private PickUpType pickUpType;
    public PickUpType Type { get { return pickUpType; } }
    [SerializeField]
    private float duration;
    public float Duration { get { return duration; } }

    [SerializeField]
    private float damage;
    public float Damage { get { return damage; } }

    [SerializeField]
    private Sprite pickUpSprite;
    public Sprite PickUpSprite { get { return pickUpSprite; } }

    [SerializeField]
    private string pickUpText;
    public string PickUpText { get { return pickUpText; } }
}
