using System;
using System.Collections;
using UnityEngine;
using Random = UnityEngine.Random;
using AttackSystem;

public class Warrior1 : EnemyBase
{
    public static event Action<Transform> deadEvent;
    
    [Header("Components")]
    [SerializeField] private ZoneDamage1 zoneDamage1;
    [SerializeField] private Animator animator;
    [SerializeField] private HealthWarrior healthWarrior;
    [SerializeField] private SoundWarrior soundWarrior;
    
    [Header("Animation States")]
    [SerializeField] private string stateRun = "Run";
    [SerializeField] private string stateAttack = "Attack";
    [SerializeField] private string stateDamage = "Damage";
    [SerializeField] private string stateDead = "Die";
    
    [Header("Detection Settings")]
    [SerializeField] private float radiusSkill = 10.0f;    
    [SerializeField] private float radiusPatrol = 20.0f; 
    [SerializeField] private float radiusFollow = 25f;
    [SerializeField] private float radiusDetection = 22f;
    [SerializeField] private int angleDetection = 45;
    
    [Header("Timing Settings")]
    [SerializeField] private float delayCast = 1.0f;
    [SerializeField] private float delayAttack = 2.0f;
    [SerializeField] private float delayDead = 4.0f;
    
    [Header("Movement Settings")]
    [SerializeField] private float prediction = 0.0f;
    [SerializeField] private float speedRotation = 10.0f;
    [SerializeField] private float speed = 10f;
    
    private HealModule _healModulePlayer;
    private Transform _targetPlayer;
    private StateWarrior _stateWarrior;
    private Transform _thisTransform;
    private Vector3 _startPosition;
    private Vector3 _direction;
    private bool _isPlay = false;
    private float _distance;
    private float _detectionRange;
    private float _followRange;
    private float _attackRange;
    private float _dot;
    private float _angleRadians;
    private float _angleDeg;
    private bool _diedPlayer = false;
    
    private float _currentHeal;
    private float _maxHeal;

    private IEnumerator Start() {
        _currentHeal = _maxHeal;
        _isPlay = true;
        while (_healModulePlayer == null) {
            _healModulePlayer = FindAnyObjectByType(typeof(HealModule)) as HealModule;
            yield return new WaitForFixedUpdate();
        }
        _targetPlayer = _healModulePlayer.transform;
        _healModulePlayer._die.AddListener(OnDiedPlayer);
        _detectionRange = radiusDetection * radiusDetection;
        _followRange = radiusFollow * radiusFollow;
        _attackRange = radiusSkill * radiusSkill;
        zoneDamage1.Initialize();
        _thisTransform = transform;
        _startPosition = _thisTransform.position;
        Subscription();
        soundWarrior.Initialize();
        StartCoroutine(Patrol());
    }

    private void OnDiedPlayer() => _diedPlayer = true;

    private IEnumerator Patrol() {
        soundWarrior.PlayPatrol(_thisTransform.position);
        Vector3 point = Vector3.zero;
        _stateWarrior = StateWarrior.Patrol;
        float minDistanceToPoint = 0.1f;
        float distance;
        
        while (true) {
            if (CheckPlayer()) {
                yield return StartCoroutine(Follow());
                continue;
            }
            
            point = GetPointFollow();
            distance = (_thisTransform.position - point).sqrMagnitude;
            animator.SetBool(stateRun, true);
            while (distance > minDistanceToPoint) {
                Debug.DrawLine(_thisTransform.position, point, Color.green);
                _thisTransform.LookAt(point);
                _thisTransform.position =
                    Vector3.MoveTowards(_thisTransform.position, point, speed * Time.deltaTime);
                distance = (_thisTransform.position - point).sqrMagnitude;
                
                if (CheckPlayer()) {
                    yield return StartCoroutine(Follow());
                    continue;
                }
                
                yield return null;
            }
            yield return new WaitForSeconds(0.1f);
        }
    }

    private Vector3 GetPointFollow() {
        Vector3 direction = Vector3.zero;
        Vector3 center = _startPosition;
        Vector2 randomDirection = Random.insideUnitCircle;
        direction.x = randomDirection.x;
        direction.z = randomDirection.y;
        float radius = radiusPatrol;
        Vector3 pointFollow = center + (radius * direction);
        return pointFollow;
    }

    private IEnumerator Follow() {
        soundWarrior.PlayDetect(_thisTransform.position);
        _stateWarrior = StateWarrior.Follow;
        float distanceToPlyer = (_thisTransform.position - _targetPlayer.position).sqrMagnitude;
        animator.SetBool(stateRun, true);
        while (distanceToPlyer > _attackRange) {
            Debug.DrawLine(_thisTransform.position, _targetPlayer.position, Color.red);
            _thisTransform.LookAt(_targetPlayer.position);
            _thisTransform.position = Vector3.MoveTowards(_thisTransform.position, _targetPlayer.position,
                speed * Time.deltaTime);
            distanceToPlyer = (_thisTransform.position - _targetPlayer.position).sqrMagnitude;
            float distanceToStartPosition = (_thisTransform.position - _startPosition).sqrMagnitude;
            if (_followRange < distanceToStartPosition) {
                yield break;
            }
            yield return null;
        }
        yield return StartCoroutine(Attack());
    }
    
    private IEnumerator Attack(){
        _stateWarrior = StateWarrior.Attack;
        float difference;
        float minDifference = 1.0f;
        Vector3 direction;
        Quaternion rotation;
        Vector3 positionPlayer = _targetPlayer.position + _targetPlayer.forward * prediction;
       
        animator.SetBool(stateRun, false);
        do {
            direction = _targetPlayer.position - _thisTransform.position;
            rotation = Quaternion.LookRotation(direction);
            _thisTransform.rotation =
                Quaternion.Lerp(_thisTransform.rotation, rotation, speedRotation * Time.deltaTime);
            difference = Mathf.Abs(_thisTransform.rotation.eulerAngles.y - rotation.eulerAngles.y);
            yield return null;
        } while (difference > minDifference);

        soundWarrior.PlayStartAttack(_thisTransform.position);
        zoneDamage1.SetPosition(positionPlayer);
        zoneDamage1.Show();
        yield return new WaitForSeconds(delayAttack);

        animator.SetTrigger(stateAttack);
        soundWarrior.PlayAttack(_thisTransform.position);
        yield return new WaitForSeconds(1.0f);
        zoneDamage1.Damage();
        yield return new WaitForSeconds(delayCast);
        zoneDamage1.Hide();

        yield return StartCoroutine(Follow());
    }
  
    private bool CheckPlayer() {
        if (_diedPlayer == true) return false;
        _direction = _targetPlayer.position - _thisTransform.position;
        _distance = _direction.sqrMagnitude; 
        if (_distance > _detectionRange) return false;
        _dot = Vector3.Dot(_thisTransform.forward, _direction.normalized);
        _angleRadians = Mathf.Acos(_dot);
        _angleDeg = _angleRadians * Mathf.Rad2Deg;
        return _angleDeg < angleDetection;
    }

    public override void TakeDamage(float damage)
    {
        if (!_isAlive) return;

        base.TakeDamage(damage);
        animator.SetTrigger(stateDamage);
        soundWarrior.PlayDamage(_thisTransform.position);
    }

    protected override void Die()
    {
        StopAllCoroutines();
        StartCoroutine(DelayDead());
    }

    private IEnumerator DelayDead() {
        deadEvent?.Invoke(transform);
        _stateWarrior = StateWarrior.Dead;
        animator.SetBool(stateRun, false);
        animator.SetTrigger(stateDead);
        zoneDamage1.Hide();
        yield return new WaitForSeconds(delayDead);
        gameObject.SetActive(false);
    } 

    private void Subscription() {
        healthWarrior.DamageEvent += TakeDamage;
        healthWarrior.DeathEvent += Die;
    }
    
    private void Unsubscription() {
        healthWarrior.DamageEvent -= TakeDamage;
        healthWarrior.DeathEvent -= Die;
    }

    private void OnDestroy() => Unsubscription();

#if UNITY_EDITOR
    private void OnDrawGizmos() {
        Vector3 center = transform.position;
        Vector3 startPosition = center;
        if (_isPlay) startPosition = _startPosition; 
        Vector3 offset = new Vector3(0, 0.1f, 0); 
        
        float radius = radiusPatrol;
        
        DrawWireDisk(startPosition + offset, radiusFollow, _followZoneColor);
        DrawWireDisk(startPosition + offset, radius, _patrolZoneColor);
        DrawWireDisk(center, radiusSkill, _attackZoneColor);
        DrawSector(center, radiusDetection, angleDetection, _detectionZoneColor);
    }

    private void DrawWireDisk(Vector3 center, float radius, Color color) {
        float gizmoDiskThickness = 0.01f;
        Color oldColor = Gizmos.color;
        Gizmos.color = color;
        Matrix4x4 oldMatrix = Gizmos.matrix;
        Gizmos.matrix = Matrix4x4.TRS(center, Quaternion.identity, new Vector3(1, gizmoDiskThickness, 1));
        Gizmos.DrawWireSphere(Vector3.zero, radius);
        Gizmos.matrix = oldMatrix;
        Gizmos.color = oldColor;
    }
    
    private void DrawSector(Vector3 center, float radius, float angle, Color color) {
        UnityEditor.Handles.color = color;
        UnityEditor.Handles.DrawSolidArc(transform.position, transform.up, transform.forward, angle , radius);
    }
#endif
}

enum StateWarrior {
    Idle,
    Attack,
    Run,
    Wait,
    Patrol,
    Follow,
    Dead
}