using System.Collections;
using UnityEngine;

namespace PlayerControllers
{
    public class LookModul : AbstractModul
    {
        [SerializeField] private JoystickStickk _lookJoystick;

        [Header("min and max")]
        [SerializeField] private float downAngle = 35f; 
        [SerializeField] private float upAngle = -35f;

        [Header("Rotate Speed")]
        [SerializeField] private float horizontalSpeed = 100f;
        [SerializeField] private float verticalSpeed = 100f;

        private float yRotate = 0f;
        private float yRotateCamera = 0;

        public IEnumerator Start()
        {
            while (!_inputSystemMN)
                yield return new WaitForFixedUpdate();
        }

        void Update()
        {
            if (!moduleIsActive || _inputSystemMN == null || _playerDead)
                return;

            yRotate = yRotateCamera;

            float xAxis = _inputSystemMN.CameraMove().x * horizontalSpeed * Time.deltaTime;
            float yAxis = _inputSystemMN.CameraMove().y * verticalSpeed * Time.deltaTime;
            yRotate -= yAxis;
            yRotate = Mathf.Clamp(yRotate, downAngle, upAngle);

            _playerData.CameraControlBlock.transform.localRotation = Quaternion.Euler(yRotate, _playerData.CameraControlBlock.transform.localEulerAngles.y + xAxis, _playerData.CameraControlBlock.transform.localEulerAngles.z);
            yRotateCamera = yRotate;
        }

        public override void SetModuleActivityType(bool _modulIsActive)
        {
            base.SetModuleActivityType(_modulIsActive);
        }
    }
}