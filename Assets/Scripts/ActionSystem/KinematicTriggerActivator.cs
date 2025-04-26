using UnityEngine;

public class KinematicTriggerActivator : MonoBehaviour
{
    [SerializeField] private Rigidbody _rb;

    private void OnTriggerEnter(Collider other)
    {
        _rb.isKinematic = true;
    }
}
