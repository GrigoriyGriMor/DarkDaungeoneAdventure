using UnityEngine;
using UnityEngine.UI;

public class HookMobileInput : AbstractInputController
{
    [SerializeField] private Button _hookBtn;

    public void Awake()
    {
        _hookBtn.onClick.AddListener(ActivateAction);
        _visual = _hookBtn.gameObject;
    }
}
