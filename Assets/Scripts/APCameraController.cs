using BaseClasses;
using System.Collections;
using Unity.Cinemachine;
using UnityEngine;

public class APCameraController : MonoBehaviour
{
    private Transform _player;

    [SerializeField] private CinemachineCamera _usingVCamera;

    [SerializeField] private CinemachineCamera cameraIdleVCamera;
    [SerializeField] private CinemachineCamera cameraMoveVCamera;

    [SerializeField] private Transform target;

    private IEnumerator Start()
    {
        cameraIdleVCamera.LookAt = target;
        cameraMoveVCamera.LookAt = target;

        SetPriorityVCamera(cameraIdleVCamera);

        while (_player == null || !LevelManager.Instance)
        {
            _player = LevelManager.Instance.PlayerController.transform;
            yield return null;
        }
    }

    private void Update()
    {
        transform.position = _player.position;
    }

    public void StartMove()
    {
        if (_usingVCamera != cameraIdleVCamera || _usingVCamera != cameraMoveVCamera)
            return;

        SetPriorityVCamera(cameraMoveVCamera);
    }

    public void StopMove()
    {
        if (_usingVCamera != cameraIdleVCamera && _usingVCamera != cameraMoveVCamera)
            return;

        SetPriorityVCamera(cameraIdleVCamera);
    }

    public void SetNewCameraTarget(Transform newTarget)
    { 
    
    }

    public void SetPriorityVCamera(CinemachineCamera priorityCamera)
    {
        if (_usingVCamera != null)
        {
            _usingVCamera.Priority = 0;

            if (_usingVCamera != cameraIdleVCamera && _usingVCamera != cameraMoveVCamera)
                _usingVCamera.transform.parent = null;
        }

        _usingVCamera = priorityCamera;
        _usingVCamera.transform.parent = transform;
        _usingVCamera.Priority = 1;
        _usingVCamera.LookAt = target;
    }

    public void ReturnBaseVCamera()
    {
        SetPriorityVCamera(cameraMoveVCamera);
    }

    public float GetCameraDir()
    {
        if (_usingVCamera != null)
            return _usingVCamera.transform.eulerAngles.y;

        Debug.LogError("Virtual Camera Missing", gameObject);
        return 0f;
    }
}
