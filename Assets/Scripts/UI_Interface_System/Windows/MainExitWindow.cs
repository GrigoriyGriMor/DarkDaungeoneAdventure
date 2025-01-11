using Game.Core;
using UnityEngine;
using UnityEngine.UI;

public class MainExitWindow : AbstractWindow
{
    [SerializeField] private Button _exitBtn;
    [SerializeField] private Button _cancelBtn;

    public override void Init(SupportClasses.WindowName parentWin = SupportClasses.WindowName.None)
    {
        base.Init(parentWin);

        _exitBtn.onClick.AddListener(Application.Quit);   

        _cancelBtn.onClick.AddListener(() =>
        {
            CloseWindow();
        });
    }
}
