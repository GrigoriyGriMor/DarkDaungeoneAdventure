using PlayerControllers;
using System;
using Unity.Cinemachine;
using UnityEngine;

public class AlternativeVCinemachineCameraActivator : MonoBehaviour
{
    [SerializeField] private CinemachineCamera _cinemachineCamera;
    [SerializeField] private AltVCameraType _altVCameraType = AltVCameraType.SliderMove;

    private Collider _collider;
    private CinemachineSplineDolly _dollyTracker;
    private PlayerController _playerController;
    private bool _dollyTrackerActive;
    private VCameraConteiner _previousAltCamera;
    private VCameraConteiner _cachedCameraContainer;
    private readonly Color _gizmoColor = Color.blue;

    public CinemachineCamera Camera => _cinemachineCamera;
    public AltVCameraType CameraType => _altVCameraType;
    public bool IsDollyTrackerActive => _dollyTrackerActive;

#if UNITY_EDITOR
    private void OnDrawGizmos()
    {
        if (_collider == null)
            _collider = GetComponent<Collider>();

        if (_collider != null)
        {
            Gizmos.color = _gizmoColor;
            Gizmos.DrawWireCube(_collider.bounds.center, _collider.bounds.size);
        }
    }
#endif

    private void Awake()
    {
        _cachedCameraContainer = new VCameraConteiner(_cinemachineCamera, this);
    }

    private void Start()
    {
        ValidateComponents();
        InitializeComponents();
    }

    private void ValidateComponents()
    {
        if (_cinemachineCamera == null)
        {
            Debug.LogError("Alternative Camera is Null", this);
            enabled = false;
            return;
        }

        if (_altVCameraType == AltVCameraType.SliderMove)
        {
            _collider = GetComponent<Collider>();
            _dollyTracker = _cinemachineCamera.GetComponent<CinemachineSplineDolly>();

            if (_collider == null || _dollyTracker == null)
            {
                Debug.LogError("You use Dolly Tracked, but CinemachineCamera hasn't CinemachineSplineDolly component, or this object hasn't collider", this);
                _altVCameraType = AltVCameraType.BlocedPos;
            }
        }
    }

    private void InitializeComponents()
    {
        if (_altVCameraType == AltVCameraType.SliderMove)
        {
            _collider = GetComponent<Collider>();
            _dollyTracker = _cinemachineCamera.GetComponent<CinemachineSplineDolly>();
        }
    }

    private void Update()
    {
        if (!_dollyTrackerActive || _playerController == null) return;

        UpdateDollyTrackerPosition();
    }

    private void UpdateDollyTrackerPosition()
    {
        if (_collider == null || _dollyTracker == null) return;

        Vector3 localMin = _collider.transform.InverseTransformPoint(_collider.bounds.min);
        Vector3 localMax = _collider.transform.InverseTransformPoint(_collider.bounds.max);
        
        float relativePosition = Mathf.InverseLerp(
            localMax.x,
            localMin.x,
            _collider.transform.InverseTransformPoint(_playerController.transform.position).x
        );
        
        _dollyTracker.CameraPosition = Mathf.Clamp01(relativePosition);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!other.TryGetComponent(out _playerController)) return;

        _previousAltCamera = _playerController.GetPlayerData().CameraControlBlock.RequestToSetPriorityVCamera(_cachedCameraContainer, _altVCameraType);
        
        if (_previousAltCamera == null)
        {
            _playerController.SwitchMode(true);
        }

        if (_altVCameraType == AltVCameraType.SliderMove)
        {
            _dollyTrackerActive = true;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (!other.TryGetComponent(out _playerController)) return;

        bool thisIsActualVC = _playerController.GetPlayerData().CameraControlBlock.ReturnBaseVCamera(_cachedCameraContainer, _previousAltCamera);
        
        if (thisIsActualVC)
        {
            _playerController.SwitchMode(false);
        }

        _previousAltCamera = null;
        _dollyTrackerActive = false;
        _playerController = null;
    }

    public void ClearPreviousCamera()
    {
        _previousAltCamera = null;
    }

    private void OnDestroy()
    {
        _previousAltCamera = null;
        _playerController = null;
    }
}

[Serializable]
public enum AltVCameraType
{ 
    Empty,
    PlayerPosMove,
    SliderMove,
    BlocedPos,
}