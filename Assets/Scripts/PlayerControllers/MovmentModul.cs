using Game.Core;
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

        [Header("")]
        [SerializeField] private float _blendMovementSpeed = 0.25f;
        [SerializeField] private float _blendRotateSpeed = 0.25f;

        float _currentAngle = 0;
        float _currentSpeed = 0;

        InputSystemManager _inputSystemMN;

        public IEnumerator Start()
        {
            while (!GameManager.Instance)
                yield return new WaitForFixedUpdate();

            _inputSystemMN = GameManager.Instance.GetManager<InputSystemManager>();

            _inputSystemMN._jumpAction += Jump;
            _mSpeed = _moveSpeed;
        }

        private void FixedUpdate()
        {
            if (!moduleIsActive) return;

            Move();
        }

        private void Move()
        {
            float horizMove = _inputSystemMN.Move().x;
            float verticalMove = _inputSystemMN.Move().y;

            if (horizMove == 0.0f && verticalMove == 0.0f)
            {
                if (_playerData.PlayerAnimator.GetBool("Run"))
                    _playerData.PlayerAnimator.SetBool("Run", false);

                _playerData.PlayerAnimator.SetTrigger("Break");

                _currentSpeed = Mathf.Lerp(_currentSpeed, 0, _blendMovementSpeed);
                _playerData.PlayerAnimator.SetFloat("Move", _currentSpeed);

                _playerData.PlayerMainCamera.StopMove();

                if (_playerData.PlayerAnimator.GetFloat("Move") < 0.01f)
                    _playerData.PlayerRB.linearVelocity = new Vector3(0, _playerData.PlayerRB.linearVelocity.y, 0);

                return;
            }

            _playerData.PlayerBase.localEulerAngles =
                new Vector3(_playerData.PlayerBase.localEulerAngles.x, _playerData.CameraControlBlock.eulerAngles.y, _playerData.PlayerBase.localEulerAngles.z);
            _playerData.CameraControlBlock.localEulerAngles =
                new Vector3(_playerData.CameraControlBlock.localEulerAngles.x, 0, _playerData.CameraControlBlock.localEulerAngles.z);

            float angle = Mathf.Atan2(horizMove, verticalMove) * Mathf.Rad2Deg;

            _currentAngle = Mathf.Lerp(_currentAngle, angle, _blendRotateSpeed);
            _playerData.PlayerVisual.transform.localRotation = Quaternion.Euler(0, _currentAngle, 0);

            _currentSpeed = Mathf.Lerp(_currentSpeed, _mSpeed, _blendMovementSpeed);
            Vector3 vec = new Vector3(horizMove * _currentSpeed, _playerData.PlayerRB.linearVelocity.y, verticalMove * _currentSpeed);
            _playerData.PlayerRB.linearVelocity = transform.TransformVector(vec);

            _playerData.PlayerMainCamera.StartMove();

            if (!_playerData.PlayerAnimator.GetBool("Run"))
                _playerData.PlayerAnimator.SetBool("Run", true);

            _playerData.PlayerAnimator.SetFloat("Move", _currentSpeed);
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

        private void OnDisable()
        {
            _inputSystemMN._jumpAction -= Jump;
        }
    }
}