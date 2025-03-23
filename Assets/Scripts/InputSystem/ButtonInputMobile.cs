using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class ButtonInputMobile : AbstractInputController, IPointerDownHandler, IPointerUpHandler
{
    [SerializeField] private Button _button;

    // Порог удержания (в секундах) для различения короткого клика и удержания
    [SerializeField] private float holdThreshold = 0.5f;
    protected bool isHolding = false;
    protected float holdTime = 0f;
    protected bool holdEventTriggered = false;

    public void Awake()
    {
        if (_visual == null)
            _visual = _button.gameObject;
    }

    #region Hold Logic
    // При нажатии кнопки начинаем отслеживать время удержания
    public virtual void OnPointerDown(PointerEventData eventData)
    {
        isHolding = true;
        holdTime = 0f;
        holdEventTriggered = false;
    }

    // При отпускании кнопки, если удержание меньше порога – считаем это коротким кликом,
    // иначе завершаем удержание.
    public virtual void OnPointerUp(PointerEventData eventData)
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

    protected virtual void Update()
    {
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
    #endregion
}
