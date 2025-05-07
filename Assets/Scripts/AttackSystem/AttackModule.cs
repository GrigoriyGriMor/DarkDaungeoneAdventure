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

        [SerializeField, Range(1, 10)] private int _jumpAttackAnimCount = 3;

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

        private Coroutine _attackCoroutine = null;
        private Coroutine _attackCooldownCoroutine = null;
        private Coroutine _blockControlCoroutine = null;

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
            _blockControlCoroutine = null;
        }

        private void StartAttackSequence()
        {
            if (_isAttacking || _attackCoroutine != null || _attackCooldownCoroutine != null) return;

            DetectTargets();
            if (_potentialTargets.Count == 0) return;

            _currentTarget = GetPriorityTarget();
            if (_currentTarget == null) return;

            if (_blockControlCoroutine != null)
                StopCoroutine(_blockControlCoroutine);
            _blockControlCoroutine = StartCoroutine(BlockControlTimer());

            float distanceToTarget = Vector3.Distance(_playerData.PlayerBase.position, _currentTarget.GetTransform().position);
            if (distanceToTarget <= _attackRange)
                StartMelleAttack();
            else
                if (distanceToTarget <= _jumpAttackRange)
                    StartDashAttack();
                else
                    MoveTowardsTarget();
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
            if (_playerController.Is2DMode)
                yield break;

            _playerController.SetCameraBlocked(true);

            while (_currentTarget != null && _currentTarget.IsAlive())
            {
                Vector3 directionToTarget = (_currentTarget.GetTransform().position - _playerData.PlayerBase.position).normalized;
                Vector3 currentForward = _playerData.CameraControlBlock.transform.forward;

                float currentAngle = Vector3.Angle(currentForward, directionToTarget);

                if (currentAngle <= _targetAngleThreshold)
                {
                    _playerController.SetCameraBlocked(false);
                    yield break;
                }

                _playerData.CameraControlBlock.transform.rotation = Quaternion.Lerp(_playerData.CameraControlBlock.transform.rotation, Quaternion.LookRotation(directionToTarget),
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
                    if (!_playerData.PlayerAnimator.GetBool("Run"))
                        _playerData.PlayerAnimator.SetBool("Run", false);
                    _playerData.PlayerAnimator.SetFloat("Move", 0);
                    _playerController.SetMovementBlocked(false);

                    if (_currentTarget.IsAlive())
                        StartMelleAttack();
                    yield break;
                }
                else if (distanceToTarget <= _jumpAttackRange)
                {
                    if (!_playerData.PlayerAnimator.GetBool("Run"))
                        _playerData.PlayerAnimator.SetBool("Run", false);
                    _playerData.PlayerAnimator.SetFloat("Move", 0);
                    _playerController.SetMovementBlocked(false);

                    if (_currentTarget.IsAlive())
                        StartDashAttack();
                    yield break;
                }

                yield return null;
            }

            _playerController.SetMovementBlocked(false);
        }

        private void StartMelleAttack()
        {
            if (_currentTarget == null) return;

            _isAttacking = true;
            _attackCooldownCoroutine = StartCoroutine(AttackCooldown());
            
            if (Time.time - _lastAttackTime > COMBO_RESET_TIME || _comboCount >= 3)
                _comboCount = 0;

            _comboCount++;
            float damage = _baseDamage * (_comboCount == 3 ? _comboMultiplier : 1f);

            Vector3 directionToTarget = (_currentTarget.GetTransform().position - _playerData.PlayerRB.transform.position).normalized;
            float targetYRotation = Mathf.Atan2(directionToTarget.x, directionToTarget.z) * Mathf.Rad2Deg;
            Quaternion targetRotation = Quaternion.Euler(0, targetYRotation, 0);
            _playerData.PlayerVisual.rotation = targetRotation;

            _attackCoroutine = StartCoroutine(DealDamageWithDelay(damage));
        }

        private IEnumerator DealDamageWithDelay(float damage)
        {
            _lastAttackTime = Time.time;
            _playerData.PlayerAnimator.SetTrigger($"Attack{_comboCount}");

            // Ждем, пока анимация атаки не начнется
            yield return new WaitForSeconds(0.05f);

            AnimatorStateInfo stateInfo = _playerData.PlayerAnimator.GetCurrentAnimatorStateInfo(0);
            while (stateInfo.normalizedTime < 0.4f)
            {
                stateInfo = _playerData.PlayerAnimator.GetCurrentAnimatorStateInfo(0);
                yield return null;
            }

            if (_currentTarget != null && _currentTarget.IsAlive())
            {
                _currentTarget.TakeDamage(damage);    
            }

            _isAttacking = false;
            _attackCoroutine = null;
        }

        IEnumerator AttackCooldown()
        {
            float cooldown = 0;
            while (cooldown < _attackDamageDelay)
            {
                cooldown += Time.deltaTime;
                yield return null;
            }

            _attackCooldownCoroutine = null;
        }

        private void StartDashAttack()
        {
            if (_currentTarget == null)
            {
                _isAttacking = false;
                return;
            }

            _isAttacking = true;
            _attackCoroutine = StartCoroutine(JumpAttackCoroutine());
        }

        private IEnumerator JumpAttackCoroutine()
        {
            _attackCooldownCoroutine = StartCoroutine(AttackCooldown());

            Vector3 targetPos = _currentTarget.GetTransform().position;
            _playerData.PlayerAnimator.SetTrigger($"JumpAttack{Random.Range(1, _jumpAttackAnimCount)}");

            // Rotate towards target during jump
            Vector3 direction = (targetPos - _playerData.PlayerBase.transform.position).normalized;
            float targetYRotation = Mathf.Atan2(direction.x, direction.z) * Mathf.Rad2Deg;
            Quaternion targetRotation = Quaternion.Euler(0, targetYRotation, 0);
            _playerData.PlayerVisual.rotation = targetRotation;

            while (Vector3.Distance(_playerData.PlayerBase.transform.position, targetPos) > _attackRange)
            {
                Vector3 moveDirection = (targetPos - _playerData.PlayerBase.transform.position).normalized;
                Vector3 movementVelocity = moveDirection * _jumpAttackSpeed;
                _playerData.PlayerRB.linearVelocity = new Vector3(movementVelocity.x, _playerData.PlayerRB.linearVelocity.y, movementVelocity.z);
                yield return null;
            }

            // Останавливаем движение
            _playerData.PlayerRB.linearVelocity = Vector3.zero;

            AnimatorStateInfo stateInfo = _playerData.PlayerAnimator.GetCurrentAnimatorStateInfo(0);
            while (stateInfo.normalizedTime < 0.75f)
            {
                stateInfo = _playerData.PlayerAnimator.GetCurrentAnimatorStateInfo(0);
                yield return null;
            }

            _currentTarget.TakeDamage(_baseDamage);
            _isAttacking = false;

            _attackCoroutine = null;
        }
        #endregion

        private void CancelAttack()
        {
            if (_isAttacking)
            {
                _isAttacking = false;
                _currentTarget = null;
                _comboCount = 0;

                if (_attackCoroutine != null)
                {
                    StopCoroutine(_attackCoroutine);
                    _attackCoroutine = null;
                }    
            }
        }
    }
} 