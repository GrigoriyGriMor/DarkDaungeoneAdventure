using UnityEngine;
using UnityEngine.UI;

public class ButtonInputMobile : AbstractInputController
{
    [SerializeField] private Button _button;

    public void Awake()
    {
        _button.onClick.AddListener(ActivateAction);
        if (_visual == null)
            _visual = _button.gameObject;
    }
}
