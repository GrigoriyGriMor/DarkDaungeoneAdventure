using Config;
using Game.Core;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

public class WindowsManager : AbstractManager
{
    private WindowsConfig _winConfig;
    private Dictionary<SupportClasses.WindowName, AbstractWindow> _cachedWindows = new();
    [SerializeField] private RectTransform _canvas;

    private CurrentWindow _currentWindow = new CurrentWindow();
    [SerializeField] private SupportClasses.WindowName _onFirstLoadWindow = SupportClasses.WindowName.MainMenuBasePanel;

    private async void Start()
    {
        while (!GameManager.Instance)
            await Task.Yield();

        _winConfig = GameManager.Instance.GetManager<ConfigManager>().GetConfiguration<WindowsConfig>();

        await OpenWindow(_onFirstLoadWindow);
    }

    public async Task OpenWindow(SupportClasses.WindowName winName, SupportClasses.WindowName parentWin = SupportClasses.WindowName.None, bool parentOpening = false)
    {
        if (_currentWindow._window != null && !parentOpening)
            await _currentWindow._window.CloseWindow();

        if (!_cachedWindows.TryGetValue(winName, out var window))
        {
            var winPrefab = _winConfig.GetWindowsData(winName)?.WinPrefab;
            if (winPrefab == null)
            {
                Debug.LogError($"[WindowsManager] Префаб для окна {winName} не найден.");
                return;
            }

            window = Instantiate(winPrefab, _canvas).GetComponent<AbstractWindow>();
            if (window == null)
            {
                Debug.LogError($"[WindowsManager] Префаб {winName} не содержит компонент AbstractWindow.");
                return;
            }

            _cachedWindows[winName] = window;
        }

        _currentWindow._window = window;
        _currentWindow._name = winName;
        window.gameObject.SetActive(true);
        window.Init(parentWin);
    }

    public AbstractWindow GetCurrentWindowIfType(SupportClasses.WindowName window)
    {
        return window == _currentWindow._name ? _currentWindow._window : null;
    }

    public void CloseAllWindows()
    {
        foreach (var window in _cachedWindows.Values)
        {
            if (window.gameObject.activeInHierarchy)
                window.gameObject.SetActive(false);
        }
    }
}

[Serializable]
public class CurrentWindow
{
    [HideInInspector] public AbstractWindow _window;
    [HideInInspector] public SupportClasses.WindowName _name;
}