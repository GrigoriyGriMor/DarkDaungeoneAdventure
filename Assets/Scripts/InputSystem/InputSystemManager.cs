using Game.Core;
using System;
using UnityEngine;

public class InputSystemManager : AbstractManager
{
    public Action _jumpAction;
    private bool _flying;

    private AbstractInputController _moveController = null;
    private AbstractInputController _cameraMoveController = null;
    private AbstractInputController _jumpController = null;

    public void RegisterController(AbstractInputController controller, InputControllerType controlType)
    {
        switch (controlType)
        {
            case InputControllerType.Move:
                _moveController = controller;
                break;
            case InputControllerType.CameraMove:
                _cameraMoveController = controller;
                break;
            case InputControllerType.Jump:
                _jumpController = controller;
                _jumpController._action += _jumpAction;
                break;
            default:
                break;
        }
    }

    public Vector2 Move()
    {
        if (_moveController == null)
            return Vector2.zero;

        return new Vector2(_moveController.HorizontalAxis(), _moveController.VerticalAxis());
    }

    public Vector2 CameraMove() 
    {
        if (_cameraMoveController == null)
            return Vector2.zero;

        return new Vector2(_cameraMoveController.HorizontalAxis(), _cameraMoveController.VerticalAxis());
    }

    private void OnDisable()
    {
        _jumpController._action -= _jumpAction;
    }
}

//[Serializable]
//public class InputController
//{
//    public InputControllerType ControllerType;
//    public AbstractInputController Controller;
//}