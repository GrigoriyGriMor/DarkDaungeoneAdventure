using BaseClasses;
using PlayerControllers;
using System;
using System.Collections;
using System.Threading.Tasks;
using TMPro.Examples;
using Unity.Cinemachine;
using Unity.Mathematics;
using UnityEngine;

public class APCameraController : MonoBehaviour
{
    [SerializeField] private VCameraConteiner _usingVCamera;

    [SerializeField] private CinemachineCamera cameraIdleVCamera;
    [SerializeField] private CinemachineCamera cameraMoveVCamera;

    [SerializeField] private Transform target;
    private Transform _currentTarget = null;

    private PlayerController _playerController;
    private Transform _player;

    private IEnumerator Start()
    {
        _usingVCamera._aVCamera = cameraIdleVCamera;
        _usingVCamera._activator = null;

        _currentTarget = target;

        RequestToSetPriorityVCamera(_usingVCamera);

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
        if (IsNotDefaultCameras(_usingVCamera._aVCamera) || _usingVCamera._aVCamera == cameraMoveVCamera)
            return;

        VCameraConteiner vCam = new VCameraConteiner { _aVCamera = cameraMoveVCamera, _activator = null };
        RequestToSetPriorityVCamera(vCam);
    }

    public void StopMove()
    {
        if (IsNotDefaultCameras(_usingVCamera._aVCamera) || _usingVCamera._aVCamera == cameraIdleVCamera)
            return;

        VCameraConteiner vCam = new VCameraConteiner { _aVCamera = cameraIdleVCamera, _activator = null };
        RequestToSetPriorityVCamera(vCam);
    }

    public void SetNewCameraTarget(Transform newTarget)
    { 
    
    }

    public VCameraConteiner RequestToSetPriorityVCamera(VCameraConteiner priorityCamera, AltVCameraType altVCType = AltVCameraType.SliderMove)
    {
        VCameraConteiner lastCamera = null;
        _usingVCamera._aVCamera.Priority = 0;

        if (IsNotDefaultCameras(_usingVCamera._aVCamera))
        {
            _usingVCamera._aVCamera.transform.parent = null;
            lastCamera = _usingVCamera;
        }

        SetPriorityVCamera(priorityCamera, altVCType);
        return lastCamera;
    }

    private async void SetPriorityVCamera(VCameraConteiner priorityCamera, AltVCameraType altVCType)
    {
        _usingVCamera = priorityCamera;

        switch (altVCType)
        {
            case AltVCameraType.SliderMove:
                //логика движения камеры отрабатывается в классе AlternativeVCameraActivator
                break;
            case AltVCameraType.PlayerPosMove:
                _usingVCamera._aVCamera.transform.parent = transform;
                break;
            case AltVCameraType.BlocedPos:
                //камера остается дочерней к своему родителю и не двигается
                break;
            default:
                break;
        }

        _usingVCamera._aVCamera.Priority = 1;

        if (IsNotDefaultCameras(_usingVCamera._aVCamera))
            _currentTarget = _player.transform;
        else
            _currentTarget = target;

        _usingVCamera._aVCamera.LookAt = _currentTarget;
    }

    public bool ReturnBaseVCamera(VCameraConteiner camera, VCameraConteiner previousCamera)
    {
        if (IsNotDefaultCameras(_usingVCamera._aVCamera) && _usingVCamera._aVCamera != camera._aVCamera)
        {
            _usingVCamera._activator.ClearPreviousCamera();
            return false;
        }

        VCameraConteiner vCam = previousCamera != null ? previousCamera : new VCameraConteiner { _aVCamera = cameraMoveVCamera, _activator = null };
        RequestToSetPriorityVCamera(vCam);

        if (previousCamera != null)
            return false;
        else
            return true;
    }

    bool IsNotDefaultCameras(CinemachineCamera camera)
    {
        if (camera != cameraIdleVCamera && camera != cameraMoveVCamera)
            return true;
        else
            return false;
    }

    public float GetCameraDir()
    {
        if (_usingVCamera != null)
            return _usingVCamera._aVCamera.transform.eulerAngles.y;

        Debug.LogError("Virtual Camera Missing", gameObject);
        return 0f;
    }
}

[Serializable]
public class VCameraConteiner
{
    public AlternativeVCinemachineCameraActivator _activator { get; set; }
    public CinemachineCamera _aVCamera { get; set; }
}
