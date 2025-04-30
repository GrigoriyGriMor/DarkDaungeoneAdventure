using AttackSystem;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

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

        [Header("Move To Target")]
        [SerializeField] private float _moveSpeed = 7f;
        [SerializeField] private float _targetAngleThreshold = 5f;
        [SerializeField] private float _cameraRotateSpeed = 5f;

        [Header("Damage Settings")]
        [SerializeField] private float _baseDamage = 20f;
        [SerializeField] private float _comboMultiplier = 1.5f;

        [Header("Return Control To Player")]
        [SerializeField] private float _blockControlTime = 0.2f;
        [SerializeField] private float _minInputThreshold = 0.1f;
        private bool _controlIsBlocked = false;

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
            if (!_controlIsBlocked && _inputSystemMN.Move().magnitude > _minInputThreshold) 
                CancelAttack();
        }

        private IEnumerator BlockControlTimer()
        {
            _controlIsBlocked = true;

            float timer = 0;
            while (timer < _blockControlTime)
            {
                timer += Time.deltaTime;
                yield return null;
            }

            _controlIsBlocked = false;
        }

        private void StartAttackSequence()
        {
            if (_isAttacking) return;

            DetectTargets();
            if (_potentialTargets.Count == 0) return;

            _currentTarget = GetPriorityTarget();
            if (_currentTarget == null) return;

            StartCoroutine(BlockControlTimer());

            float distanceToTarget = Vector3.Distance(_playerData.PlayerBase.position, _currentTarget.GetTransform().position);

            if (distanceToTarget <= _attackRange)
            {
                StartMelleAttack();
            }
            else 
            if (distanceToTarget <= _jumpAttackRange)
            {
                StartDashAttack();
            }
            else
            {
                MoveTowardsTarget();
            }
        }

        #region Check Targets
        private void DetectTargets()
        {
            _potentialTargets.Clear();
            Collider[] colliders = Physics.OverlapSphere(_playerData.PlayerBase.position, _detectionRadius, _enemyLayer);

            foreach (var collider in colliders)
            {
                EnemyBase enemyBase = collider.GetComponent<EnemyBase>();

                if (enemyBase != null && enemyBase.IsAlive())
                    _potentialTargets.Add(enemyBase);
            }
        }

        private IAttackable GetPriorityTarget()
        {
            if (_potentialTargets.Count == 0) return null;

            Vector3 playerForward = _playerData.CameraControlBlock.transform.forward;
            var priorityTargets = _potentialTargets
                .Where(target => 
                {
                    Vector3 directionToTarget = (target.GetTransform().position - _playerData.PlayerBase.position).normalized;
                    float angle = Vector3.Angle(playerForward, directionToTarget);
                    return angle <= _detectionAngle * 0.5f;
                })
                .OrderBy(target => Vector3.Distance(_playerData.PlayerRB.transform.position, target.GetTransform().position))
                .ToList();

            return priorityTargets.Count > 0 ? priorityTargets[0] : _potentialTargets.OrderBy(t => 
                Vector3.Distance(_playerData.PlayerRB.transform.position, t.GetTransform().position)).First();
        }
        #endregion

        #region Attack Types
        private void MoveTowardsTarget()
        {
            if (_currentTarget == null) return;
            
            _isAttacking = true;
            _playerController.SetMovementBlocked(true);
            StartCoroutine(MoveToTargetCoroutine());
            StartCoroutine(MoveCameraToTargetCoroutine());
        }

        private IEnumerator MoveCameraToTargetCoroutine()
        {
            _playerController.SetCameraBlocked(true);

            while (_currentTarget != null && _currentTarget.IsAlive())
            {
                Vector3 directionToTarget = (_currentTarget.GetTransform().position - _playerData.PlayerBase.position).normalized;
                Vector3 currentForward = _playerData.CameraControlBlock.forward;

                float currentAngle = Vector3.Angle(currentForward, directionToTarget);

                if (currentAngle <= _targetAngleThreshold)
                {
                    _playerController.SetCameraBlocked(false);
                    yield break;
                }

                _playerData.CameraControlBlock.rotation = Quaternion.Lerp(_playerData.CameraControlBlock.rotation, Quaternion.LookRotation(directionToTarget),
                    _cameraRotateSpeed * Time.deltaTime
                );

                yield return null;
            }

            _playerController.SetCameraBlocked(false);
        }

        private IEnumerator MoveToTargetCoroutine()
        {
            Vector3 directionToTarget = (_currentTarget.GetTransform().position - _playerData.PlayerBase.position).normalized;

            if (!_playerData.PlayerAnimator.GetBool("Run"))
                _playerData.PlayerAnimator.SetBool("Run", true);
            _playerData.PlayerAnimator.SetFloat("Move", _moveSpeed);

            while ((_currentTarget != null && _currentTarget.IsAlive()) && _isAttacking)
            {
                directionToTarget = (_currentTarget.GetTransform().position - _playerData.PlayerBase.position).normalized;
                float distanceToTarget = Vector3.Distance(_playerData.PlayerBase.position, _currentTarget.GetTransform().position);

                float targetYRotation = Mathf.Atan2(directionToTarget.x, directionToTarget.z) * Mathf.Rad2Deg;
                Quaternion targetRotation = Quaternion.Euler(0, targetYRotation, 0);
                
                _playerData.PlayerVisual.rotation = Quaternion.Lerp(
                    _playerData.PlayerVisual.rotation,
                    targetRotation,
                    _rotationSpeed * Time.deltaTime
                );

                Vector3 movementVelocity = new Vector3(directionToTarget.x * _moveSpeed, _playerData.PlayerRB.linearVelocity.y, directionToTarget.z * _moveSpeed);
                _playerData.PlayerRB.linearVelocity = movementVelocity;

                if (distanceToTarget <= _attackRange)
                {
                    StartMelleAttack();
                    _playerController.SetMovementBlocked(false);
                    yield break;
                }
                //else if (distanceToTarget <= _jumpAttackRange)
                //{
                //    StartDashAttack();
                //    yield break;
                //}

                yield return null;
            }

            _playerController.SetMovementBlocked(false);
            _isAttacking = false;
        }

        private void StartMelleAttack()
        {
            return;

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
            
            if (!_currentTarget.IsAlive())
            {
                FindNextTarget();
            }
        }

        private IEnumerator DealDamageWithDelay(float damage)
        {
            yield return new WaitForSeconds(_attackDamageDelay);
            if (_currentTarget != null && _currentTarget.IsAlive())
            {
                _currentTarget.TakeDamage(damage);
            }
        }

        private void StartDashAttack()
        {
            return;

            _isAttacking = true;
            StartCoroutine(JumpAttackCoroutine());
        }

        private IEnumerator JumpAttackCoroutine()
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
            
            if (!_currentTarget.IsAlive())
            {
                FindNextTarget();
            }

            _isAttacking = false;
        }
        #endregion

        private void FindNextTarget()
        {
            DetectTargets();
            _currentTarget = GetPriorityTarget();
            if (_currentTarget != null)
            {
                float distance = Vector3.Distance(_playerData.PlayerRB.transform.position, _currentTarget.GetTransform().position);
                if (distance <= _attackRange)
                {
                    StartMelleAttack();
                }
                else if (distance <= _jumpAttackRange)
                {
                    StartDashAttack();
                }
            }
        }

        private void CancelAttack()
        {
            if (_isAttacking)
            {
                _isAttacking = false;
                _currentTarget = null;
                _comboCount = 0;
            }
        }

#if UNITY_EDITOR
        private void OnDrawGizmos()
        {
            if (_playerData == null || _playerData.CameraControlBlock == null) return;

            // Draw detection radius
            Gizmos.color = Color.yellow;
            Gizmos.DrawWireSphere(_playerData.PlayerBase.position, _detectionRadius);

            // Draw detection angle
            Vector3 forward = _playerData.CameraControlBlock.transform.forward;
            Vector3 right = _playerData.CameraControlBlock.transform.right;
            Vector3 up = _playerData.CameraControlBlock.transform.up;

            float halfAngle = _detectionAngle * 0.5f;
            int segments = 20;
            float angleStep = halfAngle / segments;

            // Draw left side of the angle
            Gizmos.color = Color.green;
            for (int i = 0; i < segments; i++)
            {
                float angle = -halfAngle + (angleStep * i);
                Vector3 direction = Quaternion.AngleAxis(angle, up) * forward;
                Vector3 endPoint1 = _playerData.PlayerBase.position + direction * _detectionRadius;
                Gizmos.DrawLine(_playerData.PlayerBase.position, endPoint1);
            }

            // Draw right side of the angle
            for (int i = 0; i < segments; i++)
            {
                float angle = angleStep * i;
                Vector3 direction = Quaternion.AngleAxis(angle, up) * forward;
                Vector3 endPoint2 = _playerData.PlayerBase.position + direction * _detectionRadius;
                Gizmos.DrawLine(_playerData.PlayerBase.position, endPoint2);
            }

            // Draw arc connecting the sides
            Gizmos.color = Color.cyan;
            Vector3 startPoint = _playerData.PlayerBase.position + Quaternion.AngleAxis(-halfAngle, up) * forward * _detectionRadius;
            Vector3 endPoint3 = _playerData.PlayerBase.position + Quaternion.AngleAxis(halfAngle, up) * forward * _detectionRadius;
            Gizmos.DrawLine(startPoint, endPoint3);
        }
#endif
    }
} 