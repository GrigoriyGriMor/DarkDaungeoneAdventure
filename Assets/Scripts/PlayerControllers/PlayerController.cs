using Base;
using Game.Core;
using System;
using System.Threading.Tasks;
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

        [Header("Ground")]
        [SerializeField] private float _groundRayDistance = 0.25f;
        [SerializeField] private float _groundCheckRadius = 0.2f;
        [SerializeField] private Vector3 _groundCheckOffset = new Vector3(0, -0.1f, 0);
        [SerializeField] private LayerMask _groundLayer;
        private Collider[] _overlapResults = new Collider[4];

        [Header("When Obj Destroyed")]
        [SerializeField] private DestroyObjModule _destroyModule;

        public Action<bool> IsGround { get; set; }
        bool isGround = false;

        private RespawnPoint _respawnPoint;
        private Vector3 _baseLevelRespawnPos = Vector3.zero;

        public void Start()
        {
            _baseLevelRespawnPos = transform.position;

            for (int i = 0; i < _modulsRoot.childCount; i++)
            {
                if (_modulsRoot.GetChild(i).TryGetComponent(out AbstractModul modul))
                {
                    switch (modul)
                    {
                        case LookModul _m:
                            _lookModule = _m;
                            break;
                        case MovmentModul _m:
                            _movementModul = _m;
                            break;
                        case PlayerSoundModul _m:
                            _playerSoundModul = _m;
                            break;
                        case HookingModul _m:
                            _hookingModul = _m;
                            break;
                        case IGS_Modul _m:
                            _igsModul = _m;
                            break;
                        case HealModule _m:
                            _healModule = _m;
                            _healModule._die.AddListener(PlayerDie);
                            break;
                        case StaminaModule _m:
                            _staminaModule = _m;
                            break;
                        case FlyModule _m:
                            _flyModule = _m;
                            break;
                        default:
                            Debug.LogError($"{modul.gameObject.name} - module is empty");
                            break;
                    }
                }
            }

            _playerData = _characterData.GetPlayerData();

            InitModules();

            isGround = false;
        }

        private void InitModules()
        {
            _lookModule?.Init(_playerData, this);
            _movementModul?.Init(_playerData, this);
            _playerSoundModul?.Init(_playerData, this);
            _hookingModul?.Init(_playerData, this);
            _igsModul?.Init(_playerData, this);
            _healModule?.Init(_playerData, this);
            _staminaModule?.Init(_playerData, this);
            _flyModule?.Init(_playerData, this);
        }

        private void FixedUpdate()
        {
            GroundControll();
        }

        private void PlayerDie()
        {
            _lookModule?.OnPlayerDied();
            _movementModul?.OnPlayerDied();
            _playerSoundModul?.OnPlayerDied();
            _hookingModul?.OnPlayerDied();
            _igsModul?.OnPlayerDied();
            _healModule?.OnPlayerDied();
            _staminaModule?.OnPlayerDied();
            _flyModule?.OnPlayerDied();

            _playerData.PlayerAnimator.SetTrigger("Die");
            _destroyModule.SwapObj();

            GameManager.Instance.GetManager<WindowsManager>().OpenWindow(SupportClasses.WindowName.RespawnMenu, SupportClasses.WindowName.InGameHUD);
        }

        public void DeactivateModules()
        {
            _lookModule?.OnPlayerDied();
            _movementModul?.OnPlayerDied();
            _playerSoundModul?.OnPlayerDied();
            _hookingModul?.OnPlayerDied();
            _igsModul?.OnPlayerDied();
            _healModule?.OnPlayerDied();
            _staminaModule?.OnPlayerDied();
            _flyModule?.OnPlayerDied();
        }

        public async void PlayerRespawn()
        {
            _playerData.PlayerRB.isKinematic = true;

            Vector3 targetPoint = _respawnPoint != null
                ? _respawnPoint.RespawnTransform.position
                : _baseLevelRespawnPos;

            while (Vector3.SqrMagnitude(transform.position - targetPoint) > 0.001f)
            {
                transform.position = targetPoint;
                _respawnPoint?.Respawn();
                await Task.Delay(100);
            }

            _playerData.PlayerRB.isKinematic = false;

            _lookModule?.OnPlayerRespawn();
            _movementModul?.OnPlayerRespawn();
            _playerSoundModul?.OnPlayerRespawn();
            _hookingModul?.OnPlayerRespawn();
            _igsModul?.OnPlayerRespawn();
            _healModule?.OnPlayerRespawn();
            _staminaModule?.OnPlayerRespawn();
            _flyModule?.OnPlayerRespawn();

            _healModule?.Respawn();
            _destroyModule.ResetGO();

            isGround = false;
        }

        public RespawnPoint GetRespawnPoint()
        { 
            return _respawnPoint;
        }

        public bool IsFly()
        {
            return _flyModule.IsFly;
        }

        public void IsHooking(bool hooking)
        { 
            _movementModul?.IsHooking(hooking);
        }

        public bool StaminaCanBeUse()
        {
            if (_staminaModule != null)
                return _staminaModule.MinStaminaLevelReady;
            else
                return true;
        }

        public bool UseStamina(float gettingStamina)
        {
            if (_staminaModule == null)
                return true;

            return _staminaModule.UseStamina(gettingStamina);
        }

        void GroundControll()
        {
            Vector3 checkPos = transform.position + _groundCheckOffset;
            int hits = Physics.OverlapSphereNonAlloc(checkPos, _groundCheckRadius, _overlapResults, _groundLayer);

            bool onGround = hits > 0;
            if (onGround != isGround)
            {
                isGround = onGround;
                IsGround?.Invoke(isGround);
            }
        }

        public void SetRespawnPoint(RespawnPoint point)
        {
            if (_respawnPoint) 
                _respawnPoint.EnableEffect();
            
            _respawnPoint = point;
        }

        #region Collision Methods
        public Action<Collider> OnTriggerEnterAction { get; set; }
        public Action<Collider> OnTriggerExitAction { get; set; }
        public Action<Collision> OnCollisionEnterAction { get; set; }
        public Action<Collision> OnCollisionExitAction { get; set; }


        private void OnTriggerEnter(Collider other)
        {
            OnTriggerEnterAction?.Invoke(other);

            if (other.TryGetComponent(out DangerObstacle _dangerObj))
            {
                _healModule.SetDamage(_dangerObj.GetDamage());
            }
        }

        private void OnTriggerExit(Collider other)
        {
            OnTriggerExitAction?.Invoke(other);
        }

        private void OnCollisionEnter(Collision collision)
        {
            OnCollisionEnterAction?.Invoke(collision);
        }

        private void OnCollisionExit(Collision collision)
        {
            OnCollisionExitAction?.Invoke(collision);
        }
        #endregion
    }
}