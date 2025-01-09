using Config;
using Game.Core;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WindowsManager : AbstractManager
{
    private WindowsConfig _winConfig;
    [SerializeField] private RectTransform _canvas;

    private AbstractWindow _currentWindow;
    private AbstractWindow _mainHUD;

    private IEnumerator Start()
    {
        while (!GameManager.Instance)
            yield return null;

        _winConfig = GameManager.Instance.GetManager<ConfigManager>().GetConfiguration<WindowsConfig>();

        OpenWindow(SupportClasses.WindowName.InGameHUD);
    }

    public void OpenWindow(SupportClasses.WindowName winName, SupportClasses.WindowName parentWin = SupportClasses.WindowName.None)
    {
        _currentWindow = Instantiate(_winConfig.GetWindowsData(winName).WinPrefab, _canvas).GetComponent<AbstractWindow>();
        _currentWindow.ParentWindow = parentWin;
    }
}
