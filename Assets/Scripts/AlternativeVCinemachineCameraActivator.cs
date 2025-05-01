using PlayerControllers;
using Unity.Cinemachine;
using UnityEngine;

public class AlternativeVCinemachineCameraActivator : MonoBehaviour
{
    [SerializeField] private CinemachineCamera _cinemachineCamera;

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
            player.GetPlayerData().CameraControlBlock.SetPriorityVCamera(_cinemachineCamera);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.TryGetComponent(out PlayerController player))
            player.GetPlayerData().CameraControlBlock.ReturnBaseVCamera();
    }
} 