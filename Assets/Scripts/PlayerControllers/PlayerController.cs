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

        public void Start()
        {
            _playerData = _characterData.GetPlayerData(); 

            _lookModule?.Init(_playerData);
            _movementModul?.Init(_playerData);
            _playerSoundModul?.Init(_playerData);
        }
    }
}