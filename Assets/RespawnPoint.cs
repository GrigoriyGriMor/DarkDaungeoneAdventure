using PlayerControllers;
using UnityEngine;

public class RespawnPoint : MonoBehaviour
{
    [SerializeField] private Transform _respawnTransform;
    public Transform RespawnTransform { get => _respawnTransform; }

    [SerializeField] private ParticleSystem _activateEffect;
    [SerializeField] private ParticleSystem _respawnEffect;

    private void Start()
    {
        if (_respawnTransform == null)
            _respawnTransform = transform;    
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.TryGetComponent(out PlayerController controller))
        {
            controller.SetRespawnPoint(this);
            if (_activateEffect != null)
                _activateEffect.Play();
        }
    }

    public void Respawn()
    { 
        if (_respawnEffect != null)
            _respawnEffect.Play();
    }
}
