using Game.Core;
using UnityEngine;
using UnityEngine.UI;

public class MainSettingWindow : AbstractWindow
{
    [SerializeField] private Button _cancelBtn;

    public override void Init(SupportClasses.WindowName parentWin)
    {
        base.Init(parentWin);

        _cancelBtn.onClick.AddListener(() =>
        {
            CloseWindow();
        });
    }
}
