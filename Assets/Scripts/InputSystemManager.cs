using Game.Core;
using System;
using UnityEngine;

public class InputSystemManager : AbstractManager
{
    private Vector2 _moveCharacter;
    private Vector2 _cameraMove;
    public Action _jumpAction;
    private bool _flying;

    private AbstractInputController _moveController;
    private AbstractInputController _cameraMoveController;
    private AbstractInputController _jumpController;

    public void RegisterController(AbstractInputController controller, InputControllerType controlType)
    { 
    
    }

    public Vector2 Move()
    {
        return _moveCharacter;
    }

    public Vector2 CameraMove() 
    {
        return _cameraMove;
    }
}
