using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using Base;

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

        [Header("Ground")]
        [SerializeField] private float _groundRayDistance = 0.25f;

        public Action<bool> IsGround { get; set; }
        bool isGround = false;

        public void Start()
        {
            for (int i = 0; i < _modulsRoot.childCount; i++)
            {
                if (_modulsRoot.GetChild(i).TryGetComponent(out AbstractModul modul))
                {
                    if (modul is LookModul look)
                        _lookModule = look;
                    else
                        if (modul is MovmentModul movment)
                            _movementModul = movment;
                    else
                        if (modul is PlayerSoundModul sound)
                            _playerSoundModul = sound;
                    else
                        if (modul is HookingModul hook)
                            _hookingModul = hook;
                    else
                        if (modul is IGS_Modul igs)
                            _igsModul = igs;
                }
            }

            _playerData = _characterData.GetPlayerData(); 

            _lookModule?.Init(_playerData, this);
            _movementModul?.Init(_playerData, this);
            _playerSoundModul?.Init(_playerData, this);
            _hookingModul?.Init(_playerData, this);
            _igsModul?.Init(_playerData, this);

            isGround = false;
        }

        private void FixedUpdate()
        {
            GroundControll();
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