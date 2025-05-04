using BaseClasses;
using PlayerControllers;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using Unity.Cinemachine;
using UnityEngine;

public class APCameraController : MonoBehaviour
{
    [SerializeField] private VCameraConteiner _usingVCamera;
    [SerializeField] private CinemachineCamera cameraIdleVCamera;
    [SerializeField] private CinemachineCamera cameraMoveVCamera;
    [SerializeField] private Transform target;

    private Transform _currentTarget;
    private PlayerController _playerController;
    private Transform _player;
    private readonly Dictionary<CinemachineCamera, VCameraConteiner> _cameraCache = new Dictionary<CinemachineCamera, VCameraConteiner>();

    private void Awake()
    {
        InitializeCameraCache();
    }

    private void InitializeCameraCache()
    {
        _cameraCache[cameraIdleVCamera] = VCameraConteiner.CreateDefault(cameraIdleVCamera);
        _cameraCache[cameraMoveVCamera] = VCameraConteiner.CreateDefault(cameraMoveVCamera);
    }

    private IEnumerator Start()
    {
        _usingVCamera = _cameraCache[cameraIdleVCamera];
        _currentTarget = target;

        cameraIdleVCamera.LookAt = _currentTarget;
        cameraMoveVCamera.LookAt = _currentTarget;

        RequestToSetPriorityVCamera(_usingVCamera);

        while (_player == null || !LevelManager.Instance)
        {
            if (LevelManager.Instance?.PlayerController != null)
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
        if (IsNotDefaultCameras(_usingVCamera.Camera) || _usingVCamera.Camera == cameraMoveVCamera)
            return;

        RequestToSetPriorityVCamera(_cameraCache[cameraMoveVCamera]);
    }

    public void StopMove()
    {
        if (IsNotDefaultCameras(_usingVCamera.Camera) || _usingVCamera.Camera == cameraIdleVCamera)
            return;

        RequestToSetPriorityVCamera(_cameraCache[cameraIdleVCamera]);
    }

    public VCameraConteiner RequestToSetPriorityVCamera(VCameraConteiner priorityCamera, AltVCameraType altVCType = AltVCameraType.SliderMove)
    {
        if (priorityCamera == null)
            throw new ArgumentNullException(nameof(priorityCamera));

        VCameraConteiner lastCamera = null;
        _usingVCamera.Camera.Priority = 0;

        if (IsNotDefaultCameras(_usingVCamera.Camera))
        {
            _usingVCamera.Camera.transform.parent = null;
            lastCamera = _usingVCamera;
        }

        SetPriorityVCamera(priorityCamera, altVCType);
        return lastCamera;
    }

    private void SetPriorityVCamera(VCameraConteiner priorityCamera, AltVCameraType altVCType)
    {
        _usingVCamera = priorityCamera;

        switch (altVCType)
        {
            case AltVCameraType.SliderMove:
                //логика движения камеры отрабатывается в классе AlternativeVCameraActivator
                break;
            case AltVCameraType.PlayerPosMove:
                _usingVCamera.Camera.transform.parent = transform;
                break;
            case AltVCameraType.BlocedPos:
                //камера остается дочерней к своему родителю и не двигается
                break;
        }

        _usingVCamera.Camera.Priority = 1;
        _currentTarget = IsNotDefaultCameras(_usingVCamera.Camera) ? _player.transform : target;
        _usingVCamera.Camera.LookAt = _currentTarget;
    }

    public bool ReturnBaseVCamera(VCameraConteiner camera, VCameraConteiner previousCamera)
    {
        if (camera == null)
            throw new ArgumentNullException(nameof(camera));

        if (IsNotDefaultCameras(_usingVCamera.Camera) && _usingVCamera.Camera != camera.Camera)
        {
            _usingVCamera.Activator?.ClearPreviousCamera();
            return false;
        }

        var nextCamera = previousCamera ?? _cameraCache[cameraMoveVCamera];
        RequestToSetPriorityVCamera(nextCamera);

        return previousCamera == null;
    }

    private bool IsNotDefaultCameras(CinemachineCamera camera) =>
        camera != cameraIdleVCamera && camera != cameraMoveVCamera;

    public float GetCameraDir()
    {
        if (_usingVCamera?.Camera != null)
            return _usingVCamera.Camera.transform.eulerAngles.y;

        Debug.LogError("Virtual Camera Missing", gameObject);
        return 0f;
    }
}

[Serializable]
public class VCameraConteiner
{
    public AlternativeVCinemachineCameraActivator Activator { get; }
    public CinemachineCamera Camera { get; }

    public VCameraConteiner(CinemachineCamera camera, AlternativeVCinemachineCameraActivator activator = null)
    {
        Camera = camera ?? throw new ArgumentNullException(nameof(camera));
        Activator = activator;
    }

    public static VCameraConteiner CreateDefault(CinemachineCamera camera) => 
        new VCameraConteiner(camera);

    public override bool Equals(object obj)
    {
        if (obj is VCameraConteiner other)
        {
            return Camera == other.Camera && Activator == other.Activator;
        }
        return false;
    }

    public override int GetHashCode()
    {
        return HashCode.Combine(Camera, Activator);
    }

    public static bool operator ==(VCameraConteiner left, VCameraConteiner right)
    {
        if (ReferenceEquals(left, null))
            return ReferenceEquals(right, null);
        return left.Equals(right);
    }

    public static bool operator !=(VCameraConteiner left, VCameraConteiner right)
    {
        return !(left == right);
    }
}
