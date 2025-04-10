using UnityEngine;

public class ButtonInputKeyboard : AbstractInputController
{
    [SerializeField] private KeyCode _keyCode = KeyCode.Space;
    [SerializeField] private float holdThreshold = 0.5f;
    [SerializeField] private bool _useActionAtDown = false;

    protected bool isHolding = false;
    protected float holdTime = 0f;
    protected bool holdEventTriggered = false;

    protected virtual void Update()
    {
        if (isHolding)
        {
            holdTime += Time.deltaTime;
            if (!holdEventTriggered && holdTime >= holdThreshold)
            {
                holdEventTriggered = true;
                HoldingAction(true); // Запуск события удержания
            }
        }

        if (Input.GetKeyDown(_keyCode))
        {
            if (_useActionAtDown) ActivateAction(); // Короткий клик

            isHolding = true;
            holdTime = 0f;
            holdEventTriggered = false;
        }

        if (Input.GetKeyUp(_keyCode))
        {
            if (isHolding && holdTime < holdThreshold)
	    {
		if (!_useActionAtDown)
		    ActivateAction();
	    }
            else 
                if (holdEventTriggered)
                    HoldingAction(false); // Завершение удержания

	    ResetState();
        }
    }

    private void ResetState()
    {
        isHolding = false;
        holdTime = 0f;
        holdEventTriggered = false;
    }
}

    
