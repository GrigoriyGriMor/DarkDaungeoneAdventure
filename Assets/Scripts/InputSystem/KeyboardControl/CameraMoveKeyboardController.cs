using UnityEngine;

public class CameraMoveKeyboardController : AbstractInputController
{
    [SerializeField] private float _rotateSpeed = 5f;
    private void Update()
    {
        if (Input.GetMouseButton(1))
            inputVector = new Vector2(Input.GetAxis("Mouse X"), Input.GetAxis("Mouse Y"));
    }
}
