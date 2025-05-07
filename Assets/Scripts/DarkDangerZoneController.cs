using PlayerControllers;
using System.Collections;
using UnityEngine;

[RequireComponent(typeof(Collider))]
public class DarkDangerZoneController : MonoBehaviour
{
    [SerializeField] private float _minDamage = 0.1f;
    [SerializeField] private float _maxDamage = 5f;

    [SerializeField, Range(0.01f, 10)] private float _damageDelay = 0.1f;

    private Coroutine _damageCoroutine;
    private WaitForSeconds _waitForSeconds;
    private float _damageRange;
    private float _colliderRadius;

    private void Awake()
    {
        _waitForSeconds = new WaitForSeconds(_damageDelay);
        _damageRange = _maxDamage - _minDamage;
        _colliderRadius = GetComponent<Collider>().bounds.extents.magnitude;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!other.TryGetComponent(out PlayerController player)) return;
        
        if (_damageCoroutine == null)
            _damageCoroutine = StartCoroutine(DamageCoroutine(player));
    }

    private IEnumerator DamageCoroutine(PlayerController player)
    {
        var playerTransform = player.transform;
        var position = transform.position;
        
        while (true)
        {
            float distance = Vector3.Distance(position, playerTransform.position);
            float normalizedDistance = Mathf.Clamp01(distance / _colliderRadius);
            float damage = _minDamage + (_damageRange * (1f - normalizedDistance));
            
            player.SetDamage(damage);
            yield return _waitForSeconds;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (!other.TryGetComponent<PlayerController>(out _)) return;
        
        if (_damageCoroutine != null)
        {
            StopCoroutine(_damageCoroutine);
            _damageCoroutine = null;
        }
    }
}
