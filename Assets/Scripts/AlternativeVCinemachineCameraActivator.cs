using PlayerControllers;
using System;
using Unity.Cinemachine;
using UnityEngine;

public class AlternativeVCinemachineCameraActivator : MonoBehaviour
{
    [SerializeField] private CinemachineCamera _cinemachineCamera;

    [SerializeField] private AltVCameraType _altVCameraType = AltVCameraType.SliderMove;

    private Collider _colider;
    private CinemachineSplineDolly _dollyTrecker;

    private PlayerController _playerController;
    bool _dollyTreckerActive = false;

    CinemachineCamera _previousAltCamera;

    private Color _gizmoColor = Color.blue; // Цвет для отображения в Gizmo

#if UNITY_EDITOR
    private void OnDrawGizmos()
    {
        if (_colider == null)
            _colider = GetComponent<Collider>();

        if (_colider != null)
        {
            Gizmos.color = _gizmoColor;
            Gizmos.DrawWireCube(_colider.bounds.center, _colider.bounds.size);
        }
    }
#endif

    private void Start()
    {
        if (_cinemachineCamera == null)
        {
            Debug.LogError("Alternative Camera is Null");
            return;
        }

        if (_altVCameraType == AltVCameraType.SliderMove)
        { 
            _colider = GetComponent<Collider>();
            _dollyTrecker = _cinemachineCamera.GetComponent<CinemachineSplineDolly>();

            if (_colider == null || _dollyTrecker == null)
            {
                Debug.LogError("You use Dolly Treked, but CinemachineCamera have't CinemachineSplineDolly component, or this object have't collider");
                _altVCameraType = AltVCameraType.BlocedPos;
            }
        }
    }

    private void Update()
    {
        if (!_dollyTreckerActive || _playerController == null) return;

        // Get the bounds of the collider in local space
        Vector3 localMin = _colider.transform.InverseTransformPoint(_colider.bounds.min);
        Vector3 localMax = _colider.transform.InverseTransformPoint(_colider.bounds.max);
        
        // Convert player position to collider's local space and calculate position in one step
        float relativePosition = Mathf.InverseLerp(
            localMax.x,
            localMin.x,
            _colider.transform.InverseTransformPoint(_playerController.transform.position).x
        );
        
        // Update the dolly tracker position
        _dollyTrecker.CameraPosition = Mathf.Clamp01(relativePosition);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.TryGetComponent(out _playerController))
        {
            _previousAltCamera = _playerController.GetPlayerData().CameraControlBlock.RequestToSetPriorityVCamera(_cinemachineCamera, _altVCameraType);
            _playerController.SwitchMode(true);
        }

        if (_altVCameraType == AltVCameraType.SliderMove)
            _dollyTreckerActive = true;
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.TryGetComponent(out _playerController))
        {
            bool thisIsActualVC = _playerController.GetPlayerData().CameraControlBlock.ReturnBaseVCamera(_cinemachineCamera, _previousAltCamera);
            if (thisIsActualVC)
                _playerController.SwitchMode(false);
        }

        if (_altVCameraType == AltVCameraType.SliderMove)
            _dollyTreckerActive = false;

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