using AttackSystem;
using System;
using System.Collections;
using UnityEngine;

public class EnemyBase : MonoBehaviour, IAttackable
{
    [SerializeField] private float _maxHealPoints = 100;
    [SerializeField] private float _currentHP = 0;

    private MeshRenderer _meshRenderer;
    [SerializeField] private Material _defaultMat;
    [SerializeField] private Material _damageMat;

    [SerializeField] private ParticleSystem _damageParticle;

    [Header("Sound")]
    public AudioSource source;
    public AudioClip idleClip;
    public AudioClip patrolClip;
    public AudioClip attackClip;
    public AudioClip deathClip;
    public AudioClip friendsCallingClip;

    public event Action<Transform> DeadEvent;
    bool _isAlive = true;

    Coroutine _takeDamageCoroutine;

    public void Awake()
    {
        _currentHP = _maxHealPoints;
        _meshRenderer = GetComponent<MeshRenderer>();
        _defaultMat = _meshRenderer.sharedMaterial;
    }

    public void TakeDamage(float damage)
    {
        _currentHP -= damage;

        if (_takeDamageCoroutine != null)
            StopCoroutine(_takeDamageCoroutine);
        _takeDamageCoroutine = StartCoroutine(TakeDamage());
    }

    IEnumerator TakeDamage()
    {
        if (_damageParticle != null)
            _damageParticle.Play();

        if (_damageMat != null)
            _meshRenderer.material = _damageMat;

        yield return new WaitForSeconds(0.05f);
        _meshRenderer.material = _defaultMat;

        if (_currentHP < 0)
        {
            _currentHP = 0;
            Dead();
        }

        _takeDamageCoroutine = null;
    }

    private void Dead()
    {
        StartCoroutine(DelayDead());
    }

    private IEnumerator DelayDead()
    {
        DeadEvent?.Invoke(transform);
        _isAlive = false;
        //_stateWarrior = StateWarrior.Dead;
        //animator.SetBool(stateRun, false);
        //animator.SetTrigger(stateDead);
        //zoneDamage1.Hide();
        yield return null;
        gameObject.SetActive(false);
    }

    public Transform GetTransform()
    {
        return transform;
    }

    public bool IsAlive()
    {
        return _isAlive;
    }
}