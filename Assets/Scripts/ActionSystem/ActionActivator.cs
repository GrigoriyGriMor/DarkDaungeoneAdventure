using UnityEngine;
using UnityEngine.Events;

public class ActionActivator : MonoBehaviour
{
    [SerializeField] private int[] _triggersAccesID = new int[0];

    [SerializeField] UnityEvent _activateEvent = new UnityEvent();

    public void OnCollisionEnter(Collision collision)
    {
        if (collision.collider.TryGetComponent(out ActionTrigger trigger))
            for (int i = 0; i < _triggersAccesID.Length; i++)
                if (_triggersAccesID[i] == trigger.GetTriggerID())
                    _activateEvent.Invoke();
    }

    public void OnTriggerEnter(Collider collision)
    {
        if (collision.TryGetComponent(out ActionTrigger trigger))
            for (int i = 0; i < _triggersAccesID.Length; i++)
                if (_triggersAccesID[i] == trigger.GetTriggerID())
                    _activateEvent.Invoke();
    }
}
