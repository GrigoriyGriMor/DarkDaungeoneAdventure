using UnityEngine;

public class CameraMoveKeyboardController : AbstractInputController
{
    private void Update()
    {
        inputVector = new Vector2(Input.GetAxis("Mouse X"), Input.GetAxis("Mouse Y"));
    }
}
