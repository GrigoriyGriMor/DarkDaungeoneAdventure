﻿/* =======================================JoystickStick===================================
 * Класс контролирует джостик и его позицию при нажатии на экран
 * =======================================================================================*/

using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace PlayerControllers
{
    public class JoystickStickk : AbstractInputController, IDragHandler,
        IPointerDownHandler, IPointerUpHandler
    {
        [SerializeField] private bool useVerticalAxies = true;
        [SerializeField] private bool useHorizontalAxis = true;

        [SerializeField] private Image tr_plane;
        [SerializeField] private Image tr_Joystick;
        [SerializeField] private Image tr_Stick;

        public Vector2 _stickPos;

        public void OnPointerDown(PointerEventData eventData)
        {
            Vector2 stickPos;
            if (RectTransformUtility.ScreenPointToLocalPointInRectangle(tr_plane.rectTransform, eventData.position, eventData.pressEventCamera, out stickPos))
                tr_Joystick.rectTransform.anchoredPosition = stickPos;

            OnDrag(eventData);
        }

        public void OnDrag(PointerEventData eventData)
        {
            Vector2 stickPos;
            if (RectTransformUtility.ScreenPointToLocalPointInRectangle(tr_Joystick.rectTransform, eventData.position, eventData.pressEventCamera, out stickPos))
            {
                if (useHorizontalAxis) stickPos.x = stickPos.x / (tr_Joystick.rectTransform.sizeDelta.x / 2);
                if (useVerticalAxies) stickPos.y = stickPos.y / (tr_Joystick.rectTransform.sizeDelta.y / 2);
            }

            _stickPos = stickPos;
            inputVector = new Vector2(stickPos.x, stickPos.y);
            inputVector = (inputVector.magnitude > 1.0f) ? inputVector.normalized : inputVector;
            tr_Stick.rectTransform.anchoredPosition = new Vector2(inputVector.x * (-tr_Stick.rectTransform.sizeDelta.x), inputVector.y * (-tr_Stick.rectTransform.sizeDelta.y));
        }

        public void OnPointerUp(PointerEventData eventData)
        {
            inputVector = Vector2.zero;
            _stickPos = Vector2.zero;
            tr_Stick.rectTransform.anchoredPosition = Vector2.zero;
        }
    }
}