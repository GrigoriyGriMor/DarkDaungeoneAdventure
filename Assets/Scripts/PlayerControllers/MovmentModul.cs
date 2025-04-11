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
  
            if (!_playerController.IsFly() || _isGround) 
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

                //solo
                //_playerData.PlayerMainCamera.StopMove();

                if (_playerData.PlayerAnimator.GetFloat("Move") < 0.01f)
                    _playerData.PlayerRB.linearVelocity = new Vector3(0, _playerData.PlayerRB.linearVelocity.y, 0);

                return;
            }
            
            Vector3 currentPosition = _playerData.PlayerBase.localPosition;
            Vector3 cameraForward =  _playerData.CameraControlBlock.forward;
            Vector3 cameraRight =  _playerData.CameraControlBlock.right;
            
            cameraForward.y = 0;
            cameraRight.y = 0;
            cameraForward.Normalize();
            cameraRight.Normalize();
            
            Vector3 moveDirection = cameraForward * verticalMove + cameraRight * horizMove;
            Debug.DrawRay(currentPosition, moveDirection, Color.green,0.2f);
            
            float targetAngle = Mathf.Atan2(moveDirection.x, moveDirection.z) * Mathf.Rad2Deg;
            _currentAngle = Mathf.Lerp(_currentAngle, targetAngle, _blendRotateSpeed);
            _playerData.PlayerVisual.transform.localRotation = Quaternion.Euler(0, _currentAngle, 0);
            _currentSpeed = Mathf.Lerp(_currentSpeed, _mSpeed, _blendMovementSpeed);
            Vector3 vec = new Vector3(moveDirection.x * _currentSpeed, _playerData.PlayerRB.linearVelocity.y, moveDirection.z * _currentSpeed);
            _playerData.PlayerRB.linearVelocity = transform.TransformVector(vec);

            //solo
            // _playerData.PlayerMainCamera.StartMove();

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
    }
}