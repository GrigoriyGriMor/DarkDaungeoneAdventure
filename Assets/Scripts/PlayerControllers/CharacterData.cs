using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Base
{
    public class CharacterData : MonoBehaviour
    {
        private static CharacterData instance;
        public static CharacterData Instance => instance;

        [SerializeField] private Transform PlayerBase;
        [SerializeField] private Transform PlayerVisual;
        [SerializeField] private Rigidbody PlayerRB;
        [SerializeField] private Animator PlayerAnimator;
        [SerializeField] private Camera PlayerMainCamera;
        [SerializeField] private APCameraController CameraControlBlock;

        private PlayerData _playerData;

        private void Awake()
        {
            instance = this;

            _playerData = new();

            _playerData.PlayerBase = PlayerBase;
            _playerData.PlayerVisual = PlayerVisual;
            _playerData.PlayerRB = PlayerRB;
            _playerData.PlayerAnimator = PlayerAnimator;
            _playerData.PlayerMainCamera = PlayerMainCamera;
            _playerData.CameraControlBlock = CameraControlBlock;
        }

        public void UpdatePlayerData(PlayerData _newData)
        { 
            if (_newData.PlayerBase != null && _newData.PlayerBase != _playerData.PlayerBase)
                _playerData.PlayerBase = _newData.PlayerBase;

            if (_newData.PlayerVisual != null && _newData.PlayerVisual != _playerData.PlayerVisual)
                _playerData.PlayerVisual = _newData.PlayerVisual;

            if (_newData.PlayerRB != null && _newData.PlayerRB != _playerData.PlayerRB)
                _playerData.PlayerRB = _newData.PlayerRB;

            if (_newData.PlayerAnimator != null && _newData.PlayerAnimator != _playerData.PlayerAnimator)
                _playerData.PlayerAnimator = _newData.PlayerAnimator;

            if (_newData.PlayerMainCamera != null && _newData.PlayerMainCamera != _playerData.PlayerMainCamera)
                _playerData.PlayerMainCamera = _newData.PlayerMainCamera;

            if (_newData.CameraControlBlock != null && _newData.CameraControlBlock != _playerData.CameraControlBlock)
                _playerData.CameraControlBlock = _newData.CameraControlBlock;
        }

        public PlayerData GetPlayerData()
        { 
            return _playerData;
        }
    }
}