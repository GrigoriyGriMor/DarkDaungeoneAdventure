using UnityEngine;

public class ButtonInputKeyboard : AbstractInputController
{
    [SerializeField] private KeyCode _keyCode = KeyCode.Space;

    private void Update()
    {
        if (Input.GetKeyDown(_keyCode))
            ActivateAction();
    }
}
