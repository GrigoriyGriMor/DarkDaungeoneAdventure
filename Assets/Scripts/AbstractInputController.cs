using Game.Core;
using System.Collections;
using UnityEngine;

public abstract class AbstractInputController : MonoBehaviour
{
    [SerializeField] private InputControllerType _controlType = InputControllerType.Empty;
    internal Vector2 inputVector;

    public virtual IEnumerator Start()
    {
        while (!GameManager.Instance)
            yield return new WaitForFixedUpdate();

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
}

public enum InputControllerType
{
    Empty,
    KeyboardMove,
    KeyboardJump,
    MouseCameraMove,
    JoystickMove,
    JoystickCameraMove,
    TouchJump,
    SliderCameraMove,
}