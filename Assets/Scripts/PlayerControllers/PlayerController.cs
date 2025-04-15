using Base;
using System;
using UnityEngine;

namespace PlayerControllers
{
    public class PlayerController : MonoBehaviour
    {
        [Header("Character Data")]
        [SerializeField] private CharacterData _characterData;
        private PlayerData _playerData = new();

        [Header("Moduls")]
        [SerializeField] private Transform _modulsRoot;

        [Header("")]
        [SerializeField] private LookModul _lookModule;
        [SerializeField] private MovmentModul _movementModul;
        [SerializeField] private PlayerSoundModul _playerSoundModul;
        [SerializeField] private HookingModul _hookingModul;
        [SerializeField] private IGS_Modul _igsModul;
        [SerializeField] private HealModule _healModule;
        [SerializeField] private StaminaModule _staminaModule;
        [SerializeField] private FlyModule _flyModule;
        [SerializeField] private AttackModule _attackModule;

        [Header("Ground")]
        [SerializeField] private float _groundRayDistance = 0.25f;

        [Header("When Obj Destroyed")]
        [SerializeField] private DestroyObjModule _destroyModule;

        public Action<bool> IsGround { get; set; }
        private bool _isGround;

        private void Start()
        {
            InitializeModules();
            _playerData = _characterData.GetPlayerData();
            InitializeAllModules();
            _isGround = false;
        }

        private void InitializeModules()
        {
            foreach (Transform child in _modulsRoot)
            {
                if (!child.TryGetComponent(out AbstractModul modul)) continue;

                switch (modul)
                {
                    case LookModul m: _lookModule = m; break;
                    case MovmentModul m: _movementModul = m; break;
                    case PlayerSoundModul m: _playerSoundModul = m; break;
                    case HookingModul m: _hookingModul = m; break;
                    case IGS_Modul m: _igsModul = m; break;
                    case HealModule m:
                        _healModule = m;
                        _healModule._die.AddListener(PlayerDie);
                        break;
                    case StaminaModule m: _staminaModule = m; break;
                    case FlyModule m: _flyModule = m; break;
                    case AttackModule m: _attackModule = m; break;
                    default:
                        Debug.LogError($"{modul.gameObject.name} - module is empty");
                        break;
                }
            }
        }

        private void InitializeAllModules()
        {
            _lookModule?.Init(_playerData, this);
            _movementModul?.Init(_playerData, this);
            _playerSoundModul?.Init(_playerData, this);
            _hookingModul?.Init(_playerData, this);
            _igsModul?.Init(_playerData, this);
            _healModule?.Init(_playerData, this);
            _staminaModule?.Init(_playerData, this);
            _flyModule?.Init(_playerData, this);
            _attackModule?.Init(_playerData, this);
        }

        private void FixedUpdate() => GroundControl();

        private void PlayerDie()
        {
            InvokeAllModulesDie();
            _playerData.PlayerAnimator.SetTrigger("Die");
            _destroyModule.SwapObj();
        }

        private void InvokeAllModulesDie()
        {
            _lookModule?.OnPlayerDied();
            _movementModul?.OnPlayerDied();
            _playerSoundModul?.OnPlayerDied();
            _hookingModul?.OnPlayerDied();
            _igsModul?.OnPlayerDied();
            _healModule?.OnPlayerDied();
            _staminaModule?.OnPlayerDied();
            _flyModule?.OnPlayerDied();
            _attackModule?.OnPlayerDied();
        }

        public bool IsFly() => _flyModule.IsFly;

        public void IsHooking(bool hooking) => _movementModul?.IsHooking(hooking);

        public bool StaminaCanBeUse() => _staminaModule?.MinStaminaLevelReady ?? true;

        public bool UseStamina(float gettingStamina) => _staminaModule?.UseStamina(gettingStamina) ?? true;

        public void SetMovementBlocked(bool blocked)
        {
            _movementModul?.SetMovementBlocked(blocked);
        }

        public void SetMovementInput(Vector2 input)
        {
            _movementModul?.SetMovementInput(input);
        }

        public void SetCameraBlocked(bool blocked)
        {
            _lookModule?.SetCameraBlocked(blocked);
        }

        private void GroundControl()
        {
            bool newGroundState = Physics.Raycast(transform.position, -transform.up, _groundRayDistance, 1 << LayerMask.NameToLayer("Ground"));
            
            if (_isGround != newGroundState)
            {
                _isGround = newGroundState;
                IsGround?.Invoke(_isGround);
            }
        }

        #region Collision Methods
        public Action<Collider> OnTriggerEnterAction { get; set; }
        public Action<Collider> OnTriggerExitAction { get; set; }
        public Action<Collision> OnCollisionEnterAction { get; set; }
        public Action<Collision> OnCollisionExitAction { get; set; }

        private void OnTriggerEnter(Collider other)
        {
            OnTriggerEnterAction?.Invoke(other);

            if (other.TryGetComponent(out DangerObstacle dangerObj))
            {
                _healModule.SetDamage(dangerObj.GetDamage());
            }
        }

        private void OnTriggerExit(Collider other) => OnTriggerExitAction?.Invoke(other);

        private void OnCollisionEnter(Collision collision) => OnCollisionEnterAction?.Invoke(collision);

        private void OnCollisionExit(Collision collision) => OnCollisionExitAction?.Invoke(collision);
        #endregion
    }
}