using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace PlayerControllers
{
    public class MovmentModul : AbstractModul
    {
        [Header("")]
        [SerializeField] private JoystickStickk _movementJoystick;
        [SerializeField] private float _moveSpeed = 5;
        private float _mSpeed = 0;

        [Header("")]
        [SerializeField] private Button _jumpBtn;
        [SerializeField] private float _jumpForce = 250f;

        private void Start()
        {
            _jumpBtn.onClick.AddListener(Jump);
            _mSpeed = _moveSpeed;
        }

        private void FixedUpdate()
        {
            if (!moduleIsActive) return;

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

                _playerData.PlayerRB.linearVelocity = new Vector3(0, _playerData.PlayerRB.linearVelocity.y, 0);

                _playerData.PlayerMainCamera.StopMove();

                return;
            }

            _playerData.PlayerBase.localEulerAngles =
                new Vector3(_playerData.PlayerBase.localEulerAngles.x, _playerData.CameraControlBlock.eulerAngles.y, _playerData.PlayerBase.localEulerAngles.z);
            _playerData.CameraControlBlock.localEulerAngles =
                new Vector3(_playerData.CameraControlBlock.localEulerAngles.x, 0, _playerData.CameraControlBlock.localEulerAngles.z);

            float angle = Mathf.Atan2(horizMove, verticalMove) * Mathf.Rad2Deg;
            _playerData.PlayerVisual.transform.localRotation = Quaternion.Euler(0, angle, 0);

            Vector3 vec = new Vector3(horizMove * _mSpeed, _playerData.PlayerRB.linearVelocity.y, verticalMove * _mSpeed);
            _playerData.PlayerRB.linearVelocity = transform.TransformVector(vec);

            _playerData.PlayerMainCamera.StartMove();

            if (!_playerData.PlayerAnimator.GetBool("Run"))
                _playerData.PlayerAnimator.SetBool("Run", true);
        }

        void Jump()
        {
            if (!_isGround || !moduleIsActive) return;

            _playerData.PlayerRB.AddForce(_playerData.PlayerRB.transform.up * _jumpForce, ForceMode.Impulse);
            _playerData.PlayerAnimator.SetTrigger("Jump");
        }

        void ResetAllParam()
        {
            _playerData?.PlayerAnimator.SetBool("Run", false);
            _mSpeed = 0;
        }

        public void IsHooking(bool hooking)
        {
            if (hooking)
                _mSpeed = _moveSpeed / 2;
            else
                _mSpeed = _moveSpeed;
        }

        public override void SetModuleActivityType(bool _modulIsActive)
        {
            base.SetModuleActivityType(_modulIsActive);

            if (!_modulIsActive)
                ResetAllParam();
        }
    }
}