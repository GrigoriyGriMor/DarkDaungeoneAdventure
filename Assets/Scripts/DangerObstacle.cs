using PlayerControllers;
using UnityEngine;

public class DangerObstacle : MonoBehaviour
{
    [SerializeField] private float _damage = 10000;

    [SerializeField] private ParticleSystem _damageParticle;

    public float GetDamage()
    {
        return _damage;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (_damageParticle != null && other.GetComponent<PlayerController>())
        {
            _damageParticle.transform.position = other.transform.position;
            _damageParticle.Play();
        }
    }
}
