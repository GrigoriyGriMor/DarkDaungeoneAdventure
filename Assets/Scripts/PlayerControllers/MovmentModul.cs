using System.Collections;
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

        private bool _isMovementBlocked;
        private Vector2 _alternativeInput;

        public IEnumerator Start()
        {
            while (!_inputSystemMN)
                yield return new WaitForFixedUpdate();

            _inputSystemMN._jumpAction._clickAction += Jump;
            _mSpeed = _moveSpeed;
        }

        private void FixedUpdate()
        {
            if (!moduleIsActive || _inputSystemMN == null || _playerDead) return;
  
            if (!_playerController.IsFly() && _isGround) 
                Move();
        }

        private void Move()
        {
            float horizMove = 0;
            float verticalMove = 0;

            if (!_isMovementBlocked)
            {
                horizMove = _inputSystemMN.Move().x;
                verticalMove = _inputSystemMN.Move().y;
            }
            else
                return;
            
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
            if (!_isGround || !moduleIsActive || _playerDead) return;

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
            _inputSystemMN._jumpAction._clickAction -= Jump;
        }

        public void SetMovementBlocked(bool blocked)
        {
            _isMovementBlocked = blocked;
        }

        public void SetMovementInput(Vector2 input)
        {
            if (!moduleIsActive || _inputSystemMN == null || _playerDead) return;
            _alternativeInput = input;
        }
    }
}