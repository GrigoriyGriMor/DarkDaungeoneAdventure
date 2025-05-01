using PlayerControllers;
using Unity.Cinemachine;
using UnityEngine;

public class AlternativeVCinemachineCameraActivator : MonoBehaviour
{
    [SerializeField] private CinemachineCamera _cinemachineCamera;

    [SerializeField] private bool _blockCameraMove = false;

    private void Start()
    {
        if (_cinemachineCamera == null)
        {
            Debug.LogError("Alternative Camera is Null");
            return;
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.TryGetComponent(out PlayerController player))
        {
            player.GetPlayerData().CameraControlBlock.SetPriorityVCamera(_cinemachineCamera, !_blockCameraMove);
            player.SwitchMode(true);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.TryGetComponent(out PlayerController player))
        {
            player.GetPlayerData().CameraControlBlock.ReturnBaseVCamera();
            player.SwitchMode(false);
        }
    }
} 