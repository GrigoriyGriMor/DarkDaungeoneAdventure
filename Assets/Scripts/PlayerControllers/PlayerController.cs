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

        [Header("Ground")]
        [SerializeField] private float _groundRayDistance = 0.25f;

        [Header("When Obj Destroyed")]
        [SerializeField] private DestroyObjModule _destroyModule;

        public Action<bool> IsGround { get; set; }
        bool isGround = false;

        public void Start()
        {
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
                        default:
                            Debug.LogError($"{modul.gameObject.name} - module is empty");
                            break;
                    }
                }
            }

            _playerData = _characterData.GetPlayerData(); 

            _lookModule?.Init(_playerData, this);
            _movementModul?.Init(_playerData, this);
            _playerSoundModul?.Init(_playerData, this);
            _hookingModul?.Init(_playerData, this);
            _igsModul?.Init(_playerData, this);
            _healModule?.Init(_playerData, this);

            isGround = false;
        }

        private void FixedUpdate()
        {
            GroundControll();
        }

        private void PlayerDie()
        {
            _lookModule?.SetModuleActivityType(false);
            _movementModul?.SetModuleActivityType(false);
            _playerSoundModul?.SetModuleActivityType(false);
            _hookingModul?.SetModuleActivityType(false);
            _igsModul?.SetModuleActivityType(false);
            _healModule?.SetModuleActivityType(false);

            _playerData.PlayerAnimator.SetTrigger("Die");
            _destroyModule.SwapObj();
        }

        public void IsHooking(bool hooking)
        { 
            _movementModul?.IsHooking(hooking);
        }

        void GroundControll()
        {
            if (Physics.Raycast(transform.position, transform.up * -1, _groundRayDistance, (1 << LayerMask.NameToLayer("Ground"))))
            {
                if (!isGround)
                {
                    isGround = true;
                    IsGround?.Invoke(isGround);
                }
            }
            else
            {
                if (isGround)
                {
                    isGround = false;
                    IsGround?.Invoke(isGround);
                }
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