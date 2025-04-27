using Game.Core;
using System;
using UnityEngine;

public class InputSystemManager : AbstractManager
{
    public ActionBlock _jumpAction = new ActionBlock();
    public ActionBlock _hookAction = new ActionBlock();
    public ActionBlock _getItemAction = new ActionBlock();
    public ActionBlock _putItemAction = new ActionBlock();
    public ActionBlock _attackAction = new ActionBlock();

    private AbstractInputController _moveController = null;
    private AbstractInputController _cameraMoveController = null;
    private AbstractInputController _jumpController = null;
    private AbstractInputController _hookBreakController = null;
    private AbstractInputController _getItemController = null;
    private AbstractInputController _putItemController = null;
    private AbstractInputController _attackController = null;

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
                _jumpController._action += () => _jumpAction._clickAction.Invoke();
                _jumpController._holdAction += () => _jumpAction._holdStartAction.Invoke();
                _jumpController._releaseHoldAction += () => _jumpAction._holdEndAction.Invoke();
                break;
            case InputControllerType.HookBreak:
                _hookBreakController = controller;
                _hookBreakController._action += () => _hookAction._clickAction.Invoke();
                break;
            case InputControllerType.ItemGet:
                _getItemController = controller;
                _getItemController._action += () => _getItemAction._clickAction.Invoke();// Unity magic (why that not work how three action before?)
                break;
            case InputControllerType.ItemPut:
                _putItemController = controller;
                _putItemController._action += () => _putItemAction._clickAction.Invoke();// Unity magic (why that not work how three action before?)
                break;
            case InputControllerType.Attack:
                _attackController = controller;
                if (_attackAction != null && _attackAction._clickAction != null)
                    _attackController._action += _attackAction._clickAction;
                break;
            default:
                break;
        }
    }

    public GameObject GetVisual(InputControllerType _type)
    {
        switch (_type)
        {
            case InputControllerType.Jump:
                if (_jumpController != null)
                    return _jumpController.GetVisual();
                else
                    return null;
            case InputControllerType.HookBreak:
                if (_hookBreakController != null)
                    return _hookBreakController.GetVisual();
                else
                    return null;
            case InputControllerType.ItemGet:
                if (_getItemController != null)
                    return _getItemController.GetVisual();
                else
                    return null;
            case InputControllerType.ItemPut:
                if (_putItemController != null)
                    return _putItemController.GetVisual();
                else
                    return null;
            default:
                return null;
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
        _jumpController._action -= _jumpAction._clickAction;
        _hookBreakController._action -= _hookAction._clickAction;
        _getItemController._action -= _getItemAction._clickAction;
        _putItemController._action -= _putItemAction._clickAction;
    }
}

[Serializable]
public class ActionBlock
{
    public Action _clickAction;

    public Action _holdStartAction;
    public Action _holdEndAction;
}