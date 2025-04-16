using PlayerControllers;
using UnityEngine;

public class RespawnPoint : MonoBehaviour
{
    public Transform respawnTransform;
    public ParticleSystem effect;
    private void Start()
    {
        respawnTransform = transform;    
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.TryGetComponent<PlayerController>(out PlayerController controller))
        {
            controller.SetRespawnPoint(this);
        }
    }
}
