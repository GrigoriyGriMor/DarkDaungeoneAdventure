using UnityEngine;
using System.Collections.Generic;
using System.Linq;
using Base;
using AttackSystem;
using System.Collections;

namespace PlayerControllers
{
    public class AttackModule : AbstractModul
    {
        [Header("Detection Settings")]
        [SerializeField] private float _detectionRadius = 5f;
        [SerializeField] private float _detectionAngle = 90f;
        [SerializeField] private LayerMask _enemyLayer;

        [Header("Attack Settings")]
        [SerializeField] private float _attackRange = 2f;
        [SerializeField] private float _jumpAttackRange = 4f;
        [SerializeField] private float _jumpAttackSpeed = 10f;
        [SerializeField] private float _rotationSpeed = 10f;
        [SerializeField] private float _attackDamageDelay = 0.3f;

        [Header("Damage Settings")]
        [SerializeField] private float _baseDamage = 20f;
        [SerializeField] private float _comboMultiplier = 1.5f;

        private List<IAttackable> _potentialTargets = new();
        private IAttackable _currentTarget;
        private bool _isAttacking;
        private int _comboCount;
        private float _lastAttackTime;
        private const float COMBO_RESET_TIME = 2f;

        protected override void SubscribeToInput()
        {
            if (_inputSystemMN == null) return;

            _inputSystemMN._attackAction._clickAction += () => StartAttackSequence();
        }

        private void FixedUpdate()
        {
            if (_inputSystemMN.Move() != Vector2.zero)
                CancelAttack();
        }

        private void StartAttackSequence()
        {
            if (_isAttacking) return;

            DetectTargets();
            if (_potentialTargets.Count == 0) return;

            _currentTarget = GetPriorityTarget();
            Debug.LogError(_currentTarget.GetTransform().name);
            if (_currentTarget == null) return;

            float distanceToTarget = Vector3.Distance(_playerData.PlayerRB.transform.position, _currentTarget.GetTransform().position);

            if (distanceToTarget <= _attackRange)
            {
                StartCloseRangeAttack();
            }
            else if (distanceToTarget <= _jumpAttackRange)
            {
                StartJumpAttack();
            }
            else
            {
                MoveTowardsTarget();
            }
        }

        private void DetectTargets()
        {
            _potentialTargets.Clear();
            Collider[] colliders = Physics.OverlapSphere(_playerData.PlayerRB.transform.position, _detectionRadius, _enemyLayer);

            foreach (var collider in colliders)
            {
                var enemyBase = collider.GetComponent<AttackSystem.EnemyBase>();
                if (enemyBase != null && enemyBase.IsAlive)
                {
                    _potentialTargets.Add((IAttackable)enemyBase);
                }
            }
        }

        private IAttackable GetPriorityTarget()
        {
            if (_potentialTargets.Count == 0) return null;

            Vector3 playerForward = _playerData.PlayerRB.transform.forward;
            var priorityTargets = _potentialTargets
                .Where(target => 
                {
                    Vector3 directionToTarget = (target.GetTransform().position - _playerData.PlayerRB.transform.position).normalized;
                    float angle = Vector3.Angle(playerForward, directionToTarget);
                    return angle <= _detectionAngle * 0.5f;
                })
                .OrderBy(target => Vector3.Distance(_playerData.PlayerRB.transform.position, target.GetTransform().position))
                .ToList();

            return priorityTargets.Count > 0 ? priorityTargets[0] : _potentialTargets.OrderBy(t => 
                Vector3.Distance(_playerData.PlayerRB.transform.position, t.GetTransform().position)).First();
        }

        private void MoveTowardsTarget()
        {
            if (_currentTarget == null) return;
            
            _isAttacking = true;
            StartCoroutine(MoveToTargetCoroutine());
        }

        private System.Collections.IEnumerator MoveToTargetCoroutine()
        {
            while (_currentTarget != null && _currentTarget.IsAlive)
            {
                Vector3 directionToTarget = (_currentTarget.GetTransform().position - _playerData.PlayerRB.transform.position).normalized;
                float distanceToTarget = Vector3.Distance(_playerData.PlayerRB.transform.position, _currentTarget.GetTransform().position);

                // Rotate towards target
                Quaternion targetRotation = Quaternion.LookRotation(directionToTarget);
                _playerData.PlayerRB.transform.rotation = Quaternion.Lerp(
                    _playerData.PlayerRB.transform.rotation,
                    targetRotation,
                    _rotationSpeed * Time.deltaTime
                );

                if (distanceToTarget <= _attackRange)
                {
                    StartCloseRangeAttack();
                    yield break;
                }
                else if (distanceToTarget <= _jumpAttackRange)
                {
                    StartJumpAttack();
                    yield break;
                }

                yield return null;
            }

            _isAttacking = false;
        }

        private void StartCloseRangeAttack()
        {
            if (Time.time - _lastAttackTime > COMBO_RESET_TIME)
            {
                _comboCount = 0;
            }

            _comboCount = (_comboCount + 1) % 4;
            float damage = _baseDamage * (_comboCount == 3 ? _comboMultiplier : 1f);

            Vector3 directionToTarget = (_currentTarget.GetTransform().position - _playerData.PlayerRB.transform.position).normalized;
            Quaternion targetRotation = Quaternion.LookRotation(directionToTarget);
            _playerData.PlayerVisual.transform.rotation = Quaternion.Lerp(
                _playerData.PlayerVisual.transform.rotation,
                targetRotation,
                1f
            );

            _playerData.PlayerAnimator.SetTrigger($"Attack{_comboCount}");
            
            StartCoroutine(DealDamageWithDelay(damage));

            _lastAttackTime = Time.time;
            
            if (!_currentTarget.IsAlive)
            {
                FindNextTarget();
            }
        }

        private IEnumerator DealDamageWithDelay(float damage)
        {
            yield return new WaitForSeconds(_attackDamageDelay);
            if (_currentTarget != null && _currentTarget.IsAlive)
            {
                _currentTarget.TakeDamage(damage);
            }
        }

        private void StartJumpAttack()
        {
            _isAttacking = true;
            StartCoroutine(JumpAttackCoroutine());
        }

        private System.Collections.IEnumerator JumpAttackCoroutine()
        {
            if (_currentTarget == null)
            {
                _isAttacking = false;
                yield break;
            }

            Vector3 startPos = _playerData.PlayerRB.transform.position;
            Vector3 targetPos = _currentTarget.GetTransform().position;
            float journeyLength = Vector3.Distance(startPos, targetPos);
            float startTime = Time.time;

            _playerData.PlayerAnimator.SetTrigger("JumpAttack");

            while (Vector3.Distance(_playerData.PlayerRB.transform.position, targetPos) > 0.1f)
            {
                float distanceCovered = (Time.time - startTime) * _jumpAttackSpeed;
                float fractionOfJourney = distanceCovered / journeyLength;

                _playerData.PlayerRB.transform.position = Vector3.Lerp(startPos, targetPos, fractionOfJourney);
                
                // Rotate towards target during jump
                Vector3 direction = (targetPos - _playerData.PlayerRB.transform.position).normalized;
                if (direction != Vector3.zero)
                {
                    Quaternion targetRotation = Quaternion.LookRotation(direction);
                    _playerData.PlayerRB.transform.rotation = Quaternion.Lerp(
                        _playerData.PlayerRB.transform.rotation,
                        targetRotation,
                        _rotationSpeed * Time.deltaTime
                    );
                }

                yield return null;
            }

            _currentTarget.TakeDamage(_baseDamage);
            
            if (!_currentTarget.IsAlive)
            {
                FindNextTarget();
            }

            _isAttacking = false;
        }

        private void FindNextTarget()
        {
            DetectTargets();
            _currentTarget = GetPriorityTarget();
            if (_currentTarget != null)
            {
                float distance = Vector3.Distance(_playerData.PlayerRB.transform.position, _currentTarget.GetTransform().position);
                if (distance <= _attackRange)
                {
                    StartCloseRangeAttack();
                }
                else if (distance <= _jumpAttackRange)
                {
                    StartJumpAttack();
                }
            }
        }

        private void CancelAttack()
        {
            if (_isAttacking)
            {
                StopAllCoroutines();
                _isAttacking = false;
                _currentTarget = null;
                _comboCount = 0;
            }
        }

        private void OnDrawGizmosSelected()
        {
            if (!Application.isPlaying) return;
            
            // Draw detection radius
            Gizmos.color = Color.yellow;
            Gizmos.DrawWireSphere(transform.position, _detectionRadius);

            // Draw detection angle
            Vector3 forward = transform.forward;
            Vector3 right = Quaternion.Euler(0, _detectionAngle * 0.5f, 0) * forward;
            Vector3 left = Quaternion.Euler(0, -_detectionAngle * 0.5f, 0) * forward;

            Gizmos.color = Color.red;
            Gizmos.DrawRay(transform.position, right * _detectionRadius);
            Gizmos.DrawRay(transform.position, left * _detectionRadius);

            // Draw attack ranges
            Gizmos.color = Color.blue;
            Gizmos.DrawWireSphere(transform.position, _attackRange);
            
            Gizmos.color = Color.green;
            Gizmos.DrawWireSphere(transform.position, _jumpAttackRange);
        }
    }
} 