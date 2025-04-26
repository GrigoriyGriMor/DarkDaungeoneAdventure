using System;
using UnityEngine;

public static class SupportClasses
{
    public enum WindowName
    {
        None,
        MainMenuBasePanel,
        MainSettingMenu,
        MainExitMenu,
        InGameHUD,
        InGamePauseMenu,
        InGameSettingMenu,
        RespawnMenu,
        BackToMenuMenu
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