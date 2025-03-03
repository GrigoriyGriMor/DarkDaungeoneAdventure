using UnityEngine;

public class MoveKeyboardControler : AbstractInputController
{
    private void Update()
    {
        inputVector = new Vector2(Input.GetAxis("Horizontal"), Input.GetAxis("Vertical"));
    }
}
