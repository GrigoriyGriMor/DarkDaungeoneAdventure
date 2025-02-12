using Game.Core;
using System;
using System.Collections;
using UnityEngine;

public abstract class AbstractInputController : MonoBehaviour
{
    [SerializeField] private InputControllerType _controlType = InputControllerType.Empty;
    public Action _action;
    
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
}

[Serializable]
public enum InputControllerType
{
    Empty,
    Move,
    Jump,
    CameraMove,
}