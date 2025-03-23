using Game.Core;
using System;
using System.Collections;
using UnityEngine;

public abstract class AbstractInputController : MonoBehaviour
{
    [SerializeField] private InputControllerType _controlType = InputControllerType.Empty;
    public InputControllerType ControlType { get { return _controlType; } }

    public GameObject _visual;
    public Action _action;

    public Action _holdAction;
    public Action _releaseHoldAction;

    internal Vector2 inputVector;

    public virtual IEnumerator Start()
    {
        while (!GameManager.Instance)
            yield return null;

        GameManager.Instance.GetManager<InputSystemManager>().RegisterController(this, _controlType);
    }

    public virtual float HorizontalAxis()
    {
        return Mathf.Clamp(inputVector.x, -1f, 1f);
    }

    public virtual float VerticalAxis()
    {
        return Mathf.Clamp(inputVector.y, -1f, 1f);
    }

    public virtual void ActivateAction()
    {
        _action.Invoke();
    }

    public virtual void HoldingAction(bool holdStart)
    { 
        if (holdStart)
            _holdAction.Invoke();
        else
            _releaseHoldAction.Invoke();
    }

    public GameObject GetVisual()
    {
        if (_visual == null)
        {
            Debug.LogError("Object with name " + name + " dont have visual");
            return null;
        }
        else
            return _visual;
    }
}

[Serializable]
public enum InputControllerType
{
    Empty,
    Move,
    Jump,
    CameraMove,
    HookBreak,
    ItemGet,
    ItemPut,
}