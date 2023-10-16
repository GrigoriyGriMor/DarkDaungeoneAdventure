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

                return;
            }

            //_playerData.PlayerBase.localEulerAngles = new Vector3(_playerData.PlayerBase.rotation.x, _playerData.CameraControlBlock.rotation.y, _playerData.PlayerBase.rotation.z);
            //_playerData.CameraControlBlock.localEulerAngles = new Vector3(_playerData.CameraControlBlock.localEulerAngles.x, 0, _playerData.CameraControlBlock.localEulerAngles.z);

            Vector3 vec = new Vector3(horizMove * _moveSpeed, 0, verticalMove * _moveSpeed);
            _playerData.PlayerRB.velocity = transform.TransformVector(vec);

            float angle = Mathf.Atan2(horizMove, verticalMove) * Mathf.Rad2Deg;
            _playerData.PlayerVisual.transform.localRotation = Quaternion.Euler(0, angle, 0);

            if (!_playerData.PlayerAnimator.GetBool("Run")) 
                _playerData.PlayerAnimator.SetBool("Run", true);
        }
    }
}