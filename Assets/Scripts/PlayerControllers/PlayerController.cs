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
        [SerializeField] private LookModul _lookModule;
        [SerializeField] private MovmentModul _movementModul;
        [SerializeField] private PlayerSoundModul _playerSoundModul;
        [SerializeField] private HookingModul _hookingModul;

        [Header("Ground")]
        [SerializeField] private float _groundRayDistance = 0.25f;

        public Action<bool> IsGround { get; set; }
        bool isGround = false;

        public void Start()
        {
            _playerData = _characterData.GetPlayerData(); 

            _lookModule?.Init(_playerData, this);
            _movementModul?.Init(_playerData, this);
            _playerSoundModul?.Init(_playerData, this);
            _hookingModul?.Init(_playerData, this);

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
    }
}