using UnityEngine;
using UnityEngine.UI;

public class JumpMobileController : AbstractInputController
{
    [SerializeField] private Button _jumpBtn;

    public void Awake()
    {
        _jumpBtn.onClick.AddListener(ActivateAction);
    }
}
