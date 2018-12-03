using UnityEngine;
using UnityEngine.Events;

public class CollisionEventZone : MonoBehaviour
{

    private enum EventType { Enter, Exit };
    private enum CollisionType { Trigger, Collider };
    private bool canCheckForInput;

    [SerializeField] private CollisionType collisionType;
    [SerializeField] private string objectTagToLookFor;
    
    private Transform eventCaller;
    public Transform EventCaller
    {
        get { return eventCaller; }
        private set { eventCaller = value; }
    }

    public UnityEvent OnZoneEnter;
    public UnityEvent OnZoneExit;

    [Space(10)]

    [SerializeField] private KeyCode key;
    public UnityEvent OnKeyDown;

    private void Update()
    {
        if (canCheckForInput)
        {
            if (Input.GetKeyDown(key))
            {
                OnKeyDown.Invoke();
            }
        }
    }

    private void CallEvent(EventType type, Transform eventCaller)
    {
        this.eventCaller = eventCaller;

        switch (type)
        {
            case EventType.Enter:

                OnZoneEnter.Invoke();
                break;
            case EventType.Exit:

                OnZoneExit.Invoke();
                break;
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (collisionType != CollisionType.Trigger)
        {
            return;
        }

        if (other.tag == objectTagToLookFor && other.gameObject.layer != 10)
        {
            CallEvent(EventType.Enter, other.transform);
            canCheckForInput = true;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (collisionType != CollisionType.Trigger)
        {
            return;
        }

        if (other.tag == objectTagToLookFor && other.gameObject.layer != 10)
        {
            CallEvent(EventType.Exit, other.transform);
            canCheckForInput = false;
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collisionType != CollisionType.Collider)
        {
            return;
        }

        if (collision.transform.tag == objectTagToLookFor && collision.gameObject.layer != 10)
        {
            CallEvent(EventType.Enter, collision.transform);
            canCheckForInput = true;
        }
    }

    private void OnCollisionExit(Collision collision)
    {
        if (collisionType != CollisionType.Collider)
        {
            return;
        }

        if (collision.transform.tag == objectTagToLookFor && collision.gameObject.layer != 10)
        {
            CallEvent(EventType.Exit, collision.transform);
            canCheckForInput = false;
        }
    }
}
