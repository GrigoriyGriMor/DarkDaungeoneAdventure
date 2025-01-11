using Game.Core;
using UnityEngine;
using UnityEngine.UI;

public class InGameHUD : AbstractWindow
{
    [SerializeField] private Button _pauseBtn;

    public override void Init(SupportClasses.WindowName parentWin = SupportClasses.WindowName.None)
    {
        base.Init(parentWin);

        _pauseBtn.onClick.AddListener(() =>
        {
            GameManager.Instance.GetManager<WindowsManager>().OpenWindow(SupportClasses.WindowName.InGamePauseMenu, SupportClasses.WindowName.InGameHUD);
        });
    }
}
