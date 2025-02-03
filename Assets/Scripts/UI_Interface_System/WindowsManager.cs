using Config;
using Game.Core;
using System;
using System.Collections;
using UnityEngine;

public class WindowsManager : AbstractManager
{
    private WindowsConfig _winConfig;
    [SerializeField] private RectTransform _canvas;

    private CurrentWindow _currentWindow = new CurrentWindow();

    private IEnumerator Start()
    {
        while (!GameManager.Instance)
            yield return null;

        _winConfig = GameManager.Instance.GetManager<ConfigManager>().GetConfiguration<WindowsConfig>();

        OpenWindow(SupportClasses.WindowName.MainMenuBasePanel);
    }

    public async void OpenWindow(SupportClasses.WindowName winName, SupportClasses.WindowName parentWin = SupportClasses.WindowName.None, bool parentOpening = false)
    {
        if (_currentWindow._window != null && !parentOpening)
            await _currentWindow._window.CloseWindow();

        _currentWindow._window = Instantiate(_winConfig.GetWindowsData(winName).WinPrefab, _canvas).GetComponent<AbstractWindow>();
        _currentWindow._name = winName;
        _currentWindow._window.Init(parentWin);
    }

    public AbstractWindow GetCurrentWindowIfType(SupportClasses.WindowName window)
    { 
        if (window == _currentWindow._name)
            return _currentWindow._window;
        else
            return null;
    }
}

[Serializable]
public class CurrentWindow
{
    [HideInInspector] public AbstractWindow _window;
    [HideInInspector] public SupportClasses.WindowName _name;
}
