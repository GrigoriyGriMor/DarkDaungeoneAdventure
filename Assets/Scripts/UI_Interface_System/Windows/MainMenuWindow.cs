using Game.Core;
using UnityEngine;
using UnityEngine.UI;

public class MainMenuWindow : AbstractWindow
{
    [SerializeField] private Button _startGameBtn;
    [SerializeField] private Button _settingBtn;
    [SerializeField] private Button _exitBtn;

    private WindowsManager _windowsManager;

    public override void Init(SupportClasses.WindowName parentWin = SupportClasses.WindowName.None)
    {
        base.Init(parentWin);
        _windowsManager = GameManager.Instance.GetManager<WindowsManager>();

        _startGameBtn.onClick.AddListener(() =>
        {
            Debug.LogError("Game was started");
        });

        _settingBtn.onClick.AddListener(() =>
        {
            _windowsManager.OpenWindow(SupportClasses.WindowName.MainSettingMenu, SupportClasses.WindowName.MainMenuBasePanel);
        });

        _exitBtn.onClick.AddListener(() =>
        {
            _windowsManager.OpenWindow(SupportClasses.WindowName.MainExitMenu, SupportClasses.WindowName.MainMenuBasePanel);
        });
    }
}
