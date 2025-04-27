using PlayerControllers;
using System.Collections;
using UnityEngine;

public class RespawnPoint : MonoBehaviour
{
    [SerializeField] private Transform _respawnTransform;
    public Transform RespawnTransform { get => _respawnTransform; }

    [SerializeField] private GameObject _basicEffect;

    [SerializeField] private ParticleSystem _activateEffect;
    [SerializeField] private ParticleSystem _respawnEffect;

    [SerializeField] private Animator _anim;

    private void Start()
    {
        if (_respawnTransform == null)
            _respawnTransform = transform;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.TryGetComponent(out PlayerController controller))
        {
            if (controller.GetRespawnPoint() != this)
            {
                controller.SetRespawnPoint(this);
                if (_activateEffect != null)
                    _activateEffect.Play();

                StartCoroutine(DisableEffect());
            }
        }
    }

    public void Respawn()
    {
        if (_respawnEffect != null)
        {
            _respawnEffect.gameObject.SetActive(true);
            _respawnEffect.Play();

            StartCoroutine(DisableEffect());
        }
    }

    public void EnableEffect()
    {
        _basicEffect.SetActive(true);
    }

    private IEnumerator DisableEffect()
    {
        _anim.SetTrigger("Start");
        yield return new WaitForSeconds(1.5f);

        if (_basicEffect)
            _basicEffect.SetActive(false);
    }

    private void OnDisable()
    {
        StopAllCoroutines();
    }
}
