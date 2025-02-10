using UnityEngine;

public class JumpKeyboardController : AbstractInputController
{
    [SerializeField] private KeyCode _jumpKeyCode = KeyCode.Space;

    private void Update()
    {
        if (Input.GetKeyDown(_jumpKeyCode))
            ActivateAction();
    }
}
