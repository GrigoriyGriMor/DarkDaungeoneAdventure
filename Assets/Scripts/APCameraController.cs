using BaseClasses;
using PlayerControllers;
using System.Collections;
using System.Threading.Tasks;
using Unity.Cinemachine;
using Unity.Mathematics;
using UnityEngine;

public class APCameraController : MonoBehaviour
{
    [SerializeField] private CinemachineCamera _usingVCamera;

    [SerializeField] private CinemachineCamera cameraIdleVCamera;
    [SerializeField] private CinemachineCamera cameraMoveVCamera;

    [SerializeField] private Transform target;
    private Transform _currentTarget = null;

    private PlayerController _playerController;
    private Transform _player;

    private IEnumerator Start()
    {
        _usingVCamera = cameraIdleVCamera;

        _currentTarget = target;

        cameraIdleVCamera.LookAt = _currentTarget;
        cameraMoveVCamera.LookAt = _currentTarget;

        RequestToSetPriorityVCamera(cameraIdleVCamera);

        while (_player == null || !LevelManager.Instance)
        {
            if (LevelManager.Instance.PlayerController != null)
            {
                _playerController = LevelManager.Instance.PlayerController;
                _player = _playerController.transform;
            }
            
            yield return null;
        }
    }

    private void Update()
    {
        if (_player != null)
            transform.position = _player.position;
    }

    public void StartMove()
    {
        if (_usingVCamera != cameraIdleVCamera || _usingVCamera != cameraMoveVCamera)
            return;

        RequestToSetPriorityVCamera(cameraMoveVCamera);
    }

    public void StopMove()
    {
        if (_usingVCamera != cameraIdleVCamera && _usingVCamera != cameraMoveVCamera)
            return;

        RequestToSetPriorityVCamera(cameraIdleVCamera);
    }

    public void SetNewCameraTarget(Transform newTarget)
    { 
    
    }

    public CinemachineCamera RequestToSetPriorityVCamera(CinemachineCamera priorityCamera, AltVCameraType altVCType = AltVCameraType.SliderMove)
    {
        if (_usingVCamera != null)
        {
            _usingVCamera.Priority = 0;

            if (_usingVCamera != cameraIdleVCamera && _usingVCamera != cameraMoveVCamera)
            {
                _usingVCamera.transform.parent = null;
                SetPriorityVCamera(priorityCamera, altVCType);
                return _usingVCamera;
            }
            SetPriorityVCamera(priorityCamera, altVCType);
        }
        else
            SetPriorityVCamera(priorityCamera, altVCType);

        return null;
    }

    private void SetPriorityVCamera(CinemachineCamera priorityCamera, AltVCameraType altVCType)
    {
        _usingVCamera = priorityCamera;


        switch (altVCType)
        {
            case AltVCameraType.SliderMove:
                //логика движения камеры отрабатывается в классе AlternativeVCameraActivator
                break;
            case AltVCameraType.PlayerPosMove:
                _usingVCamera.transform.parent = transform;
                break;
            case AltVCameraType.BlocedPos:
                //камера остается дочерней к своему родителю и не двигается
                break;
            default:
                break;
        }

        _usingVCamera.Priority = 1;

        if (_usingVCamera != cameraIdleVCamera && _usingVCamera != cameraMoveVCamera)
            _currentTarget = _player.transform;
        else
            _currentTarget = target;

        _usingVCamera.LookAt = _currentTarget;
    }

    public bool ReturnBaseVCamera(CinemachineCamera camera, CinemachineCamera previousCamera)
    {
        if (camera == _usingVCamera)
        {
            RequestToSetPriorityVCamera(previousCamera != null ? previousCamera : cameraMoveVCamera);
            return true;
        }
        else
            return false;
    }

    public float GetCameraDir()
    {
        if (_usingVCamera != null)
            return _usingVCamera.transform.eulerAngles.y;

        Debug.LogError("Virtual Camera Missing", gameObject);
        return 0f;
    }
}
