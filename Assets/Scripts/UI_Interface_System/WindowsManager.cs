using Config;
using Game.Core;
using System.Collections;
using UnityEngine;

public class WindowsManager : AbstractManager
{
    private WindowsConfig _winConfig;
    [SerializeField] private RectTransform _canvas;

    private AbstractWindow _currentWindow;

    private IEnumerator Start()
    {
        while (!GameManager.Instance)
            yield return null;

        _winConfig = GameManager.Instance.GetManager<ConfigManager>().GetConfiguration<WindowsConfig>();

        OpenWindow(SupportClasses.WindowName.MainMenuBasePanel);
    }

    public async void OpenWindow(SupportClasses.WindowName winName, SupportClasses.WindowName parentWin = SupportClasses.WindowName.None, bool parentOpening = false)
    {
        if (_currentWindow != null && !parentOpening)
            await _currentWindow.CloseWindow();

        _currentWindow = Instantiate(_winConfig.GetWindowsData(winName).WinPrefab, _canvas).GetComponent<AbstractWindow>();
        _currentWindow.Init(parentWin);
    }
}
