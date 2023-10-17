using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace PlayerControllers
{
    public class MovmentModul : AbstractModul
    {
        [Header("")]
        [SerializeField] private float _moveSpeed = 5;

        [SerializeField] private JoystickStickk _movementJoystick;
        [SerializeField] private Button _jumpBtn;

        private void FixedUpdate()
        {
            Move();
        }

        private void Move()
        {
            float horizMove = _movementJoystick.HorizontalAxis();
            float verticalMove = _movementJoystick.VerticalAxis();

            if (horizMove == 0.0f && verticalMove == 0.0f)
            {
                if (_playerData.PlayerAnimator.GetBool("Run")) 
                    _playerData.PlayerAnimator.SetBool("Run", false);

                _playerData.PlayerRB.velocity = new Vector3(0, 0, 0);

                _playerData.PlayerMainCamera.StopMove();

                return;
            }

            _playerData.PlayerBase.localEulerAngles = 
                new Vector3(_playerData.PlayerBase.localEulerAngles.x, _playerData.CameraControlBlock.eulerAngles.y, _playerData.PlayerBase.localEulerAngles.z);
            _playerData.CameraControlBlock.localEulerAngles = 
                new Vector3(_playerData.CameraControlBlock.localEulerAngles.x, 0, _playerData.CameraControlBlock.localEulerAngles.z);
            
            float angle = Mathf.Atan2(horizMove, verticalMove) * Mathf.Rad2Deg;
            _playerData.PlayerVisual.transform.localRotation = Quaternion.Euler(0, angle, 0);

            Vector3 vec = new Vector3(horizMove * _moveSpeed, 0, verticalMove * _moveSpeed);
            _playerData.PlayerRB.velocity = transform.TransformVector(vec);

            _playerData.PlayerMainCamera.StartMove();

            if (!_playerData.PlayerAnimator.GetBool("Run")) 
                _playerData.PlayerAnimator.SetBool("Run", true);
        }
    }
}