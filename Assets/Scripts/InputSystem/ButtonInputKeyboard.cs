using UnityEngine;

public class ButtonInputKeyboard : AbstractInputController
{
    [SerializeField] private KeyCode _keyCode = KeyCode.Space;
    [SerializeField] private float holdThreshold = 0.5f;
    protected bool isHolding = false;
    protected float holdTime = 0f;
    protected bool holdEventTriggered = false;

    private void Update()
    {
        if (Input.GetKeyDown(_keyCode))
            {
                isHolding = true;
                holdTime = 0f;
                holdEventTriggered = false;

            }
        else
            if(Input.GetKeyUp(_keyCode))
            {
                if (isHolding && holdTime < holdThreshold)
            ActivateAction();
        else 
        if (holdEventTriggered)
            HoldingAction(false);

        // Сброс состояния
        isHolding = false;
        holdTime = 0f;
        holdEventTriggered = false;
            }
            
        if (isHolding && !holdEventTriggered)
        {
            holdTime += Time.deltaTime;
            if (holdTime >= holdThreshold)
            {
                holdEventTriggered = true;
                HoldingAction(true);
            }
        }
    }
}

    
