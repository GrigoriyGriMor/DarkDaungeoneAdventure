using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public static class SupportClasses
{


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