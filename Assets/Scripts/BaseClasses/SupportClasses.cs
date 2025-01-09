using System;
using UnityEngine;

public static class SupportClasses
{
    public enum WindowName
    {
        None,
        MMPanel,
        MMSettingPanel,
        MMExitPanel,
        GMPanel,
        GMSetting,
        InGameHUD,
    }

    public enum WindowType
    { 
        None,
        Window,
        Notification,
    }

}

[Serializable]
public class PlayerData
{
    [HideInInspector] public Transform PlayerBase;
    [HideInInspector] public Transform PlayerVisual;
    [HideInInspector] public Rigidbody PlayerRB;
    [HideInInspector] public Animator PlayerAnimator;
    [HideInInspector] public APCameraController PlayerMainCamera;
    [HideInInspector] public Transform CameraControlBlock;
}